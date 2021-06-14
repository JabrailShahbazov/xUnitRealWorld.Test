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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
