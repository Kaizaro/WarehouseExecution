using Microsoft.EntityFrameworkCore;
using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobStep> JobSteps => Set<JobStep>();
    internal DbSet<JobNumberCounter> JobNumberCounters => Set<JobNumberCounter>();

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

        ConfigureLocation(modelBuilder);
        ConfigureJob(modelBuilder);
        ConfigureJobStep(modelBuilder);
        ConfigureJobNumberCounter(modelBuilder);
    }

    private static void ConfigureLocation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(64);

            entity.HasIndex(x => x.Code)
                .IsUnique();

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();

            entity.HasData(
                new Location
                {
                    Id = new Guid("f0b57672-6ec3-45d6-bf6b-5f6d07ba2ac1"),
                    Code = "A-01",
                    Name = "Inbound Buffer A-01",
                    CreatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = new Guid("a6de81a2-827a-4ef8-ab80-69a304a4c613"),
                    Code = "A-02",
                    Name = "Inbound Buffer A-02",
                    CreatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = new Guid("4b250f3b-6537-4ca8-872f-fb702db9d712"),
                    Code = "B-01",
                    Name = "Storage Lane B-01",
                    CreatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = new Guid("77bfa0d1-517e-48ec-8846-5646041ebf66"),
                    Code = "B-02",
                    Name = "Storage Lane B-02",
                    CreatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = new Guid("97ccb9f8-525f-43b8-80d7-04727a2adfe4"),
                    Code = "P-01",
                    Name = "Packing Station P-01",
                    CreatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAtUtc = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc)
                });
        });
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
                .HasMaxLength(128);

            entity.Property(x => x.ProductName)
                .HasMaxLength(256);

            entity.Property(x => x.ToLocationId).IsRequired();
            entity.Property(x => x.FromLocationId).IsRequired();

            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();

            entity.HasMany(x => x.Steps)
                .WithOne(x => x.Job)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Location>()
                .WithMany()
                .HasForeignKey(x => x.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Location>()
                .WithMany()
                .HasForeignKey(x => x.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureJobStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobStep>(entity =>
        {
            entity.ToTable("JobSteps");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.JobId).IsRequired();
            entity.Property(x => x.StepNumber).IsRequired();

            entity.HasIndex(x => new { x.JobId, x.StepNumber })
                .IsUnique();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            entity.Property(x => x.ToLocationId).IsRequired();
            entity.Property(x => x.FromLocationId).IsRequired();

            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();

            entity.HasOne<Location>()
                .WithMany()
                .HasForeignKey(x => x.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Location>()
                .WithMany()
                .HasForeignKey(x => x.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureJobNumberCounter(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobNumberCounter>(entity =>
        {
            entity.ToTable("JobNumberCounters");

            entity.HasKey(x => x.Date);

            entity.Property(x => x.Date)
                .HasColumnType("date");

            entity.Property(x => x.LastValue)
                .IsRequired();
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
                entry.Property(x => x.CreatedAtUtc).IsModified = false;
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<JobStep>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.UpdatedAtUtc = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.CreatedAtUtc).IsModified = false;
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<Location>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.UpdatedAtUtc = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.CreatedAtUtc).IsModified = false;
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }
    }
}
