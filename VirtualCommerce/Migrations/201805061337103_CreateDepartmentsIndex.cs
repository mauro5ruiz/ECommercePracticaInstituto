namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDepartmentsIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Departments", "Name", unique: true, name: "IX_Departments_Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Departments", "IX_Departments_Name");
        }
    }
}
