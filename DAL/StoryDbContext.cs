using Microsoft.EntityFrameworkCore;
using StoryGame.Models;

namespace StoryGame.DAL;

public class StoryDbContext : DbContext
{
    public StoryDbContext(DbContextOptions<StoryDbContext> options)
        : base(options)
    {
        //Database.EnsureCreated();
    }

    public DbSet<Story> Stories { get; set; }
    public DbSet<Scene> Scenes { get; set; }
    public DbSet<Choice> Choices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // A Scene has many Choices
        modelBuilder
            .Entity<Scene>()
            .HasMany(s => s.ChoiceList)
            .WithOne(c => c.ThisScene)
            .HasForeignKey(c => c.ThisSceneId)
            .OnDelete(DeleteBehavior.Cascade);

        // A Choice optionally leads to another Scene
        modelBuilder
            .Entity<Choice>()
            .HasOne(c => c.NextScene)
            .WithMany()
            .HasForeignKey(c => c.NextSceneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
