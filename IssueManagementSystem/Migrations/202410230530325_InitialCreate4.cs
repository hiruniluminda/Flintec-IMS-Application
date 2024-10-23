namespace IssueManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SummaryReports", "DSAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "DSAA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "ProductionProcessTimeT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "ProductionProcessTimeA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "KaizensT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "KaizensA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SummaryReports", "KaizensA", c => c.String());
            AlterColumn("dbo.SummaryReports", "KaizensT", c => c.String());
            AlterColumn("dbo.SummaryReports", "ProductionProcessTimeA", c => c.String());
            AlterColumn("dbo.SummaryReports", "ProductionProcessTimeT", c => c.String());
            AlterColumn("dbo.SummaryReports", "DSAA", c => c.String());
            AlterColumn("dbo.SummaryReports", "DSAT", c => c.String());
        }
    }
}
