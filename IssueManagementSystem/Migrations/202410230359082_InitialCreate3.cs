namespace IssueManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SummaryReports", "Month", c => c.String());
            AddColumn("dbo.SummaryReports", "Year", c => c.Int(nullable: false));
            AlterColumn("dbo.SummaryReports", "Wk", c => c.String());
            AlterColumn("dbo.SummaryReports", "InternalAccA", c => c.Int(nullable: false));
            AlterColumn("dbo.SummaryReports", "CustomerCompT", c => c.Int(nullable: false));
            AlterColumn("dbo.SummaryReports", "CustomerCompA", c => c.Int(nullable: false));
            AlterColumn("dbo.SummaryReports", "sampleAuditFindA", c => c.Int(nullable: false));
            AlterColumn("dbo.SummaryReports", "DSAA", c => c.String());
            AlterColumn("dbo.SummaryReports", "ProductionProcessTimeT", c => c.String());
            AlterColumn("dbo.SummaryReports", "ReworkCostT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "KaizensA", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SummaryReports", "KaizensA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "ReworkCostT", c => c.String());
            AlterColumn("dbo.SummaryReports", "ProductionProcessTimeT", c => c.Int(nullable: false));
            AlterColumn("dbo.SummaryReports", "DSAA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "sampleAuditFindA", c => c.String());
            AlterColumn("dbo.SummaryReports", "CustomerCompA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SummaryReports", "CustomerCompT", c => c.String());
            AlterColumn("dbo.SummaryReports", "InternalAccA", c => c.String());
            AlterColumn("dbo.SummaryReports", "Wk", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SummaryReports", "Year");
            DropColumn("dbo.SummaryReports", "Month");
        }
    }
}
