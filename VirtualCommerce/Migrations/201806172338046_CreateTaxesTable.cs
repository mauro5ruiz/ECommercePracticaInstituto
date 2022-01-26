namespace VirtualCommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTaxesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        TaxId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                        CompanyId = c.Int(nullable: false),
                        Rate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TaxId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => new { t.CompanyId, t.Description }, unique: true, name: "IX_Taxes_CompanyId_Description");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Taxes", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Taxes", "IX_Taxes_CompanyId_Description");
            DropTable("dbo.Taxes");
        }
    }
}
