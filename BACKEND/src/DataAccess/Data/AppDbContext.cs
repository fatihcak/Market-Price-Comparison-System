using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace DataAccess.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Market> Markets { get; set; }
    public DbSet<MarketProductPrice> MarketProductPrices { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<UserProductList> UserProductLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ProductCategory
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("ProductCategories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasColumnName("CategoryName").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
        });

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ProductName).HasColumnName("ProductName").HasMaxLength(200);
            entity.Property(e => e.Brand).HasColumnName("Brand").HasMaxLength(100);
            entity.Property(e => e.Unit).HasColumnName("Unit").HasMaxLength(50);
            entity.Property(e => e.LastUpdated).HasColumnName("LastUpdated");
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.ProductName);
            entity.HasIndex(e => e.Brand);
        });

        // Market
        modelBuilder.Entity<Market>(entity =>
        {
            entity.ToTable("Markets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("MarketID");
            entity.Property(e => e.MarketName).HasColumnName("MarketName").HasMaxLength(200);
            entity.Property(e => e.LogoUrl).HasColumnName("LogoURL").HasMaxLength(500);
            entity.Property(e => e.Website).HasColumnName("WebsiteURL").HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

            // Indexes
            entity.HasIndex(e => e.MarketName);
        });

        // City
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("Cities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("CityID");
            entity.Property(e => e.CityName).HasColumnName("CityName").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
        });

        // District
        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("Districts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("DistrictID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.DistrictName).HasColumnName("DistrictName").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

            entity.HasOne(e => e.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.CityId);
            entity.HasIndex(e => e.DistrictName);
        });

        // MarketProductPrice
        modelBuilder.Entity<MarketProductPrice>(entity =>
        {
            entity.ToTable("MarketProductPrices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("PriceID");
            entity.Property(e => e.MarketId).HasColumnName("MarketID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.DistrictId).HasColumnName("DistrictID");
            entity.Property(e => e.Price).HasColumnName("Price").HasColumnType("decimal(18,2)");
            entity.Property(e => e.LastUpdated).HasColumnName("LastUpdated");
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

            entity.HasOne(e => e.Market)
                .WithMany(m => m.MarketProductPrices)
                .HasForeignKey(e => e.MarketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.MarketProductPrices)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.District)
                .WithMany(d => d.MarketProductPrices)
                .HasForeignKey(e => e.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            entity.HasIndex(e => e.MarketId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.DistrictId);
            entity.HasIndex(e => new { e.ProductId, e.DistrictId });
            entity.HasIndex(e => e.LastUpdated);
        });

        // UserProductList
        modelBuilder.Entity<UserProductList>(entity =>
        {
            entity.ToTable("UserProductLists");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ListID");
            entity.Property(e => e.SessionId).HasColumnName("SessionID").HasMaxLength(255);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.Property(e => e.AddedDate).HasColumnName("AddedDate");
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

            entity.HasOne(e => e.Product)
                .WithMany(p => p.UserProductLists)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.SessionId, e.ProductId }).IsUnique();
        });

        // Global query filters for soft delete
        modelBuilder.Entity<ProductCategory>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Market>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MarketProductPrice>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<City>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<District>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserProductList>().HasQueryFilter(e => !e.IsDeleted);
    }
}
