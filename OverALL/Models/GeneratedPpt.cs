using System.ComponentModel.DataAnnotations;

namespace OverALL.Models;

/// <summary>
/// 生成的PPT文件实体
/// </summary>
public class GeneratedPpt
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
    /// 生成时间
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// PPT类型
    /// </summary>
    public PptType Type { get; set; } = PptType.Standard;

    /// <summary>
    /// 包含的幻灯片数量
    /// </summary>
    public int SlideCount { get; set; }

    /// <summary>
    /// 是否包含备注
    /// </summary>
    public bool HasNotes { get; set; }

    /// <summary>
    /// 生成配置（JSON格式）
    /// </summary>
    public string? GenerationConfig { get; set; }    /// <summary>
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
/// PPT类型枚举
/// </summary>
public enum PptType
{
    Standard = 0,       // 标准PPT
    WithNotes = 1,      // 带备注的PPT
    Summary = 2,        // 摘要PPT
    Detailed = 3        // 详细PPT
}