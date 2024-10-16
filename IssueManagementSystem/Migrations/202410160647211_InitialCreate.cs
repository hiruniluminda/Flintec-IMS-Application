namespace IssueManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MonthlySummaryVDs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LineId = c.String(),
                        Date = c.String(),
                        Wk = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InternalAccT = c.Int(nullable: false),
                        InternalAccA = c.String(),
                        CustomerCompT = c.String(),
                        CustomerCompA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        sampleAuditFindT = c.Int(nullable: false),
                        sampleAuditFindA = c.String(),
                        DSAT = c.String(),
                        DSAA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionProcessTimeT = c.Int(nullable: false),
                        ProductionProcessTimeA = c.String(),
                        ReworkCostT = c.String(),
                        ReworkCostA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        KaizensT = c.String(),
                        KaizensA = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MonthlySummaryVDs");
        }
    }
}
