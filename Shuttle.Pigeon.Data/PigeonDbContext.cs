using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.Data;

public class PigeonDbContext : DbContext
{
    public PigeonDbContext(IOptions<PigeonDataOptions> pigeonDataOptions, DbContextOptions<PigeonDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Message> Messages { get; set; } = null!;
    public DbSet<Models.MessageAttachment> MessageAttachments { get; set; } = null!;
    public DbSet<Models.MessageRecipient> MessageRecipients { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pigeon");

        modelBuilder.Entity<Models.MessageAttachment>()
            .HasOne(p => p.Message)
            .WithMany(b => b.Attachments)
            .HasForeignKey(p => p.MessageId) 
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.MessageRecipient>()
            .HasOne(p => p.Message)
            .WithMany(b => b.Recipients)
            .HasForeignKey(p => p.MessageId) 
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
    }
}