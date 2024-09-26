using IssueManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IssueManagementSystem.Controllers
{
    public class DisplayController : Controller
    {
       
      
        public ActionResult slect()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Error"); // Return an error view
            }
        }

 
        public ActionResult Rasp(int id)

        {
            try
            {
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {


                    ViewBag.id = id;
                    //   ViewBag.issueoccourInfo = db.issue_occurrence.Where(x => x.line_line_id == id && x.issue_satus=="1").Count();
                    //if (ViewBag.issueoccourInfo == 0 && id == 3)
                    //{
                    //    return Redirect("http://192.168.1.30:84/Report/GagingD7/" + id);
                    //}
                    //else if (ViewBag.issueoccourInfo == 0 && id == 17)
                    //{
                    //    return Redirect("http://192.168.1.30:84/Report/GagingD8/" + id);
                    //}
                    //else
                    return View();

                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }

        }
        public ActionResult Stores(string location)
        {
            try
            {
                ViewBag.location = location;
                return View();
            }
            catch(Exception e) {
                return View("Error");
            }

        }
        public ActionResult machineshop(int id)
        {
            try { 
            ViewBag.id = id;
            return View();
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        public ActionResult accessoriesHeatTreatment(int id)
        {
            try
            {
                ViewBag.id = id;
                return View();
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
        public ActionResult Maintenance(string location)

        {
            try
            {
                ViewBag.location = location;
                return View();
            }
            catch (Exception e)
            {
                return View("Error");
            }

        }
        public ActionResult test()
        {
            try
            {
                return View();
            }catch(Exception e)
            {
                return View("Error");
            }

        }
        [HttpPost]//solvedIssueMethod
        public JsonResult updateScreen()
        {
            try
            {
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the screen." });
            }
        }
        class machines_temp{
            public string machine_machine_id { get; set; }
        }

        [HttpPost]
        public ActionResult getBlinkingMachinesList(string line)
        {
            try
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
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while fetching machine list." });
            }
        }
    }
}