using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OverALL.Data;
using OverALL.Data.Models;

namespace OverALL.Services;

/// <summary>
/// PDF处理服务 - 处理PDF文档上传、分析和PPT生成流程
/// </summary>
public class PdfProcessingService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PdfProcessingService> _logger;
    private readonly PdfProjectService _projectService;

    public PdfProcessingService(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<PdfProcessingService> logger,
        PdfProjectService projectService)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
        _projectService = projectService;
    }    /// <summary>
         /// 上传PDF文档到项目
         /// </summary>
    public async Task<PdfDocument> UploadPdfAsync(string projectId, string userId, IFormFile pdfFile)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new ArgumentException("项目不存在或无权限访问");

        // 验证文件类型
        if (!pdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
            !pdfFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("只支持PDF文件格式");

        // 验证文件大小（限制为50MB）
        const long maxFileSize = 50 * 1024 * 1024; // 50MB
        if (pdfFile.Length > maxFileSize)
            throw new ArgumentException("文件大小不能超过50MB");

        if (pdfFile.Length == 0)
            throw new ArgumentException("文件不能为空");

        // 生成文件路径
        var fileName = Path.GetFileNameWithoutExtension(pdfFile.FileName);
        var fileExtension = Path.GetExtension(pdfFile.FileName);
        var uniqueFileName = $"{fileName}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";

        // 使用相对路径存储在数据库中
        var relativeFilePath = Path.Combine(project.ProjectFolder, "Documents", uniqueFileName);
        var absoluteFilePath = _projectService.GetAbsolutePath(relativeFilePath);

        // 确保目录存在
        var directoryPath = Path.GetDirectoryName(absoluteFilePath);
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 保存文件
        try
        {
            using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file {FileName} to {FilePath}", pdfFile.FileName, absoluteFilePath);
            throw new InvalidOperationException($"保存文件失败: {ex.Message}");
        }

        // 保存到数据库（存储相对路径）
        var document = new PdfDocument
        {
            Id = IdGenerator.GenerateDocumentId(projectId, pdfFile.FileName),
            FileName = pdfFile.FileName,
            FilePath = relativeFilePath, // 存储相对路径
            FileSize = pdfFile.Length,
            ProjectId = projectId,
            UploadedAt = DateTime.UtcNow,
            Status = DocumentStatus.Uploaded
        };

        try
        {
            _context.PdfDocuments.Add(document);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // 如果数据库保存失败，尝试删除已上传的文件
            try
            {
                if (File.Exists(absoluteFilePath))
                {
                    File.Delete(absoluteFilePath);
                }
            }
            catch (Exception deleteEx)
            {
                _logger.LogWarning(deleteEx, "Failed to cleanup file {FilePath} after database save failure", absoluteFilePath);
            }

            _logger.LogError(ex, "Failed to save document {FileName} to database", pdfFile.FileName);
            throw new InvalidOperationException($"保存文档信息到数据库失败: {ex.Message}");
        }

        _logger.LogInformation("Uploaded PDF {FileName} to project {ProjectId}", pdfFile.FileName, projectId);
        return document;
    }/// <summary>
     /// 开始处理PDF文档（调用Python脚本或C#实现）
     /// </summary>
    public async Task<bool> StartPdfProcessingAsync(string projectId, string userId)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            return false;

        if (project.Status == ProjectStatus.Processing)
            return false; // 已经在处理中

        // 更新项目状态
        await _projectService.UpdateProjectStatusAsync(projectId, ProjectStatus.Processing, userId);

        try
        {
            // 记录处理开始
            await RecordProcessingStepAsync(projectId, "开始处理", "开始PDF文档处理流程");

            // 步骤1: 文献解析模块
            await ProcessDocumentAnalysisAsync(projectId);

            // 步骤2: 文献获取模块
            await ProcessReferenceExtractionAsync(projectId);

            // 步骤7: 备注生成模块（如果Python同事已完成）
            await ProcessNotesGenerationAsync(projectId);

            // 更新项目状态为完成
            await _projectService.UpdateProjectStatusAsync(projectId, ProjectStatus.Completed, userId);
            await RecordProcessingStepAsync(projectId, "处理完成", "所有处理步骤已完成");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Processing failed for project {ProjectId}", projectId);
            await _projectService.UpdateProjectStatusAsync(projectId, ProjectStatus.Failed, userId);
            await RecordProcessingStepAsync(projectId, "处理失败", $"处理过程中发生错误: {ex.Message}");
            return false;
        }
    }    /// <summary>
         /// 步骤1: 文献解析模块
         /// </summary>
    private async Task ProcessDocumentAnalysisAsync(string projectId)
    {
        await RecordProcessingStepAsync(projectId, "文献解析", "开始解析PDF文档结构");

        // TODO: 调用Python脚本或实现C#版本
        // 这里可以调用Python同事的实现

        await Task.Delay(2000); // 模拟处理时间
        await RecordProcessingStepAsync(projectId, "文献解析", "PDF文档解析完成，生成结构化内容");
    }

    /// <summary>
    /// 步骤2: 文献获取模块
    /// </summary>
    private async Task ProcessReferenceExtractionAsync(string projectId)
    {
        await RecordProcessingStepAsync(projectId, "文献获取", "开始提取参考文献");

        // TODO: 调用Python脚本或实现C#版本

        await Task.Delay(1500); // 模拟处理时间
        await RecordProcessingStepAsync(projectId, "文献获取", "参考文献提取完成");
    }

    /// <summary>
    /// 步骤7: 备注生成模块
    /// </summary>
    private async Task ProcessNotesGenerationAsync(string projectId)
    {
        await RecordProcessingStepAsync(projectId, "备注生成", "开始生成演示文稿备注");

        // TODO: 调用Python同事的实现

        await Task.Delay(3000); // 模拟处理时间
        await RecordProcessingStepAsync(projectId, "备注生成", "PPT备注生成完成");
    }

    /// <summary>
    /// 调用Python处理脚本
    /// </summary>
    private async Task<string> CallPythonScriptAsync(string scriptPath, string arguments)
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{scriptPath} {arguments}",
                WorkingDirectory = _environment.ContentRootPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
                throw new InvalidOperationException("无法启动Python进程");

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Python脚本执行失败: {error}");

            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute Python script {ScriptPath}", scriptPath);
            throw;
        }
    }    /// <summary>
         /// 记录处理步骤
         /// </summary>
    private async Task RecordProcessingStepAsync(string projectId, string stepName, string description)
    {
        var step = new ProcessingStep
        {
            Id = IdGenerator.GenerateStepId(projectId, stepName),
            ProjectId = projectId,
            StepName = stepName,
            Description = description,
            StartedAt = DateTime.UtcNow,
            Status = StepStatus.Completed
        };

        _context.ProcessingSteps.Add(step);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Recorded processing step {StepName} for project {ProjectId}", stepName, projectId);
    }    /// <summary>
         /// 获取项目的处理进度
         /// </summary>
    public async Task<ProcessingProgress> GetProcessingProgressAsync(string projectId, string userId)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new ArgumentException("项目不存在或无权限访问");

        var steps = await _context.ProcessingSteps
            .Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.StartedAt)
            .ToListAsync();

        return new ProcessingProgress
        {
            ProjectStatus = project.Status,
            ProcessingSteps = steps,
            CurrentStep = steps.LastOrDefault()?.StepName ?? "未开始",
            Progress = CalculateProgress(project.Status, steps.Count)
        };
    }

    /// <summary>
    /// 计算处理进度百分比
    /// </summary>
    private int CalculateProgress(ProjectStatus status, int completedSteps)
    {
        return status switch
        {
            ProjectStatus.Created => 0,
            ProjectStatus.Processing => Math.Min(completedSteps * 20, 80), // 每个步骤20%，最多80%
            ProjectStatus.Completed => 100,
            ProjectStatus.Failed => 0,
            ProjectStatus.Cancelled => 0,
            _ => 0
        };
    }
}

/// <summary>
/// 处理进度信息
/// </summary>
public class ProcessingProgress
{
    public ProjectStatus ProjectStatus { get; set; }
    public List<ProcessingStep> ProcessingSteps { get; set; } = new();
    public string CurrentStep { get; set; } = string.Empty;
    public int Progress { get; set; }
}