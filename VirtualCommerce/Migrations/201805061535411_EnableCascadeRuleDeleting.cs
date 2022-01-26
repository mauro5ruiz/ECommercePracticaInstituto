namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableCascadeRuleDeleting : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cities", "DepartmentId", "dbo.Departments");
            AddForeignKey("dbo.Cities", "DepartmentId", "dbo.Departments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cities", "DepartmentId", "dbo.Departments");
            AddForeignKey("dbo.Cities", "DepartmentId", "dbo.Departments", "DepartmentId", cascadeDelete: true);
        }
    }
}
