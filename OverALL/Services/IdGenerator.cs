using System.Security.Cryptography;
using System.Text;

namespace OverALL.Services;

/// <summary>
/// ID生成器 - 用于生成唯一的哈希ID
/// </summary>
public static class IdGenerator
{
    /// <summary>
    /// 生成项目ID - 基于时间戳、用户ID和随机数的哈希
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="projectName">项目名称</param>
    /// <returns>32位哈希字符串</returns>
    public static string GenerateProjectId(string userId, string projectName)
    {
        // 组合用于生成哈希的字符串
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var randomValue = Guid.NewGuid().ToString("N");
        var combinedString = $"{userId}_{projectName}_{timestamp}_{randomValue}";
        
        // 使用MD5生成哈希（32位）
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// 生成文档ID - 基于项目ID、文件名和时间戳的哈希
    /// </summary>
    /// <param name="projectId">项目ID</param>
    /// <param name="fileName">文件名</param>
    /// <returns>32位哈希字符串</returns>
    public static string GenerateDocumentId(string projectId, string fileName)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var randomValue = Guid.NewGuid().ToString("N");
        var combinedString = $"{projectId}_{fileName}_{timestamp}_{randomValue}";
        
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// 生成处理步骤ID
    /// </summary>
    /// <param name="projectId">项目ID</param>
    /// <param name="stepName">步骤名称</param>
    /// <returns>32位哈希字符串</returns>
    public static string GenerateStepId(string projectId, string stepName)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var randomValue = Guid.NewGuid().ToString("N");
        var combinedString = $"{projectId}_{stepName}_{timestamp}_{randomValue}";
        
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// 生成PPT文件ID
    /// </summary>
    /// <param name="projectId">项目ID</param>
    /// <param name="fileName">文件名</param>
    /// <returns>32位哈希字符串</returns>
    public static string GeneratePptId(string projectId, string fileName)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var randomValue = Guid.NewGuid().ToString("N");
        var combinedString = $"{projectId}_{fileName}_{timestamp}_{randomValue}";
        
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
