using System.ComponentModel.DataAnnotations;

namespace OverALL.Data.Models;

/// <summary>
/// PDF文档实体
/// </summary>
public class PdfDocument
{
    [Key]
    [MaxLength(32)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 上传时间
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 文档状态
    /// </summary>
    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;

    /// <summary>
    /// 结构化内容（文献解析结果）
    /// </summary>
    public string? StructuredContent { get; set; }

    /// <summary>
    /// 参考文献列表
    /// </summary>
    public string? References { get; set; }    /// <summary>
    /// 所属项目ID
    /// </summary>
    [MaxLength(32)]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// 所属项目
    /// </summary>
    public PdfProject Project { get; set; } = null!;
}

/// <summary>
/// 文档状态枚举
/// </summary>
public enum DocumentStatus
{
    Uploaded = 0,       // 已上传
    Analyzing = 1,      // 分析中
    Analyzed = 2,       // 已分析
    Failed = 3          // 分析失败
}