using InvoicesApp.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace InvoicesApp.DAL
{
    public class WarehouseContext : DbContext
    {

        public WarehouseContext() : base("WarehouseContext")
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

    }
}