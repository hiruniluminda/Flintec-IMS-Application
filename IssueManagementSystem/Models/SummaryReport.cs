using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueManagementSystem.Models
{
    public class SummaryReport
    {
        public int Id { get; set; }
        public int LineId { get; set; }
        public string Date { get; set; }
        public decimal Wk { get; set; }
        public int InternalAccT { get; set; }
        public string InternalAccA { get; set; }
        public string CustomerCompT { get; set; }
        public decimal CustomerCompA { get; set; }
        public int sampleAuditFindT { get; set; }
        public string sampleAuditFindA { get; set; }
        public string DSAT { get; set; }
        public decimal DSAA { get; set; }
        public int ProductionProcessTimeT { get; set; }
        public string ProductionProcessTimeA { get; set; }
        public string ReworkCostT { get; set; }
        public decimal ReworkCostA { get; set; }
        public string KaizensT { get; set; }
        public decimal KaizensA { get; set; }
    }
}