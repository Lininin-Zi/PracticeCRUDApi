using Microsoft.EntityFrameworkCore;

namespace PracticeCRUDApi.Models
{
    public class ProductsDbContext: DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = null!;//產品資料表DbSet

        public virtual DbSet<User> Users { get; set; }//使用者資料表DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasMaxLength(200);
            });
        }
    }
}
