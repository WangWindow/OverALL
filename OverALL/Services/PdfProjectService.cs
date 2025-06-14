using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OverALL.Data;
using OverALL.Models;

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
        // 生成哈希项目ID
        var projectId = IdGenerator.GenerateProjectId(userId, name);
        // 使用相对路径存储在数据库中
        var projectFolder = Path.Combine("ProjectFiles", projectId);

        // 创建实际的文件夹（使用绝对路径）
        var actualProjectFolder = Path.Combine(_environment.ContentRootPath, projectFolder);
        Directory.CreateDirectory(actualProjectFolder);

        var project = new PdfProject
        {
            Id = projectId,
            Name = name,
            Description = description,
            ProjectFolder = projectFolder, // 存储相对路径
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
    public async Task<PdfProject?> GetProjectByIdAsync(string projectId, string userId)
    {
        return await _context.PdfProjects
            .Where(p => p.Id == projectId && p.UserId == userId)
            .Include(p => p.Documents)
            .Include(p => p.ProcessingSteps.OrderBy(s => s.StartedAt))
            .Include(p => p.GeneratedPpts)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 更新项目基本信息
    /// </summary>
    public async Task<bool> UpdateProjectAsync(string projectId, string name, string? description, string userId)
    {
        var project = await _context.PdfProjects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
            return false;

        project.Name = name;
        project.Description = description;
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated project {ProjectId} basic information", projectId);
        return true;
    }

    /// <summary>
    /// 更新项目状态
    /// </summary>
    public async Task<bool> UpdateProjectStatusAsync(string projectId, ProjectStatus status, string userId)
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
    public async Task<bool> DeleteProjectAsync(string projectId, string userId)
    {
        var project = await _context.PdfProjects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
            return false;

        // 删除项目文件夹
        try
        {
            var absolutePath = GetProjectAbsolutePath(project);
            if (Directory.Exists(absolutePath))
            {
                Directory.Delete(absolutePath, true);
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

    /// <summary>
    /// 获取项目的绝对路径
    /// </summary>
    /// <param name="project">项目实体</param>
    /// <returns>项目的绝对路径</returns>
    public string GetProjectAbsolutePath(PdfProject project)
    {
        return Path.Combine(_environment.ContentRootPath, project.ProjectFolder);
    }

    /// <summary>
    /// 获取项目的绝对路径
    /// </summary>
    /// <param name="relativePath">相对路径</param>
    /// <returns>绝对路径</returns>
    public string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(_environment.ContentRootPath, relativePath);
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