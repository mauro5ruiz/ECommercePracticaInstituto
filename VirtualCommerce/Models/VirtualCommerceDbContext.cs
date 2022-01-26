using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Models
{
    public class VirtualCommerceDbContext:DbContext
    {
        public VirtualCommerceDbContext() : base("DefaultConnection")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<Department> Departments { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Tax> Taxes { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CompanyCustomer> CompanyCustomers { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<CompanySupplier> CompanySuppliers { get; set; }

        public DbSet<Status> Status { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<PurchaseDetailTmp> PurchaseDetailTmps { get; set; }
    }
}