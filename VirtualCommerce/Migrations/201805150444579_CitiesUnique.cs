namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CitiesUnique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Cities", new[] { "DepartmentId" });
            CreateIndex("dbo.Cities", new[] { "DepartmentId", "Name" }, unique: true, name: "IX_Cities_DepartmentId_Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Cities", "IX_Cities_DepartmentId_Name");
            CreateIndex("dbo.Cities", "DepartmentId");
        }
    }
}
