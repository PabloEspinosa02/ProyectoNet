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
        public DbSet<Role> Roles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Insertar en la tabla usuario
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    Username = "Admin",
                    PasswordHash = "Admin",
                    Email = "admin@gmail.com",
                    IdRole = 1
                },
                new User()
                {
                    Id = 2,
                    Username = "Pablo",
                    PasswordHash = "123456",
                    Email = "pablo@gmail.com",
                    IdRole = 2
                }

            );

            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    IdRole = 1,
                    NameRole = "Admin"
                },
                new Role()
                {
                    IdRole = 2,
                    NameRole = "User"
                }
            );

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
