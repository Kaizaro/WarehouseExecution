using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WarehouseExecution.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobStep> JobSteps => Set<JobStep>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureJob(modelBuilder);
        ConfigureJobStep(modelBuilder);
    }

    private static void ConfigureJob(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("Jobs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.JobNumber)
                .IsRequired()
                .HasMaxLength(64);

            entity.HasIndex(x => x.JobNumber)
                .IsUnique();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            entity.Property(x => x.ProductCode)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(x => x.ProductName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.ToLocation)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(x => x.FromLocation)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
        });
    }

    private static void ConfigureJobStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobStep>(entity =>
        {
            entity.ToTable("JobSteps");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.StepNumber).IsRequired();

            entity.HasIndex(x => new { x.JobId, x.StepNumber })
                .IsUnique();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            entity.Property(x => x.ToLocation)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(x => x.FromLocation)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(x => x.CreatedAtUtc).IsRequired();
        });
    }

    private void ApplyAuditDates()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Job>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.UpdatedAtUtc = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<JobStep>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
            }
        }
    }
}
