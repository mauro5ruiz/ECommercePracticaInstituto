namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateInventoriesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        InventoryId = c.Int(nullable: false, identity: true),
                        WarehouseId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Stock = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .Index(t => t.WarehouseId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inventories", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.Inventories", "ProductId", "dbo.Products");
            DropIndex("dbo.Inventories", new[] { "ProductId" });
            DropIndex("dbo.Inventories", new[] { "WarehouseId" });
            DropTable("dbo.Inventories");
        }
    }
}
