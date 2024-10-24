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

        public static int lineId;

        // GET: QRIssueDisplay
        public ActionResult slect()
        {
            
            return View();
        }

        public ActionResult FilterIssuesByLine(int lineId)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                // Fetch the filtered issues based on lineId from the database or data source
                var filteredIssues = db.issue_occurrence.Where(x => x.line_line_id == lineId && x.issue_satus == "1").ToList();

                // Return a partial view with the filtered issues table or the data
                return PartialView("_FilteredIssuesTable", filteredIssues);
            }
        }

        public ActionResult QRView(int id)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {


                ViewBag.id = id;
                ViewBag.issueoccourInfo = db.issue_occurrence.Where(x => x.line_line_id == id  && x.issue_satus == "1").Count();
                //if (ViewBag.issueoccourInfo == 0 && id==3)
                //{
                //    return Redirect("http://192.168.1.30:84/Report/GagingD1/"+id);
                //}
                //else 
                

                List<line> lineList = db.lines.ToList();
                ViewBag.lineList = lineList;

                List<issue> issueList = db.issues.ToList();
                ViewBag.issueList = issueList;

                return View();

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