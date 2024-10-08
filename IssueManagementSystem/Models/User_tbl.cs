namespace IssueManagementSystem.Models
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    public partial class User_tbl
    {
        public int EmployeeNumber { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Plese Enter Your EmployeeNumber")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Plese Enter Your User PassWord")]
        public string Password { get; set; }
        public string SuplierAndIntrancit { get; set; }
        public string ChemicalAndMechaniacl { get; set; }
        public string FlintecStockManegement { get; set; }
        public string CenteringProcess { get; set; }
        public string MaterialRequest { get; set; }
        public string SerialNumberGenaration { get; set; }
        public string MachineshopBinHandling { get; set; }
        public string HTProcessControling { get; set; }
        public string QulityApprovels { get; set; }
        public string OutsourceHandling { get; set; }
        public string BinTrackingAndTransfering { get; set; }
        public string EMail { get; set; }
        public string Location { get; set; }
        public string Gaging { get; set; }
        public string Span { get; set; }
        public string Engineering { get; set; }
        public string AssemblyLine { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public string LoginErrorMessage { get; set; }
    }
}