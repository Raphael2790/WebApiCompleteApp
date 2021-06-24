using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RSS.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSS.Data.EntitiesMappings
{
    public class SupplierConfigurationMap : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(s => s.IdentificationDocument)
                .IsRequired()
                .HasColumnType("varchar(14)");

            builder.Property(s => s.Active)
               .HasColumnType("bit");

            builder.Property(s => s.SupplierType)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnType("tinyint");

            builder.HasOne(s => s.Adress)
                .WithOne(a => a.Supplier);

            builder.HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId);
        }
    }
}
