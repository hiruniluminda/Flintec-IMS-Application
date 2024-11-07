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
        public String Date { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string Wk { get; set; }
        public int InternalAccT { get; set; }
        public int InternalAccA { get; set; }
        public int CustomerCompT { get; set; }
        public int CustomerCompA { get; set; }
        public int sampleAuditFindT { get; set; }
        public int sampleAuditFindA { get; set; }
        public decimal DSAT { get; set; }
        public decimal DSAA { get; set; }
        public decimal ProductionProcessTimeT { get; set; }
        public decimal ProductionProcessTimeA { get; set; }
        public decimal ReworkCostT { get; set; }
        public decimal ReworkCostA { get; set; }
        public decimal KaizensT { get; set; }
        public decimal KaizensA { get; set; }
    }
}