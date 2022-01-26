namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateWarehousesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        WarehouseId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 50),
                        Phone = c.String(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WarehouseId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => new { t.CompanyId, t.Name }, unique: true, name: "IX_Warehouses_CompanyId_Description")
                .Index(t => t.DepartmentId)
                .Index(t => t.CityId);
            
            AddColumn("dbo.Departments", "Department_DepartmentId", c => c.Int());
            CreateIndex("dbo.Departments", "Department_DepartmentId");
            AddForeignKey("dbo.Departments", "Department_DepartmentId", "dbo.Departments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Warehouses", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Warehouses", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Warehouses", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Departments", "Department_DepartmentId", "dbo.Departments");
            DropIndex("dbo.Warehouses", new[] { "CityId" });
            DropIndex("dbo.Warehouses", new[] { "DepartmentId" });
            DropIndex("dbo.Warehouses", "IX_Warehouses_CompanyId_Description");
            DropIndex("dbo.Departments", new[] { "Department_DepartmentId" });
            DropColumn("dbo.Departments", "Department_DepartmentId");
            DropTable("dbo.Warehouses");
        }
    }
}
