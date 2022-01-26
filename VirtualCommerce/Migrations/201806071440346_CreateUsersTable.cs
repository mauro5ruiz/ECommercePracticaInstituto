namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateUsersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 256),
                        FirstName = c.String(nullable: false, maxLength: 120),
                        LastName = c.String(nullable: false, maxLength: 120),
                        Phone = c.String(),
                        Photo = c.String(),
                        Address = c.String(nullable: false, maxLength: 120),
                        DepartmentId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.UserName, unique: true, name: "IX_Users_UserName")
                .Index(t => t.DepartmentId)
                .Index(t => t.CityId)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Users", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Users", "CityId", "dbo.Cities");
            DropIndex("dbo.Users", new[] { "CompanyId" });
            DropIndex("dbo.Users", new[] { "CityId" });
            DropIndex("dbo.Users", new[] { "DepartmentId" });
            DropIndex("dbo.Users", "IX_Users_UserName");
            DropTable("dbo.Users");
        }
    }
}
