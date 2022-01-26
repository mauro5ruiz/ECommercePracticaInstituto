namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateVARIASTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        PurchaseId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        SupplierId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId)
                .Index(t => t.CompanyId)
                .Index(t => t.SupplierId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.PurchaseDetails",
                c => new
                    {
                        PurchaseDetailId = c.Int(nullable: false, identity: true),
                        PurchaseId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 100),
                        TaxRate = c.Double(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseDetailId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Purchases", t => t.PurchaseId)
                .Index(t => t.PurchaseId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.PurchaseDetailTmps",
                c => new
                    {
                        PurchaseDetailTmpId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 256),
                        ProductId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 100),
                        TaxRate = c.Double(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseDetailTmpId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.StatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchases", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.PurchaseDetails", "PurchaseId", "dbo.Purchases");
            DropForeignKey("dbo.PurchaseDetailTmps", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PurchaseDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Purchases", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.Purchases", "CompanyId", "dbo.Companies");
            DropIndex("dbo.PurchaseDetailTmps", new[] { "ProductId" });
            DropIndex("dbo.PurchaseDetails", new[] { "ProductId" });
            DropIndex("dbo.PurchaseDetails", new[] { "PurchaseId" });
            DropIndex("dbo.Purchases", new[] { "WarehouseId" });
            DropIndex("dbo.Purchases", new[] { "SupplierId" });
            DropIndex("dbo.Purchases", new[] { "CompanyId" });
            DropTable("dbo.Status");
            DropTable("dbo.PurchaseDetailTmps");
            DropTable("dbo.PurchaseDetails");
            DropTable("dbo.Purchases");
        }
    }
}
