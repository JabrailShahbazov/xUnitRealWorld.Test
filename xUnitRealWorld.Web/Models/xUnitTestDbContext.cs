using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace xUnitRealWorld.Web.Models
{
    public partial class xUnitTestDbContext : DbContext
    {
        public xUnitTestDbContext()
        {
        }

        public xUnitTestDbContext(DbContextOptions<xUnitTestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        /*"Data Source=DESKTOP-8UI7HQO; Initial Catalog=xUnitTestDb;" +
        " Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;" +
        "ApplicationIntent=ReadWrite;MultiSubnetFailover=False"*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasData(new Category {Id = 1, Name = "Qelemler"},
                    new Category {Id = 2, Name = "Defterler"});
                entity.ToTable("Category");
                entity.Property(x => x.Name).HasMaxLength(50);
                entity.HasMany(x => x.Products).WithOne(x => x.Category)
                    .HasForeignKey(s => s.CategoryId);

            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
