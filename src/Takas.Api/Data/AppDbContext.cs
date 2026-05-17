using Microsoft.EntityFrameworkCore;
using Takas.Api.Entities;
using Takas.Api.Enums;

namespace Takas.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<DeliveryVerification> DeliveryVerifications => Set<DeliveryVerification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Role).HasConversion<string>();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(x => x.OwnerId);
            entity.HasIndex(x => x.CategoryId);
            entity.HasIndex(x => x.Status);
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Condition).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Campus).HasMaxLength(100).IsRequired();
            entity.Property(x => x.EstimatedMinPrice).HasColumnType("numeric(12,2)");
            entity.Property(x => x.EstimatedMaxPrice).HasColumnType("numeric(12,2)");
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.Owner)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasIndex(x => new { x.ProductId, x.IsMain });
            entity.Property(x => x.ImageUrl).HasMaxLength(1000).IsRequired();
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasIndex(x => new { x.ProductId, x.SenderId, x.Status });
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.CashAmount).HasColumnType("numeric(12,2)");
            entity.Property(x => x.Message).HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.Product)
                .WithMany(x => x.Offers)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Sender)
                .WithMany(x => x.SentOffers)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.OfferedProduct)
                .WithMany()
                .HasForeignKey(x => x.OfferedProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasIndex(x => new { x.ReceiverId, x.IsRead });
            entity.HasIndex(x => new { x.SenderId, x.ReceiverId, x.CreatedAt });
            entity.Property(x => x.Content).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.Sender)
                .WithMany(x => x.SentMessages)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Receiver)
                .WithMany(x => x.ReceivedMessages)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Product)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasIndex(x => new { x.UserId, x.CreatedAt });
            entity.HasIndex(x => new { x.Status, x.CreatedAt });
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.Subject).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.AdminReply).HasMaxLength(4000);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(x => x.RepliedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.User)
                .WithMany(x => x.ContactMessages)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(x => new { x.UserId, x.IsRead });
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasIndex(x => x.Status);
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.Reporter)
                .WithMany(x => x.CreatedReports)
                .HasForeignKey(x => x.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ReportedUser)
                .WithMany(x => x.ReportsAboutUser)
                .HasForeignKey(x => x.ReportedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Product)
                .WithMany(x => x.Reports)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DeliveryVerification>(entity =>
        {
            entity.HasIndex(x => x.OfferId).IsUnique();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(x => x.ExpiresAt).HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.Offer)
                .WithOne(x => x.DeliveryVerification)
                .HasForeignKey<DeliveryVerification>(x => x.OfferId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
