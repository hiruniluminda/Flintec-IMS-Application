using IssueManagementSystem.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IssueManagementSystem.Controllers
{
    public class ResponsiblePersonController : Controller
    {
        // GET: ResponsiblePerson
        public ActionResult Index()
        {
            if (Session["userID"] == null || ((string)Session["Role"] != "responsiblePerson"))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public JsonResult GetIssues()
        {
            try
            {
                return Json(IssueService.GetIssue(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                // Log the exception (e.g., to a file or monitoring system)
                return Json(new { success = false, message = "Error fetching issues: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getlineInfo(int? list_id)
        {
            try
            {
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    var lineInfo = db.lines.FirstOrDefault(x => x.line_id == list_id);
                    if (lineInfo == null)
                    {
                        return Json(new { success = false, message = "Line not found." }, JsonRequestBehavior.AllowGet);
                    }

                    var line = new line
                    {
                        line_id = lineInfo.line_id,
                        line_name = lineInfo.line_name
                    };

                    return Json(line, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error fetching line info: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult savecomment(int? id, string comment, int? emp_id)
        {
            try
            {
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    var time = DateTime.Now;
                    string current_time = time.ToString("yyyy-MM-dd HH:mm:ss");
                    string query = $"UPDATE issue_occurrence SET responsible_person_confirm_status = 0, responsible_person_confirm_feedback='{comment}', commented_date='{current_time}', solved_emp_id='{emp_id}' WHERE issue_occurrence_id = {id}";

                    db.Database.ExecuteSqlCommand(query);
                    var issueinfo = db.issue_occurrence.FirstOrDefault(x => x.issue_occurrence_id == id);
                    return Json(issueinfo?.issue_satus, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error saving comment: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult offBuzzer(string issueJson)
        {
            try
            {
                Communication com = new Communication();
                JArray issueData = JArray.Parse(issueJson) as JArray;

                foreach (JObject item in issueData)
                {
                    var time = DateTime.Now;
                    string current_time = time.ToString("yyyy-MM-dd HH:mm");
                    var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    int issueid = Int32.Parse(item["issueid"].ToString());
                    int emp_id = Int32.Parse(item["employee_id"].ToString());

                    using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                    {
                        var issueoccourInfo = db.issue_occurrence.FirstOrDefault(x => x.issue_occurrence_id == issueid);
                        if (issueoccourInfo == null)
                        {
                            return Json(new { success = false, message = $"Issue with ID {issueid} not found." }, JsonRequestBehavior.AllowGet);
                        }

                        issueoccourInfo.buzzer_off_by = emp_id;
                        issueoccourInfo.buzzer_off_time = date;
                        var location = item["location"].ToString();
                        db.SaveChanges();

                        var count = db.issue_occurrence.Count(x => x.issue_issue_ID == 2 && x.buzzer_off_by == null);
                        if (count == 0)
                        {
                            com.storesbuzzerOff(location);
                        }
                    }
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error turning off buzzer: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult offBuzzermaintenance(string issueJson)
        {
            try
            {
                Communication com = new Communication();
                JArray issueData = JArray.Parse(issueJson) as JArray;

                foreach (JObject item in issueData)
                {
                    var time = DateTime.Now;
                    string current_time = time.ToString("yyyy-MM-dd HH:mm");
                    var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    int issueid = Int32.Parse(item["issueid"].ToString());
                    int emp_id = Int32.Parse(item["employee_id"].ToString());

                    using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                    {
                        var issueoccourInfo = db.issue_occurrence.FirstOrDefault(x => x.issue_occurrence_id == issueid);
                        if (issueoccourInfo == null)
                        {
                            return Json(new { success = false, message = $"Issue with ID {issueid} not found." }, JsonRequestBehavior.AllowGet);
                        }

                        issueoccourInfo.buzzer_off_by = emp_id;
                        issueoccourInfo.buzzer_off_time = date;
                        var location = item["location"].ToString();
                        db.SaveChanges();

                        var count = db.issue_occurrence.Count(x => x.issue_issue_ID == 1 && x.buzzer_off_by == null && x.location == location);
                        if (count == 0)
                        {
                            com.maintenancesbuzzerOff(location);
                        }
                    }
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error turning off maintenance buzzer: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}







/*using IssueManagementSystem.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IssueManagementSystem.Controllers
{
    public class ResponsiblePersonController : Controller
    {
        // GET: ResponsiblePerson
        public ActionResult Index()
        {
            if (Session["userID"] == null || ((string)Session["Role"] != "responsiblePerson"))
            {
                return RedirectToAction("Index", "Login");

            }

            return View();
         
        }
        public JsonResult GetIssues()
        {
            return Json(IssueService.GetIssue(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult getlineInfo(int? list_id)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                var lineInfo=db.lines.Where(x => x.line_id ==list_id).FirstOrDefault();
                line line=new line {
                    line_id = lineInfo.line_id,
                    line_name = lineInfo.line_name
                   
                };
                return Json(line, JsonRequestBehavior.AllowGet);
            }
               
            
        }

        public JsonResult savecomment(int? id ,string comment,int? emp_id)
        {
            try
            {
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    var time = DateTime.Now;
                    string current_time = time.ToString("yyyy-MM-dd HH:mm:ss");
                    string query = "UPDATE issue_occurrence SET responsible_person_confirm_status = 0 ,responsible_person_confirm_feedback='"+ comment + "',commented_date='"+ current_time + "' , solved_emp_id='"+ emp_id + "'WHERE issue_occurrence_id =" + id ;
                    db.Database.ExecuteSqlCommand(query);
                    var issueinfo = db.issue_occurrence.Where(x => x.issue_occurrence_id ==id).First();
                    return this.Json(issueinfo.issue_satus, JsonRequestBehavior.AllowGet);
                   
                }
            }
            catch (Exception e)
            {
                return this.Json(e, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult offBuzzer(string issueJson)
        {
            Communication com = new Communication();
            JArray issueData = JArray.Parse(issueJson) as JArray;
            foreach (JObject item in issueData)
            {
                var time = DateTime.Now;
                string current_time = time.ToString("yyyy-MM-dd HH:mm");
                var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                int issueid = Int32.Parse(item["issueid"].ToString());
                int emp_id = Int32.Parse(item["employee_id"].ToString());
                issue_management_systemEntities1 db = new issue_management_systemEntities1();
                var issueoccourInfo = db.issue_occurrence.Where(x => x.issue_occurrence_id == issueid).FirstOrDefault();
                issueoccourInfo.buzzer_off_by = emp_id;
                issueoccourInfo.buzzer_off_time = date;
                var location = item["location"].ToString();
                db.SaveChanges();

                var count = db.issue_occurrence.Where(x => x.issue_issue_ID == 2 && x.buzzer_off_by == null).Count();

                if (count == 0)
                {
                    com.storesbuzzerOff(location);
                }
            }
            return Json(true);
        }

        public JsonResult offBuzzermaintenance(string issueJson)
        {
            Communication com = new Communication();
            JArray issueData = JArray.Parse(issueJson) as JArray;
            foreach (JObject item in issueData)
            {
                var time = DateTime.Now;
                string current_time = time.ToString("yyyy-MM-dd HH:mm");
                var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                int issueid = Int32.Parse(item["issueid"].ToString());
                int emp_id = Int32.Parse(item["employee_id"].ToString());
         
                issue_management_systemEntities1 db = new issue_management_systemEntities1();
                var issueoccourInfo = db.issue_occurrence.Where(x => x.issue_occurrence_id == issueid).FirstOrDefault();
                issueoccourInfo.buzzer_off_by = emp_id;
                issueoccourInfo.buzzer_off_time = date;
                var location = item["location"].ToString();
                db.SaveChanges();

                var count = db.issue_occurrence.Where(x => x.issue_issue_ID == 1 && x.buzzer_off_by == null && x.location==location).Count();

                if (count == 0)
                {
                    com.maintenancesbuzzerOff(location);
                }
            }

            //update Issueststus as 0




            return Json(true);
        }
    }
   
}*/