using System.ComponentModel.DataAnnotations;
using OverALL.Data;

namespace OverALL.Data.Models;

/// <summary>
/// PDF项目实体 - 用户创建的处理项目
/// </summary>
public class PdfProject
{
    [Key]
    [MaxLength(32)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 项目存储文件夹路径
    /// </summary>
    [Required]
    public string ProjectFolder { get; set; } = string.Empty;

    /// <summary>
    /// 项目状态
    /// </summary>
    public ProjectStatus Status { get; set; } = ProjectStatus.Created;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 所属用户ID
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 所属用户
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// 项目包含的PDF文档
    /// </summary>
    public ICollection<PdfDocument> Documents { get; set; } = new List<PdfDocument>();

    /// <summary>
    /// 处理步骤记录
    /// </summary>
    public ICollection<ProcessingStep> ProcessingSteps { get; set; } = new List<ProcessingStep>();

    /// <summary>
    /// 生成的PPT文件
    /// </summary>
    public ICollection<GeneratedPpt> GeneratedPpts { get; set; } = new List<GeneratedPpt>();
}

/// <summary>
/// 项目状态枚举
/// </summary>
public enum ProjectStatus
{
    Created = 0,        // 已创建
    Processing = 1,     // 处理中
    Completed = 2,      // 已完成
    Failed = 3,         // 处理失败
    Cancelled = 4       // 已取消
}