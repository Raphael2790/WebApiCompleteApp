using Microsoft.EntityFrameworkCore;
using RSS.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSS.Data.Context
{
    public class CompleteAppDbContext : DbContext
    {
        public CompleteAppDbContext(DbContextOptions<CompleteAppDbContext> options) : base (options) {}

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompleteAppDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }
    }
}
