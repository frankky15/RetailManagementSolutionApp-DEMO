﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RMS.Models;

namespace RMS.Data
{
    public partial class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> Brands { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Staff> Staffs { get; set; }

        public virtual DbSet<Stock> Stocks { get; set; }

        public virtual DbSet<Store> Stores { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.BrandId).HasName("PK__brands__5E5A8E273BB2C54D");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__categori__D54EE9B4A1A7C974");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB85196B7BA5");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__orders__46596229765FE789");

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__orders__customer__47DBAE45");

                entity.HasOne(d => d.Staff).WithMany(p => p.Orders)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__orders__staff_id__49C3F6B7");

                entity.HasOne(d => d.Store).WithMany(p => p.Orders).HasConstraintName("FK__orders__store_id__48CFD27E");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ItemId }).HasName("PK__order_it__837942D495266F68");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__order__4D94879B");

                entity.HasOne(d => d.Product).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__produ__4E88ABD4");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId).HasName("PK__products__47027DF50E210C70");

                entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasConstraintName("FK__products__brand___3C69FB99");

                entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__products__catego__3B75D760");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("PK__staffs__1963DD9CF2F8A053");

                entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager).HasConstraintName("FK__staffs__manager___44FF419A");

                entity.HasOne(d => d.Store).WithMany(p => p.Staff).HasConstraintName("FK__staffs__store_id__440B1D61");

                entity.HasOne(d => d.User).WithOne(p => p.Staff).HasForeignKey<Staff>(d => d.UserId).HasConstraintName("FK_AspNetUsers_staffs_staff_id");
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_AspNetUsers");

                entity.HasOne(d => d.Staff).WithOne(p => p.User).HasForeignKey<Staff>(d => d.UserId).HasConstraintName("FK_AspNetUsers_staffs_staff_id");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.ProductId }).HasName("PK__stocks__E68284D394389A60");

                entity.HasOne(d => d.Product).WithMany(p => p.Stocks).HasConstraintName("FK__stocks__product___52593CB8");

                entity.HasOne(d => d.Store).WithMany(p => p.Stocks).HasConstraintName("FK__stocks__store_id__5165187F");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.StoreId).HasName("PK__stores__A2F2A30C55BC99CA");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
