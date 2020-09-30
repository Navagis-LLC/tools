using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RegisterProject_Spice.Pages.Models
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options): base(options){}

        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<BillingAccount> BillingAccounts { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>()
            .HasIndex(b => b.Username)
            .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
