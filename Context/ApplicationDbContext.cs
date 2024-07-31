using Microsoft.EntityFrameworkCore;
using TiendaUT.Domain;

namespace TiendaUT.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.TotalAmount)
                      .HasColumnType("decimal(18,2)");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
