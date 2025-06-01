using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OverALL.Data;
using OverALL.Data.Models;

namespace OverALL.Services;

/// <summary>
/// PDF项目服务 - 管理项目的创建、查询、更新等操作
/// </summary>
public class PdfProjectService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PdfProjectService> _logger;

    public PdfProjectService(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<PdfProjectService> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// 创建新的PDF项目
    /// </summary>
    public async Task<PdfProject> CreateProjectAsync(string name, string? description, string userId)
    {
        // 生成项目文件夹路径
        var projectId = Guid.NewGuid().ToString("N");
        var projectFolder = Path.Combine(_environment.ContentRootPath, "ProjectFiles", projectId);

        // 创建文件夹
        Directory.CreateDirectory(projectFolder);

        var project = new PdfProject
        {
            Name = name,
            Description = description,
            ProjectFolder = projectFolder,
            UserId = userId,
            Status = ProjectStatus.Created,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.PdfProjects.Add(project);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created project {ProjectId} for user {UserId}", project.Id, userId);
        return project;
    }

    /// <summary>
    /// 获取用户的所有项目
    /// </summary>
    public async Task<List<PdfProject>> GetUserProjectsAsync(string userId)
    {
        return await _context.PdfProjects
            .Where(p => p.UserId == userId)
            .Include(p => p.Documents)
            .Include(p => p.ProcessingSteps)
            .Include(p => p.GeneratedPpts)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 根据ID获取项目
    /// </summary>
    public async Task<PdfProject?> GetProjectByIdAsync(int projectId, string userId)
    {
        return await _context.PdfProjects
            .Where(p => p.Id == projectId && p.UserId == userId)
            .Include(p => p.Documents)
            .Include(p => p.ProcessingSteps.OrderBy(s => s.StartedAt))
            .Include(p => p.GeneratedPpts)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 更新项目状态
    /// </summary>
    public async Task<bool> UpdateProjectStatusAsync(int projectId, ProjectStatus status, string userId)
    {
        var project = await _context.PdfProjects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
            return false;

        project.Status = status;
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated project {ProjectId} status to {Status}", projectId, status);
        return true;
    }

    /// <summary>
    /// 删除项目
    /// </summary>
    public async Task<bool> DeleteProjectAsync(int projectId, string userId)
    {
        var project = await _context.PdfProjects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
            return false;

        // 删除项目文件夹
        try
        {
            if (Directory.Exists(project.ProjectFolder))
            {
                Directory.Delete(project.ProjectFolder, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete project folder {ProjectFolder}", project.ProjectFolder);
        }

        // 删除数据库记录
        _context.PdfProjects.Remove(project);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted project {ProjectId}", projectId);
        return true;
    }

    /// <summary>
    /// 获取项目统计信息
    /// </summary>
    public async Task<ProjectStatistics> GetProjectStatisticsAsync(string userId)
    {
        var projects = await _context.PdfProjects
            .Where(p => p.UserId == userId)
            .ToListAsync();

        return new ProjectStatistics
        {
            TotalProjects = projects.Count,
            CompletedProjects = projects.Count(p => p.Status == ProjectStatus.Completed),
            ProcessingProjects = projects.Count(p => p.Status == ProjectStatus.Processing),
            FailedProjects = projects.Count(p => p.Status == ProjectStatus.Failed)
        };
    }
}

/// <summary>
/// 项目统计信息
/// </summary>
public class ProjectStatistics
{
    public int TotalProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int ProcessingProjects { get; set; }
    public int FailedProjects { get; set; }
}