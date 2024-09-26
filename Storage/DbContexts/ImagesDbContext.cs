using Microsoft.EntityFrameworkCore;
using Storage.DbModels;

namespace Storage.DbContexts;

public class ImagesDbContext(DbContextOptions<ImagesDbContext> options) : DbContext(options)
{
    public DbSet<ImageMetadata> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageMetadata>(
            entity => entity.HasKey(i => i.Name)
        );
    }
}