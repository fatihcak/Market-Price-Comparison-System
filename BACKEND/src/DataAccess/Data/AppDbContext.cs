using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace DataAccess.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProductCategory> ProductCategory { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<Market> Market { get; set; }
    public DbSet<MarketProductPrice> MarketProductPrice { get; set; }
    public DbSet<City> City { get; set; }
    public DbSet<District> District { get; set; }
    public DbSet<UserProductList> UserProductList { get; set; }
    public DbSet<AdminUser> AdminUser { get; set; }
    public DbSet<ProductPriceHistory> ProductPriceHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ProductCategory - DB Columns: CategoryID, CategoryName, ParentCategoryID
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("ProductCategory");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasColumnName("CategoryName").HasMaxLength(100);
            
            // Ignore columns not in database
            entity.Ignore(e => e.IsDeleted);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.Icon);
        });

        // Product - DB Columns: ProductID, CategoryID, ProductName, Brand, Unit, LastUpdated, ImageURL, MarketProductURL
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ProductName).HasColumnName("ProductName").HasMaxLength(200);
            entity.Property(e => e.Brand).HasColumnName("Brand").HasMaxLength(100);
            entity.Property(e => e.Unit).HasColumnName("Unit").HasMaxLength(50);
            entity.Property(e => e.LastUpdated).HasColumnName("LastUpdated");
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL").HasMaxLength(500);
            
            // Ignore columns not in database
            entity.Ignore(e => e.IsDeleted);
            entity.Ignore(e => e.CreatedAt);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.ProductName);
            entity.HasIndex(e => e.Brand);
        });

        // Market - DB Columns: MarketID, MarketName, WebsiteURL
        modelBuilder.Entity<Market>(entity =>
        {
            entity.ToTable("Market");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("MarketID");
            entity.Property(e => e.MarketName).HasColumnName("MarketName").HasMaxLength(200);
            entity.Property(e => e.Website).HasColumnName("WebsiteURL").HasMaxLength(500);
            
            // Ignore columns not in database
            entity.Ignore(e => e.IsDeleted);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.LogoUrl);

            // Indexes
            entity.HasIndex(e => e.MarketName);
        });

        // City - DB Columns: CityID, CityName
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("CityID");
            entity.Property(e => e.CityName).HasColumnName("CityName").HasMaxLength(100);
            
            // Ignore columns not in database
            entity.Ignore(e => e.IsDeleted);
            entity.Ignore(e => e.CreatedAt);
        });

        // District - DB Columns: DistrictID, CityID, DistrictName
        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("District");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("DistrictID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.DistrictName).HasColumnName("DistrictName").HasMaxLength(100);
            
            // Ignore columns not in database
            entity.Ignore(e => e.IsDeleted);
            entity.Ignore(e => e.CreatedAt);

            entity.HasOne(e => e.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.CityId);
            entity.HasIndex(e => e.DistrictName);
        });

        // MarketProductPrice - DB Columns: PriceID, MarketID, ProductID, DistrictID, Price, LastUpdated
        modelBuilder.Entity<MarketProductPrice>(entity =>
        {
            entity.ToTable("MarketProductPrice");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("PriceID");
            entity.Property(e => e.MarketId).HasColumnName("MarketID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.DistrictId).HasColumnName("DistrictID");
            entity.Property(e => e.Price).HasColumnName("Price").HasColumnType("decimal(18,2)");
            entity.Property(e => e.LastUpdated).HasColumnName("LastUpdated");
            
            // Ignore columns not in database
            entity.Ignore(e => e.IsDeleted);
            entity.Ignore(e => e.CreatedAt);

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

        // UserProductList - (Keep for future use, ignore DB fields)
        modelBuilder.Entity<UserProductList>(entity =>
        {
            entity.ToTable("UserProductList");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ListID");
            entity.Property(e => e.SessionId).HasColumnName("SessionID").HasMaxLength(255);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.Property(e => e.AddedDate).HasColumnName("AddedDate");
            
            // Ignore columns not in database
            entity.Ignore(e => e.IsDeleted);
            entity.Ignore(e => e.CreatedAt);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.UserProductLists)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.SessionId, e.ProductId }).IsUnique();
        });

        // NO SOFT DELETE FILTERS - Database doesn't have IsDeleted column

        // AdminUser (separate table, keep as is)
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.ToTable("AdminUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // ProductPriceHistory (keep for future use)
        modelBuilder.Entity<ProductPriceHistory>(entity =>
        {
            entity.ToTable("ProductPriceHistories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ChangedDate).HasColumnName("ChangedDate");

            entity.HasOne(e => e.MarketProductPrice)
                .WithMany()
                .HasForeignKey(e => e.MarketProductPriceId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.MarketProductPriceId);
            entity.HasIndex(e => e.ChangedDate);
        });
    }
}
