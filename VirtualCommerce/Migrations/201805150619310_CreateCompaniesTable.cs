namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCompaniesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(nullable: false),
                        Logo = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.Name, unique: true, name: "IX_Companies_Name")
                .Index(t => t.DepartmentId)
                .Index(t => t.CityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Companies", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Companies", "CityId", "dbo.Cities");
            DropIndex("dbo.Companies", new[] { "CityId" });
            DropIndex("dbo.Companies", new[] { "DepartmentId" });
            DropIndex("dbo.Companies", "IX_Companies_Name");
            DropTable("dbo.Companies");
        }
    }
}
