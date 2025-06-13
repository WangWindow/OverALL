using Microsoft.EntityFrameworkCore;
using OverALL.Data;
using OverALL.Data.Models;

namespace OverALL.Services;

/// <summary>
/// PDF文档服务 - 管理PDF文档的存储、检索和操作
/// </summary>
public class PdfDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PdfDocumentService> _logger;

    public PdfDocumentService(
        ApplicationDbContext context,
        ILogger<PdfDocumentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取项目的所有文档
    /// </summary>
    public async Task<List<PdfDocument>> GetProjectDocumentsAsync(int projectId)
    {
        return await _context.PdfDocuments
            .Where(d => d.ProjectId == projectId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 根据ID获取文档
    /// </summary>
    public async Task<PdfDocument?> GetDocumentByIdAsync(int documentId)
    {
        return await _context.PdfDocuments
            .Include(d => d.Project)
            .FirstOrDefaultAsync(d => d.Id == documentId);
    }

    /// <summary>
    /// 更新文档状态
    /// </summary>
    public async Task<bool> UpdateDocumentStatusAsync(int documentId, DocumentStatus status)
    {
        var document = await _context.PdfDocuments.FindAsync(documentId);
        if (document == null)
            return false;

        document.Status = status;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated document {DocumentId} status to {Status}", documentId, status);
        return true;
    }

    /// <summary>
    /// 更新文档的结构化内容
    /// </summary>
    public async Task<bool> UpdateDocumentContentAsync(int documentId, string? structuredContent, string? references)
    {
        var document = await _context.PdfDocuments.FindAsync(documentId);
        if (document == null)
            return false;

        document.StructuredContent = structuredContent;
        document.References = references;
        document.Status = DocumentStatus.Analyzed;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated document {DocumentId} content", documentId);
        return true;
    }

    /// <summary>
    /// 删除文档
    /// </summary>
    public async Task<bool> DeleteDocumentAsync(int documentId)
    {
        var document = await _context.PdfDocuments.FindAsync(documentId);
        if (document == null)
            return false;

        // 删除文件
        try
        {
            if (File.Exists(document.FilePath))
            {
                File.Delete(document.FilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete document file {FilePath}", document.FilePath);
        }

        // 删除数据库记录
        _context.PdfDocuments.Remove(document);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted document {DocumentId}", documentId);
        return true;
    }

    /// <summary>
    /// 获取文档统计信息
    /// </summary>
    public async Task<DocumentStatistics> GetDocumentStatisticsAsync(int projectId)
    {
        var documents = await _context.PdfDocuments
            .Where(d => d.ProjectId == projectId)
            .ToListAsync();

        return new DocumentStatistics
        {
            TotalDocuments = documents.Count,
            UploadedDocuments = documents.Count(d => d.Status == DocumentStatus.Uploaded),
            AnalyzingDocuments = documents.Count(d => d.Status == DocumentStatus.Analyzing),
            AnalyzedDocuments = documents.Count(d => d.Status == DocumentStatus.Analyzed),
            FailedDocuments = documents.Count(d => d.Status == DocumentStatus.Failed),
            TotalSize = documents.Sum(d => d.FileSize)
        };
    }
}

/// <summary>
/// 文档统计信息
/// </summary>
public class DocumentStatistics
{
    public int TotalDocuments { get; set; }
    public int UploadedDocuments { get; set; }
    public int AnalyzingDocuments { get; set; }
    public int AnalyzedDocuments { get; set; }
    public int FailedDocuments { get; set; }
    public long TotalSize { get; set; }
}