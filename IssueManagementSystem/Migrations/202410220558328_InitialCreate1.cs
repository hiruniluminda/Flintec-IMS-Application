namespace IssueManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MonthlySummaryVD", newName: "SummaryReports");
        }

        public override void Down()
        {
            RenameTable(name: "dbo.SummaryReports", newName: "MonthlySummaryVD");
        }
    }
}
