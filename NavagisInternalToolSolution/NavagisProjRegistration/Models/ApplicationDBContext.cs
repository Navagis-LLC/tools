using System.Data.Entity;

namespace NavagisProjRegistration.Models
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<BillingAccount> BillingAccounts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AdminResetPassword> AdminResetPasswords { get; set; }
    }
}