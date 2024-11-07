using IssueManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Shapes;

namespace IssueManagementSystem.Controllers
{
    public class QRViewController : Controller
    {

        public ActionResult QRView(int id) // Main view action
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {

                ViewBag.id = id;
                List<line> lineList = db.lines.ToList(); // Load lines for the dropdown
                ViewBag.lineList = lineList;

                var issueList = db.issue_occurrence
                    .Where(x => x.line_line_id == id && x.issue_satus == "1")
                    .ToList(); // Load issues for the default line
                List<User_tbl> Name = db.User_tbl.ToList();
                ViewBag.empName = Name;

                return View(issueList); // Pass issue list to the view
            }
        }

        public ActionResult GetIssuesByLine(int lineId) // AJAX action to get issues based on line ID
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                var issues = db.issue_occurrence
                    .Where(x => x.line_line_id == lineId && x.issue_satus == "1")
                    .ToList();

                return PartialView("_IssueTable", issues); // Return a partial view for the issues table
            }
        }
    




    public ActionResult Stores(string location)
        {
            ViewBag.location = location;
            return View();

        }
        public ActionResult machineshop(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult accessoriesHeatTreatment(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Maintenance(string location)


        {
            ViewBag.location = location;
            return View();

        }
        public ActionResult test()
        {

            return View();

        }
        [HttpPost]//solvedIssueMethod
        public JsonResult updateScreen()
        {

            return Json(true);
        }
        class machines_temp
        {
            public string machine_machine_id { get; set; }
        }

        [HttpPost]
        public ActionResult getBlinkingMachinesList(string line)
        {
            int lineID = Int32.Parse(line);

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                //List<issue_occurrence> c = db.issue_occurrence.Where(x=>x.line_line_id== lineID && x.issue_issue_ID == 1).ToList();
                //foreach (issue_occurrence x in c) { System.Diagnostics.Debug.WriteLine(x.machine_machine_id); }
                String query_1 = "SELECT issue_occurrence.machine_machine_id FROM issue_occurrence WHERE issue_occurrence.line_line_id =" + line + " AND issue_occurrence.issue_issue_ID=1 AND issue_occurrence.issue_satus=1";
                var c = db.Database.SqlQuery<machines_temp>(query_1).ToList();

                return Json(c, JsonRequestBehavior.AllowGet);
            }
        }
    }
}