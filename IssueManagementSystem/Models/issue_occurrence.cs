//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IssueManagementSystem.Models
{
    using System;
    using System.Collections.Generic;

    public partial class issue_occurrence
    {
        internal int issue_status;

        public int issue_occurrence_id { get; set; }
        public Nullable<System.DateTime> issue_date { get; set; }
        public string material_id { get; set; }
        public string description { get; set; }
        public string machine_machine_id { get; set; }
        public Nullable<int> line_line_id { get; set; }
        public Nullable<int> issue_issue_ID { get; set; }
        public Nullable<int> responsible_person_emp_id { get; set; }
        public string issue_satus { get; set; }
        public string location { get; set; }
        public Nullable<int> responsible_person_confirm_status { get; set; }
        public string responsible_person_confirm_feedback { get; set; }
        public Nullable<System.DateTime> solved_date { get; set; }
        public Nullable<System.DateTime> commented_date { get; set; }
        public string department { get; set; }
        public Nullable<int> solved_emp_id { get; set; }
        public Nullable<int> buzzer_off_by { get; set; }
        public Nullable<System.DateTime> buzzer_off_time { get; set; }
        public string issueDate { get; set; }
        public string solvedDate { get; set; }
        public string commentedDate { get; set; }
        public string responciblepersonName { get; set; }
        public virtual issue issue { get; set; }
        public string matirial { get; set; }
        public string lineid { get; set; }
        public string Role { get; set; }
        public string solved_by { get; set; }
        public String lineName { get; set; }
        public String issueName { get; set; }
        public Nullable<int> group { get; set; }
        public string job_card { get; set; }
        public Nullable<int> loged_by { get; set; }
    }
}
