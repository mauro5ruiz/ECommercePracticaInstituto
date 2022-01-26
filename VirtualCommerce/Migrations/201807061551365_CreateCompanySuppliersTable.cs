namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCompanySuppliersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanySuppliers",
                c => new
                    {
                        CompanySupplierId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        SupplierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanySupplierId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId)
                .Index(t => t.CompanyId)
                .Index(t => t.SupplierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanySuppliers", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.CompanySuppliers", "CompanyId", "dbo.Companies");
            DropIndex("dbo.CompanySuppliers", new[] { "SupplierId" });
            DropIndex("dbo.CompanySuppliers", new[] { "CompanyId" });
            DropTable("dbo.CompanySuppliers");
        }
    }
}
