using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OverALL.Models;

namespace OverALL.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<PdfProject> PdfProjects { get; set; }
    public DbSet<PdfDocument> PdfDocuments { get; set; }
    public DbSet<ProcessingStep> ProcessingSteps { get; set; }
    public DbSet<GeneratedPpt> GeneratedPpts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置PDF项目实体
        modelBuilder.Entity<PdfProject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().HasMaxLength(32);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProjectFolder).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UserId).IsRequired();

            // 配置与用户的关系
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // 配置索引
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // 配置PDF文档实体
        modelBuilder.Entity<PdfDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().HasMaxLength(32);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ProjectId).IsRequired().HasMaxLength(32);

            // 配置与项目的关系
            entity.HasOne(e => e.Project)
                  .WithMany(e => e.Documents)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProjectId);
        });

        // 配置处理步骤实体
        modelBuilder.Entity<ProcessingStep>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().HasMaxLength(32);
            entity.Property(e => e.StepName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProjectId).IsRequired().HasMaxLength(32);

            // 配置与项目的关系
            entity.HasOne(e => e.Project)
                  .WithMany(e => e.ProcessingSteps)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.StartedAt);
        });

        // 配置生成PPT实体
        modelBuilder.Entity<GeneratedPpt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().HasMaxLength(32);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ProjectId).IsRequired().HasMaxLength(32);

            // 配置与项目的关系
            entity.HasOne(e => e.Project)
                  .WithMany(e => e.GeneratedPpts)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProjectId);
        });
    }
}
