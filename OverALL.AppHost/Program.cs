var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OverALL>("overall");

builder.Build().Run();
