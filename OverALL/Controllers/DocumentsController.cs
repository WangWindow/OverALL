using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverALL.Services;
using System.Security.Claims;

namespace OverALL.Controllers;

/// <summary>
/// 文档下载控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly PdfDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        PdfDocumentService documentService,
        ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    /// <summary>
    /// 下载文档
    /// </summary>
    /// <param name="documentId">文档ID</param>
    /// <returns>文档文件</returns>
    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> DownloadDocument(string documentId)
    {
        try
        {
            var downloadInfo = await _documentService.GetDocumentDownloadInfoAsync(documentId);
            if (downloadInfo == null)
            {
                return NotFound("文档不存在或文件已被删除");
            }

            var (filePath, fileName, contentType) = downloadInfo.Value;

            // 验证用户权限（确保用户只能下载自己的文档）
            var document = await _documentService.GetDocumentByIdAsync(documentId);
            if (document?.Project?.UserId != GetCurrentUserId())
            {
                return Forbid("您没有权限访问此文档");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocumentId}", documentId);
            return StatusCode(500, "下载文档时发生错误");
        }
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    /// <returns>用户ID</returns>
    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }
}
