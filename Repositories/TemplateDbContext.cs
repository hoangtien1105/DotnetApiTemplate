using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;

namespace Repositories
{
    public class TemplateDbContext : IdentityDbContext<Account>
    {
        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options) { }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.HasKey(e => e.Id).HasName("Order_pk");
                entity.Property(e => e.Description).HasMaxLength(100);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(a => a.Account)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                 .HasConstraintName("Order_Account_Fk");
            });

            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.HasOne(p => p.Order)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderDetails_Order_Fk");


                entity.HasOne(p => p.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderDetails_Product_Fk");
            });

        }



    }
}