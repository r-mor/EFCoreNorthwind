using Microsoft.EntityFrameworkCore;
using Project11E.Entity.Models;

namespace Project11E.Entity;

internal class Northwind : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    //public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (ProjectConstants.DatabaseProvider == "SqlServer")
        {
            optionsBuilder.UseSqlServer(ProjectConstants.SqlServerConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //global filter to remove discontinued products
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.Discontinued);

        modelBuilder.Entity<Order>()
            .Property(o => o.Id)
            .HasColumnName("OrderId");

        modelBuilder.Entity<OrderDetail>()
            .ToTable("Order Details");

        modelBuilder.Entity<OrderDetail>()
            .HasKey(o => new { o.OrderId, o.ProductId });

    }
}
