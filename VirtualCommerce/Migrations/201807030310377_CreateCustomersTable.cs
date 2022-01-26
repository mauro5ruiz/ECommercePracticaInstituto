namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCustomersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departments", "Department_DepartmentId", "dbo.Departments");
            DropIndex("dbo.Departments", new[] { "Department_DepartmentId" });
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 256),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Phone = c.String(nullable: false, maxLength: 20),
                        Address = c.String(nullable: false, maxLength: 100),
                        DepartmentId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.CityId);
            
            DropColumn("dbo.Departments", "Department_DepartmentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "Department_DepartmentId", c => c.Int());
            DropForeignKey("dbo.Customers", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Customers", "CityId", "dbo.Cities");
            DropIndex("dbo.Customers", new[] { "CityId" });
            DropIndex("dbo.Customers", new[] { "DepartmentId" });
            DropTable("dbo.Customers");
            CreateIndex("dbo.Departments", "Department_DepartmentId");
            AddForeignKey("dbo.Departments", "Department_DepartmentId", "dbo.Departments", "DepartmentId");
        }
    }
}
