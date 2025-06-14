using System.ComponentModel.DataAnnotations;

namespace OverALL.Data.Models;

/// <summary>
/// 处理步骤记录实体
/// </summary>
public class ProcessingStep
{
    [Key]
    [MaxLength(32)]
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// 步骤名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// 步骤描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 步骤类型
    /// </summary>
    public StepType Type { get; set; }

    /// <summary>
    /// 步骤状态
    /// </summary>
    public StepStatus Status { get; set; } = StepStatus.Pending;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 输入数据
    /// </summary>
    public string? Input { get; set; }

    /// <summary>
    /// 输出结果
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }    /// <summary>
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
/// 步骤类型枚举
/// </summary>
public enum StepType
{
    DocumentAnalysis = 1,    // 文献解析模块
    DocumentExtraction = 2,  // 文献获取模块
    ContentStructuring = 3,  // 内容结构化模块
    PagePlanning = 4,        // 页面规划模块
    ContentCollection = 5,   // 内容集合模块
    SlideGeneration = 6,     // 演示文稿生成模块
    NoteGeneration = 7       // 备注生成模块
}

/// <summary>
/// 步骤状态枚举
/// </summary>
public enum StepStatus
{
    Pending = 0,        // 等待中
    Running = 1,        // 运行中
    Completed = 2,      // 已完成
    Failed = 3,         // 失败
    Skipped = 4         // 跳过
}