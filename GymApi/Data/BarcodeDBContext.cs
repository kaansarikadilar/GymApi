using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Data
{
       public class BarcodeDbContext : DbContext
    {
        public BarcodeDbContext(DbContextOptions<BarcodeDbContext> options) : base(options) { }

        public DbSet<Barcode> Barcodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Barcode>()
                .HasIndex(b => b.Code)
                .IsUnique();
        }
    }
}