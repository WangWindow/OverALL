using System;
using OverALL.Services;

Console.WriteLine("测试哈希ID生成");
Console.WriteLine("================");

// 测试项目ID生成
var userId = "user123";
var projectName1 = "测试项目1";
var projectName2 = "测试项目2";

var projectId1 = IdGenerator.GenerateProjectId(userId, projectName1);
var projectId2 = IdGenerator.GenerateProjectId(userId, projectName2);
var projectId3 = IdGenerator.GenerateProjectId(userId, projectName1); // 相同名称，但应该生成不同ID

Console.WriteLine($"项目ID 1: {projectId1}");
Console.WriteLine($"项目ID 2: {projectId2}");
Console.WriteLine($"项目ID 3 (相同名称): {projectId3}");
Console.WriteLine();

// 验证ID长度
Console.WriteLine($"ID长度: {projectId1.Length} 字符");
Console.WriteLine($"是否为32字符: {projectId1.Length == 32}");
Console.WriteLine();

// 测试文档ID生成
var documentId1 = IdGenerator.GenerateDocumentId(projectId1, "document1.pdf");
var documentId2 = IdGenerator.GenerateDocumentId(projectId1, "document2.pdf");

Console.WriteLine($"文档ID 1: {documentId1}");
Console.WriteLine($"文档ID 2: {documentId2}");
Console.WriteLine();

// 测试处理步骤ID生成
var stepId1 = IdGenerator.GenerateStepId(projectId1, "文档解析");
var stepId2 = IdGenerator.GenerateStepId(projectId1, "文献提取");

Console.WriteLine($"步骤ID 1: {stepId1}");
Console.WriteLine($"步骤ID 2: {stepId2}");
Console.WriteLine();

// 验证唯一性
Console.WriteLine($"项目ID是否唯一: {projectId1 != projectId2 && projectId1 != projectId3 && projectId2 != projectId3}");
Console.WriteLine($"文档ID是否唯一: {documentId1 != documentId2}");
Console.WriteLine($"步骤ID是否唯一: {stepId1 != stepId2}");
