using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace DataAccess.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserAddress> UsersAddresses { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Market> Markets { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("USERID");
            entity.Property(e => e.Username).HasColumnName("USERNAME").HasMaxLength(100);
            entity.Property(e => e.Surname).HasColumnName("SURNAME").HasMaxLength(100);
            entity.Property(e => e.Email).HasColumnName("EMAIL").HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasColumnName("PASSWORDHASH");
            entity.Property(e => e.UserRole).HasColumnName("USERROLE").HasMaxLength(50);
            entity.Property(e => e.IsDeleted).HasColumnName("ISDELETED");
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // UserAddress
        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.ToTable("UsersAdresses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ADRESSID");
            entity.Property(e => e.UserId).HasColumnName("USERID");
            entity.Property(e => e.Address).HasColumnName("ADRESS").HasMaxLength(500);
            entity.Property(e => e.City).HasColumnName("CITY").HasMaxLength(100);
            entity.Property(e => e.District).HasColumnName("DISTRICT").HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasColumnName("POSTALCODE").HasMaxLength(20);
            entity.Property(e => e.Country).HasColumnName("COUNTRY").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("ISDELETED");
            entity.HasOne(e => e.User).WithMany(u => u.Addresses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("PRODUCTID");
            entity.Property(e => e.ProductName).HasColumnName("PRODUCTNAME").HasMaxLength(200);
            entity.Property(e => e.CategoryId).HasColumnName("CATEGORYID");
            entity.Property(e => e.ImageUrl).HasColumnName("IMAGEURL").HasMaxLength(500);
            entity.Property(e => e.Unit).HasColumnName("UNIT").HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnName("CREATEDAT");
            entity.HasOne(e => e.Category).WithMany(c => c.Products).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        // Market
        modelBuilder.Entity<Market>(entity =>
        {
            entity.ToTable("Markets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("MARKETID");
            entity.Property(e => e.MarketName).HasColumnName("MARKETNAME").HasMaxLength(200);
            entity.Property(e => e.LogoUrl).HasColumnName("LOGOURL").HasMaxLength(500);
            entity.Property(e => e.Website).HasColumnName("WEBSITE").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("CREATEDAT");
        });

        // Price
        modelBuilder.Entity<Price>(entity =>
        {
            entity.ToTable("Prices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("PRICEID");
            entity.Property(e => e.ProductId).HasColumnName("PRODUCTID");
            entity.Property(e => e.MarketId).HasColumnName("MARKETID");
            entity.Property(e => e.PriceValue).HasColumnName("PRICE").HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountPrice).HasColumnName("DISCOUNTPRICE").HasColumnType("decimal(18,2)");
            entity.Property(e => e.UpdatedAt).HasColumnName("UPDATEDAT");
            entity.Property(e => e.CreatedAt).HasColumnName("CREATEDAT");
            entity.Property(e => e.IsDeleted).HasColumnName("ISDELETED");
            entity.HasOne(e => e.Product).WithMany(p => p.Prices).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Market).WithMany(m => m.Prices).HasForeignKey(e => e.MarketId).OnDelete(DeleteBehavior.Cascade);
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("CATEGORYID");
            entity.Property(e => e.CategoryName).HasColumnName("CATEGORYNAME").HasMaxLength(100);
            entity.Property(e => e.Icon).HasColumnName("ICON").HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnName("CREATEDAT");
        });

        // ShoppingList
        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.ToTable("ShoppingLists");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("SHOPPINGLISTID");
            entity.Property(e => e.UserId).HasColumnName("USERID");
            entity.Property(e => e.ListName).HasColumnName("LISTNAME").HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnName("CREATEDAT");
            entity.HasOne(e => e.User).WithMany(u => u.ShoppingLists).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ShoppingListItem
        modelBuilder.Entity<ShoppingListItem>(entity =>
        {
            entity.ToTable("ShoppingListItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("LISTITEMID");
            entity.Property(e => e.ShoppingListId).HasColumnName("SHOPPINGLISTID");
            entity.Property(e => e.ProductId).HasColumnName("PRODUCTID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.CreatedAt).HasColumnName("CREATEDAT");
            entity.HasOne(e => e.ShoppingList).WithMany(sl => sl.Items).HasForeignKey(e => e.ShoppingListId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product).WithMany(p => p.ShoppingListItems).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserAddress>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Price>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ShoppingList>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Market>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ShoppingListItem>().HasQueryFilter(e => !e.IsDeleted);


    }
}
