namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCategoriesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => new { t.CompanyId, t.Description }, unique: true, name: "IX_Category_CompanyId_Description");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Categories", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Categories", "IX_Category_CompanyId_Description");
            DropTable("dbo.Categories");
        }
    }
}
