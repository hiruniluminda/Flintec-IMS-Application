using IssueManagementSystem.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace IssueManagementSystem.Controllers
{
    public class DisplayController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            using (var db = new issue_management_systemEntities1())
            {
                // Check department in session and set counts
                SetViewBagCounts(db);
                return View();
            }
        }

        private void SetViewBagCounts(issue_management_systemEntities1 db)
        {
            // Depending on the department, count issues
            if ((string)Session["department"] == "MD")
            {
                ViewBag.BrakedownCount = db.issue_occurrence.Count(x => x.issue_issue_ID == 1 && x.issue_satus == "1");
                ViewBag.ITIsuue = db.issue_occurrence.Count(x => x.issue_issue_ID == 5 && x.issue_satus == "1");
                ViewBag.TechnicalIssue = db.issue_occurrence.Count(x => x.issue_issue_ID == 3 && x.issue_satus == "1");
                ViewBag.MaterialDelayCount = db.issue_occurrence.Count(x => x.issue_issue_ID == 2 && x.issue_satus == "1");
                ViewBag.QualityIsuue = db.issue_occurrence.Count(x => x.issue_issue_ID == 4 && x.issue_satus == "1");
            }
            else
            {
                string location = Session["location"].ToString();
                // Similar logic, but can add filtering by location if required
                ViewBag.BrakedownCount = db.issue_occurrence.Count(x => x.location == location && x.issue_issue_ID == 1 && x.issue_satus == "1");
                ViewBag.ITIsuue = db.issue_occurrence.Count(x => x.location == location && x.issue_issue_ID == 5 && x.issue_satus == "1");
                ViewBag.TechnicalIssue = db.issue_occurrence.Count(x => x.location == location && x.issue_issue_ID == 3 && x.issue_satus == "1");
                ViewBag.MaterialDelayCount = db.issue_occurrence.Count(x => x.location == location && x.issue_issue_ID == 2 && x.issue_satus == "1");
                ViewBag.QualityIsuue = db.issue_occurrence.Count(x => x.location == location && x.issue_issue_ID == 4 && x.issue_satus == "1");
            }
        }

        public ActionResult Rasp(int id)
        {
            using (var db = new issue_management_systemEntities1())
            {
                // Retrieve issue counts for a specific line ID
                var lineCounts = new
                {
                    BrakedownCount = db.issue_occurrence.Count(x => x.line_line_id == id && x.issue_issue_ID == 1 && x.issue_satus == "1"),
                    ITIsuue = db.issue_occurrence.Count(x => x.line_line_id == id && x.issue_issue_ID == 5 && x.issue_satus == "1"),
                    TechnicalIssue = db.issue_occurrence.Count(x => x.line_line_id == id && x.issue_issue_ID == 3 && x.issue_satus == "1"),
                    MaterialDelayCount = db.issue_occurrence.Count(x => x.line_line_id == id && x.issue_issue_ID == 2 && x.issue_satus == "1"),
                    QualityIsuue = db.issue_occurrence.Count(x => x.line_line_id == id && x.issue_issue_ID == 4 && x.issue_satus == "1")
                };

                ViewBag.BrakedownCount = lineCounts.BrakedownCount;
                ViewBag.ITIsuue = lineCounts.ITIsuue;
                ViewBag.TechnicalIssue = lineCounts.TechnicalIssue;
                ViewBag.MaterialDelayCount = lineCounts.MaterialDelayCount;
                ViewBag.QualityIsuue = lineCounts.QualityIsuue;
                ViewBag.id = id; // Pass the line ID to the View

                return View();
            }
        }

        public JsonResult FilterSelectBoxes()
        {
            string query = @"SELECT DISTINCT deps.department_id, deps.department_name,
                                    lines.line_id, lines.line_name, l_map.issues AS issues
                             FROM issue_management_system.dbo.departments deps
                             JOIN issue_management_system.dbo.lines lines ON lines.department_id = deps.department_id
                             JOIN issue_management_system.dbo.line_map l_map ON l_map.line_id = lines.line_id";
            using (var db = new issue_management_systemEntities1())
            {
                var result = db.Database.SqlQuery<TempClasses.tempClass10>(query).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult updateScreen()
        {
            try
            {
                // Logic to update the screen if needed
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the screen." });
            }
        }

        class machines_temp
        {
            public string machine_machine_id { get; set; }
        }

        [HttpPost]
        public JsonResult getBlinkingMachinesList(string line)
        {
            try
            {
                int lineID = Int32.Parse(line);

                using (var db = new issue_management_systemEntities1())
                {
                    // Fetch machines with active issues for the specified line
                    string query = "SELECT issue_occurrence.machine_machine_id FROM issue_occurrence WHERE issue_occurrence.line_line_id = @lineID AND issue_occurrence.issue_issue_ID = 1 AND issue_occurrence.issue_satus = 1";
                    var machines = db.Database.SqlQuery<machines_temp>(query, new SqlParameter("@lineID", lineID)).ToList();

                    return Json(machines, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while fetching machine list." });
            }
        }
    }
}
