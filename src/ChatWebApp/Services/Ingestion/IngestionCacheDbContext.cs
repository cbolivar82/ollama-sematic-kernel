using ChatWebApp.Services.Ingestion.Entities;

using Microsoft.EntityFrameworkCore;

namespace ChatWebApp.Services.Ingestion;

// A DbContext that keeps track of which documents have been ingested.
// This makes it possible to avoid re-ingesting documents that have not changed,
// and to delete documents that have been removed from the underlying source.
public class IngestionCacheDbContext : DbContext
{
    public IngestionCacheDbContext(DbContextOptions<IngestionCacheDbContext> options) : base(options)
    {
    }

    public DbSet<IngestedDocument> Documents { get; set; } = default!;
    public DbSet<IngestedRecord> Records { get; set; } = default!;

    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<IngestionCacheDbContext>();
        db.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IngestedDocument>().HasMany(d => d.Records).WithOne().HasForeignKey(r => r.DocumentId).OnDelete(DeleteBehavior.Cascade);
    }
}
