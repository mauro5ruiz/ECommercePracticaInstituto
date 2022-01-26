namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCompanyCustomersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyCustomers",
                c => new
                    {
                        CompanyCustomerId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyCustomerId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CompanyId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanyCustomers", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.CompanyCustomers", "CompanyId", "dbo.Companies");
            DropIndex("dbo.CompanyCustomers", new[] { "CustomerId" });
            DropIndex("dbo.CompanyCustomers", new[] { "CompanyId" });
            DropTable("dbo.CompanyCustomers");
        }
    }
}
