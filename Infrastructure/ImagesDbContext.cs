using dotnet_intermediate_mentoring_program.Images.Responses;
using Microsoft.EntityFrameworkCore;

namespace dotnet_intermediate_mentoring_program.Infrastructure;

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