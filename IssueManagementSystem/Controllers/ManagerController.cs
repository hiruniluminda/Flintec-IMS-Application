using IssueManagementSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace IssueManagementSystem.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult Index()
        {
            using (var db = new issue_management_systemEntities1())
            {
                if ((string)Session["department"] == "MD")
                {
                    SetViewBagCounts(db);
                }
                else
                {
                    string location = Session["location"].ToString();
                    SetViewBagCounts(db);
                }
                return View();
            }
        }

        private void SetViewBagCounts(issue_management_systemEntities1 db)
        {
            ViewBag.BrakedownCount = db.issue_occurrence.Count(x => x.issue_satus == "1" && x.issue_issue_ID == 1);
            ViewBag.MaterialDelayCount = db.issue_occurrence.Count(x => x.issue_satus == "1" && x.issue_issue_ID == 2);
            ViewBag.TechnicalIssue = db.issue_occurrence.Count(x => x.issue_satus == "1" && x.issue_issue_ID == 3);
            ViewBag.QualityIsuue = db.issue_occurrence.Count(x => x.issue_satus == "1" && x.issue_issue_ID == 4);
            ViewBag.ITIsuue = db.issue_occurrence.Count(x => x.issue_satus == "1" && x.issue_issue_ID == 5);
        }

        public JsonResult FilterSelectBoxes()
        {
            string query = @"SELECT DISTINCT deps.department_id, deps.department_name,
                                    lines.line_id, lines.line_name, l_map.issues AS issues
                             FROM issue_management_system_new.dbo.departments deps
                             JOIN issue_management_system_new.dbo.lines lines ON lines.department_id = deps.department_id
                             JOIN issue_management_system_new.dbo.line_map l_map ON l_map.line_id = lines.line_id";
            using (var db = new issue_management_systemEntities1())
            {
                var result = db.Database.SqlQuery<TempClasses.tempClass10>(query).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetNotification()
        {
            return Json(NotificaionService.GetNotification(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult HideNotification(int? notificationId)
        {
            if (notificationId == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            using (var db = new issue_management_systemEntities1())
            {
                string query = "UPDATE tbl_Notifications SET Status = 0 WHERE NotificationId = @id";
                db.Database.ExecuteSqlCommand(query, new SqlParameter("@id", notificationId));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FillChart1(string barChart, string startDate, string endDate, string plantLocation)
        {
            if (string.IsNullOrWhiteSpace(plantLocation) || string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return Json(new { error = "Invalid parameters" }, JsonRequestBehavior.AllowGet);

            try
            {
                switch (barChart)
                {
                    case "1":
                        return Json(GetChart1Data(startDate, endDate, plantLocation), JsonRequestBehavior.AllowGet);
                    case "2":
                        return Json(GetChart2Data(startDate, endDate, plantLocation), JsonRequestBehavior.AllowGet);
                    default:
                        return Json(new { error = "Invalid barChart value" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<TempClasses.tempClass> GetChart1Data(string startDate, string endDate, string plantLocation)
        {
            string query = @"SELECT TOP 10 issue_occurrence.machine_machine_id,
                                    COUNT(issue_occurrence.machine_machine_id) AS count 
                             FROM issue_occurrence 
                             WHERE issue_occurrence.issue_issue_ID = 1 
                             AND issue_occurrence.location IN (@location)
                             AND issue_occurrence.issue_date BETWEEN @startDate AND @endDate 
                             GROUP BY issue_occurrence.machine_machine_id 
                             ORDER BY count DESC";

            using (var db = new issue_management_systemEntities1())
            {
                return db.Database.SqlQuery<TempClasses.tempClass>(query,
                    new SqlParameter("@location", plantLocation),
                    new SqlParameter("@startDate", startDate),
                    new SqlParameter("@endDate", endDate)).ToList();
            }
        }

        private List<TempClasses.tempClass4> GetChart2Data(string startDate, string endDate, string plantLocation)
        {
            string query = @"SELECT TOP 10 ic.material_id AS Search_Description,
                                    COUNT(ic.material_id) AS count 
                             FROM issue_management_system_new.dbo.issue_occurrence ic 
                             WHERE ic.issue_issue_ID = 2
                             AND ic.location IN (@location)
                             AND ic.issue_date BETWEEN @startDate AND @endDate 
                             GROUP BY ic.material_id
                             ORDER BY count DESC";

            var chartData = new List<TempClasses.tempClass4>();

            using (var db = new issue_management_systemEntities1())
            {
                chartData = db.Database.SqlQuery<TempClasses.tempClass4>(query,
                    new SqlParameter("@location", plantLocation),
                    new SqlParameter("@startDate", startDate),
                    new SqlParameter("@endDate", endDate)).ToList();
            }

            foreach (var obj in chartData)
            {
                using (var db1 = new FLINTEC_Context())
                {
                    string query5 = @"SELECT TOP 1 f.[Search Description] 
                                      FROM FLINTEC.dbo.FLINTEC$Item f
                                      WHERE f.No_ = @materialId";
                    var description = db1.Database.SqlQuery<string>(query5, new SqlParameter("@materialId", obj.Search_Description)).FirstOrDefault();
                    obj.Search_Description = description ?? obj.Search_Description; // Keep original if not found
                }
            }
            return chartData;
        }

        [HttpPost]
        public JsonResult FillChart2(string startDate, string endDate, string plantLocation)
        {
            try
            {
                using (var db = new issue_management_systemEntities1())
                {
                    string query = @"SELECT TOP 10
                                        issue_management_system_new.dbo.issue_occurrence.issue_date,
                                        issue_management_system_new.dbo.issues.issue,
                                        issue_management_system_new.dbo.issue_occurrence.machine_machine_id,
                                        issue_management_system_new.dbo.issue_occurrence.material_id,
                                        BigRed.dbo.tbl_PPA_User.Name,
                                        DATEDIFF(minute, issue_occurrence.issue_date, issue_occurrence.solved_date) AS DateDiff
                                 FROM issue_management_system_new.dbo.issue_occurrence
                                 JOIN BigRed.dbo.tbl_PPA_User ON BigRed.dbo.tbl_PPA_User.UserName = issue_management_system_new.dbo.issue_occurrence.responsible_person_emp_id
                                 JOIN issue_management_system_new.dbo.issues ON issue_management_system_new.dbo.issue_occurrence.issue_issue_ID = issue_management_system_new.dbo.issues.issue_id
                                 WHERE issue_occurrence.location IN (@location)
                                 AND issue_occurrence.issue_date BETWEEN @startDate AND @endDate
                                 ORDER BY DateDiff DESC";

                    var chart1Data = db.Database.SqlQuery<TempClasses.tempClass2>(query,
                        new SqlParameter("@location", plantLocation),
                        new SqlParameter("@startDate", startDate),
                        new SqlParameter("@endDate", endDate)).ToList();

                    return Json(chart1Data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult FillChart3(string startDate, string endDate, string plantLocation)
        {
            try
            {
                using (var db = new issue_management_systemEntities1())
                {
                    string query = @"SELECT TOP 10 issues.issue, COUNT(issue_issue_ID) AS 'count'
                                     FROM issue_occurrence
                                     JOIN issues ON issues.issue_id = issue_occurrence.issue_issue_ID
                                     WHERE issue_occurrence.location IN (@location)
                                     AND issue_date BETWEEN @startDate AND @endDate 
                                     GROUP BY issues.issue";

                    var chart1Data = db.Database.SqlQuery<TempClasses.tempClass3>(query,
                        new SqlParameter("@location", plantLocation),
                        new SqlParameter("@startDate", startDate),
                        new SqlParameter("@endDate", endDate)).ToList();

                    return Json(chart1Data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult loadIssueList()
        {
            try
            {
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    String query1 = @"..."; // Your original query here
                    String query2 = @"..."; // Your original query here

                    var cmd = db.Database.Connection.CreateCommand();
                    cmd.Connection.Open();

                    DataTable dt1 = new DataTable();
                    DataTable dt2 = new DataTable();

                    try
                    {
                        cmd.CommandText = query1;
                        dt1.Load(cmd.ExecuteReader());

                        cmd.CommandText = query2;
                        dt2.Load(cmd.ExecuteReader());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error executing SQL commands: " + ex.Message);
                        return Json(new { success = false, message = "An error occurred while fetching issue lists." }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        cmd.Connection.Close();
                    }

                    List<TempClasses.tempClass5> dt1_data = new List<TempClasses.tempClass5>();

                    // Process dt1 and dt2 rows here, with similar error handling in each part

                    return Json(dt1_data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading issue list: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while loading the issue list." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult notificationOnOff(string issue_line_person_id, string issue_occurrence_id, string status)
        {
            try
            {
                String query = @"UPDATE notification_handling 
                            SET notification_handling.notification_status=" + status + @"
                            WHERE notification_handling.issue_occurrence_id='" + issue_occurrence_id + @"' 
                            AND notification_handling.EmployeeNumber='" + issue_line_person_id + @"'";

                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    db.Database.ExecuteSqlCommand(query);
                }
                return Json(new { success = true, message = "Changes Updated" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating notification status: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while updating the notification status." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult checkNotificationList_formanagers(int empID, string issueOccID)
        {
            try
            {
                String query = @"SELECT CASE WHEN (" + empID + @" IN(SELECT notification_handling.EmployeeNumber 
                                  FROM notification_handling 
                                  WHERE notification_handling.issue_occurrence_id = " + issueOccID + @"  
                                  AND notification_handling.notification_status=1))
                            THEN CAST(1 AS BIT)
                            ELSE CAST(0 AS BIT) END";

                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    bool resultVar = db.Database.SqlQuery<bool>(query).Single();
                    return Json(resultVar, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking notification list: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while checking the notification list." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult totalNumberOfIssues(string startDate, string endDate, string plant)
        {
            try
            {
                String query = @"SELECT COUNT(issue_occurrence.issue_occurrence_id) AS issuesCount 
                            FROM issue_occurrence 
                            WHERE issue_occurrence.issue_date BETWEEN '" + startDate + @"' AND '" + endDate + @"'
                            AND issue_occurrence.location IN('" + plant + @"')";

                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    int resultVar = db.Database.SqlQuery<int>(query).Single();
                    return Json(resultVar, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error counting issues: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while counting issues." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult voiceDataFilter(String issue, String date)
        {
            try
            {
                String query = @"..."; // Your original query here
                List<String> resultVar;

                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    resultVar = db.Database.SqlQuery<String>(query).ToList();
                }

                return Json(resultVar, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error filtering voice data: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while filtering voice data." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}













/*using IssueManagementSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IssueManagementSystem.Controllers
{

    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult Index()
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                if ((string)Session["department"] == "MD")
                {
                    ViewBag.BrakedownCount = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 1).Count();
                    ViewBag.MaterialDelayCount = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 2).Count();
                    ViewBag.TechnicalIssue = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 3).Count();
                    ViewBag.QualityIsuue = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 4).Count();
                    ViewBag.ITIsuue = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 5).Count();
                }
                else
                {
                    string location = Session["location"].ToString();
                    ViewBag.BrakedownCount = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 1).Count();
                    ViewBag.MaterialDelayCount = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 2).Count();
                    ViewBag.TechnicalIssue = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 3).Count();
                    ViewBag.QualityIsuue = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 4).Count();
                    ViewBag.ITIsuue = db.issue_occurrence.Where(x => x.issue_satus == "1" && x.issue_issue_ID == 5).Count();
                }
                return View();
            }
        }

        public JsonResult filterSelectBoxes()
        {
            //Departments //Lines //Issues
            string query1 = @"SELECT DISTINCT deps.department_id,deps.department_name,
                                lines.line_id,lines.line_name,l_map.issues AS issues
                                FROM issue_management_system.dbo.departments deps , issue_management_system.dbo.lines lines,
                                issue_management_system.dbo.line_map l_map
                                WHERE lines.department_id = deps.department_id AND l_map.line_id = lines.line_id";
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {

                var d_obj1 = db.Database.SqlQuery<TempClasses.tempClass10>(query1).ToList();
                return Json(d_obj1);
            }
        }

        [HttpGet]
        public JsonResult GetNotification()
        {
            return Json(NotificaionService.GetNotification(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult hideNotification(int? notificationId)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                string query = "UPDATE tbl_Notifications SET Status = 0 WHERE NotificationId = " + notificationId;
                db.Database.ExecuteSqlCommand(query);
            }
            return Json(true);
        }

        //fill 
        [HttpPost]
        public JsonResult fillChart1(string barChart, string startDate, string endDate, string plantLocation)
        {
            var chart1Data = new List<TempClasses.tempClass>();
            var chart2Data = new List<TempClasses.tempClass4>();


            if (barChart.Equals("1"))
            {


                String query = @"SELECT TOP 10 issue_occurrence.machine_machine_id,
                                    count(issue_occurrence.machine_machine_id) AS count FROM issue_occurrence 
                                    WHERE issue_occurrence.issue_issue_ID = 1 AND issue_occurrence.location IN ('" + plantLocation + @"')
                                    AND  issue_occurrence.issue_date BETWEEN '" + startDate + @"' AND '" + endDate + @"' 
                                    GROUP BY issue_occurrence.machine_machine_id  ORDER BY count Desc";

                System.Diagnostics.Debug.Print(query);
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    chart1Data = db.Database.SqlQuery<TempClasses.tempClass>(query).ToList();
                }
                return Json(chart1Data, JsonRequestBehavior.AllowGet);
            }

            if (barChart.Equals("2"))
            {

                //String query = @"SELECT TOP 10 FLINTEC.dbo.FLINTEC$Item.[Search Description] AS Search_Description,
                //                count(issue_management_system.dbo.issue_occurrence.material_id) AS count 
                //                FROM issue_management_system.dbo.issue_occurrence,FLINTEC.dbo.FLINTEC$Item
                //                WHERE issue_occurrence.issue_issue_ID = 2
                //                AND issue_occurrence.location IN ('"+plantLocation+@"') AND 
                //                issue_management_system.dbo.issue_occurrence.material_id  COLLATE SQL_Latin1_General_CP1_CS_AS LIKE FLINTEC.dbo.FLINTEC$Item.No_ COLLATE SQL_Latin1_General_CP1_CS_AS
                //                AND issue_occurrence.issue_date BETWEEN '"+startDate+@"' AND '"+endDate+@"' 
                //                GROUP BY FLINTEC.dbo.FLINTEC$Item.[Search Description]
                //                ORDER BY count Desc";

                String query = @"SELECT TOP 10 ic.material_id AS Search_Description,
                                count(ic.material_id) AS count 
                                FROM issue_management_system.dbo.issue_occurrence ic 
                                WHERE ic.issue_issue_ID = 2
                                AND ic.location IN ('" + plantLocation + @"')
                                AND ic.issue_date BETWEEN '" + startDate + @"' AND '" + endDate + @"' 
                                GROUP BY ic.material_id
                                ORDER BY count Desc";

                try
                {

                    using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                    {
                        chart2Data = db.Database.SqlQuery<TempClasses.tempClass4>(query).ToList();
                    }

                }
                catch (Exception ex) { }

                foreach (var obj in chart2Data)
                {
                    //obj.Search_Description
                    using (FLINTEC_Context db1 = new FLINTEC_Context())
                    {
                        String query5 = @"SELECT TOP 1 f.[Search Description] AS Search_Description
                                        FROM  FLINTEC.dbo.FLINTEC$Item f
                                        WHERE f.No_ = '" + obj.Search_Description + "'";
                        var d3 = db1.Database.SqlQuery<String>(query5).FirstOrDefault();

                        obj.Search_Description = d3;
                    }
                }
                return Json(chart2Data, JsonRequestBehavior.AllowGet);
            }
            return Json(chart1Data, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult fillChart2(string startDate, string endDate, string plantLocation)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                String query = @"SELECT TOP 10
                                issue_management_system.dbo.issue_occurrence.issue_date,
                                issue_management_system.dbo.issues.issue,
                                issue_management_system.dbo.issue_occurrence.machine_machine_id,
                                issue_management_system.dbo.issue_occurrence.material_id,
                                BigRed.dbo.tbl_PPA_User.Name,
                                DATEDIFF(minute, issue_occurrence.issue_date, issue_occurrence.solved_date) AS DateDiff
                                FROM
                                issue_management_system.dbo.issue_occurrence,BigRed.dbo.tbl_PPA_User,issue_management_system.dbo.issues
                                WHERE issue_occurrence.location IN ('" + plantLocation + @"') AND 
                                BigRed.dbo.tbl_PPA_User.UserName LIKE issue_management_system.dbo.issue_occurrence.responsible_person_emp_id AND
                                issue_management_system.dbo.issue_occurrence.issue_issue_ID = issue_management_system.dbo.issues.issue_id 
                                AND issue_occurrence.issue_date BETWEEN '" + startDate + @"' AND '" + endDate + @"' 
                                ORDER BY DateDiff DESC";

                var chart1Data = db.Database.SqlQuery<TempClasses.tempClass2>(query).ToList();
                return Json(chart1Data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult fillChart3(string startDate, string endDate, string plantLocation)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                String query = @"SELECT TOP 10 issues.issue , count(issue_issue_ID) AS 'count'
                                 FROM issue_occurrence,issues 
                                 WHERE issue_occurrence.location IN ('" + plantLocation + @"') AND  issue_date BETWEEN  '" + startDate + @"' AND '" + endDate + @"' 
                                 AND issues.issue_id = issue_occurrence.issue_issue_ID GROUP BY issue";

                var chart1Data = db.Database.SqlQuery<TempClasses.tempClass3>(query).ToList();
                return Json(chart1Data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult loadIssueList()
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                String query = @"SELECT(lines.department_id)AS dep_occured,issues.issue,lines.line_name,f.issue_occurrence_id,issue_date,g.Name,
                                CONCAT(FLINTEC.dbo.FLINTEC$Item.No_,'  ',FLINTEC.dbo.FLINTEC$Item.Description) AS material_id,description,
                                machine_machine_id,line_line_id,issue_issue_ID,issue_satus,f.location,responsible_person_confirm_status,
                                responsible_person_confirm_feedback,solved_date,commented_date,f.department,
                                (SELECT  a.Name FROM BigRed.dbo.tbl_PPA_User a  WHERE a.UserName LIKE f.solved_emp_id ) AS solved_emp,
                                (SELECT  i.Name FROM BigRed.dbo.tbl_PPA_User i WHERE i.UserName LIKE f.buzzer_off_by  ) AS buzzer_off_by
                                ,buzzer_off_time,job_card

                                FROM 

                                issue_occurrence f,lines,issues,FLINTEC.dbo.FLINTEC$Item,BigRed.dbo.tbl_PPA_User g

                                WHERE

                                lines.line_id = f.line_line_id AND issues.issue_id = f.issue_issue_ID AND
                                (FLINTEC.dbo.FLINTEC$Item.No_ COLLATE SQL_Latin1_General_CP1_CS_AS LIKE f.material_id COLLATE SQL_Latin1_General_CP1_CS_AS)
                                AND g.UserName LIKE f.responsible_person_emp_id
 
                                UNION 

                                SELECT(lines.department_id)AS dep_occured,issues.issue,lines.line_name,e.issue_occurrence_id,issue_date,h.Name,
                                (NULL) AS material_id,description,machine_machine_id,line_line_id,issue_issue_ID,issue_satus,
                                e.location,responsible_person_confirm_status,responsible_person_confirm_feedback,solved_date,commented_date,
                                e.department,
                                (SELECT  c.Name FROM BigRed.dbo.tbl_PPA_User c WHERE c.UserName LIKE e.solved_emp_id  ) AS solved_emp,
                                (SELECT  i.Name FROM BigRed.dbo.tbl_PPA_User i WHERE i.UserName LIKE e.buzzer_off_by  ) AS buzzer_off_by
                                ,buzzer_off_time,job_card

                                FROM

                                issue_occurrence e,lines,issues,BigRed.dbo.tbl_PPA_User h
                                WHERE

                                lines.line_id = e.line_line_id AND issues.issue_id = e.issue_issue_ID AND 
                                h.UserName LIKE e.responsible_person_emp_id AND
                                e.material_id IS NULL ORDER BY issue_date DESC";

                String query1 = @"SELECT
                                issue_date,f.issue_occurrence_id,
                                issues.issue,lines.line_name,
                                issue_satus,f.location,
                                description,g.Name,
                                ''+material_id AS material_id,machine_machine_id,
                                line_line_id,issue_issue_ID,
                                responsible_person_confirm_status,responsible_person_confirm_feedback,
                                solved_date,commented_date,
                                f.department,(SELECT  a.Name FROM BigRed.dbo.tbl_PPA_User a  WHERE a.UserName LIKE f.solved_emp_id ) AS solved_emp,
                                (SELECT  i.Name FROM BigRed.dbo.tbl_PPA_User i WHERE i.UserName LIKE f.buzzer_off_by  ) AS buzzer_off_by,buzzer_off_time,
                                (lines.department_id)AS dep_occured,job_card

                                FROM 

                                issue_occurrence f,lines,issues,BigRed.dbo.tbl_PPA_User g

                                WHERE

                                lines.line_id = f.line_line_id AND issues.issue_id = f.issue_issue_ID
                                AND g.UserName LIKE f.responsible_person_emp_id AND f.issue_issue_ID = 2";



                String query2 = @"SELECT
                                issue_date,e.issue_occurrence_id,
                                issues.issue,lines.line_name,
                                issue_satus,e.location,
                                description,h.Name,
                                ''+material_id AS material_id,machine_machine_id,
                                line_line_id,issue_issue_ID,
                                responsible_person_confirm_status,responsible_person_confirm_feedback,
                                solved_date,commented_date,
                                e.department,(SELECT  a.Name FROM BigRed.dbo.tbl_PPA_User a  WHERE a.UserName LIKE e.solved_emp_id ) AS solved_emp,
                                (SELECT  i.Name FROM BigRed.dbo.tbl_PPA_User i WHERE i.UserName LIKE e.buzzer_off_by  ) AS buzzer_off_by,buzzer_off_time,
                                (lines.department_id)AS dep_occured,job_card

                                FROM

                                issue_occurrence e,lines,issues,BigRed.dbo.tbl_PPA_User h
                                WHERE

                                lines.line_id = e.line_line_id AND issues.issue_id = e.issue_issue_ID AND 
                                h.UserName LIKE e.responsible_person_emp_id AND
                                e.material_id IS NULL ORDER BY issue_date DESC";

                //List<TempClasses.tempClass5> data = db.Database.SqlQuery<TempClasses.tempClass5>(query).ToList();
                var cmd = db.Database.Connection.CreateCommand();
                cmd.Connection.Open();

                cmd.CommandText = query1;
                DataTable dt1 = new DataTable();
                dt1.Load(cmd.ExecuteReader());

                cmd.CommandText = query2;
                DataTable dt2 = new DataTable();
                dt2.Load(cmd.ExecuteReader());

                //DataRow[] check_val = dt1.Select("issue_occurrence_id = '"+5610+"'");

                dt1.Columns["material_id"].ReadOnly = false;
                dt1.Columns["material_id"].MaxLength = 100;
                List<TempClasses.tempClass5> dt1_data = new List<TempClasses.tempClass5>();


                foreach (DataRow row in dt1.Rows)
                {
                    using (FLINTEC_Context db1 = new FLINTEC_Context())
                    {
                        String query3 = "SELECT FLINTEC.dbo.FLINTEC$Item.Description FROM  FLINTEC.dbo.FLINTEC$Item WHERE FLINTEC.dbo.FLINTEC$Item.No_ = '" + (string)row["material_id"] + "'";
                        var cmd1 = db1.Database.Connection.CreateCommand();
                        cmd1.CommandText = query3;
                        cmd1.Connection.Open();
                        DbDataReader reader = cmd1.ExecuteReader();
                        while (reader.Read())
                        {
                            row.SetField("material_id", (string)row["material_id"] + " - " + reader[0]);
                        }

                        cmd1.Connection.Close();
                    }

                    var data_object = new TempClasses.tempClass5();
                    data_object.issue_date = (DateTime)row["issue_date"];//"issue_date"
                    data_object.issue_occurrence_id = (int)row["issue_occurrence_id"];//"issue_occurrence_id"
                    data_object.issue = (string)row["issue"];//"issue"
                    data_object.line_name = (string)row["line_name"];//"line_name"
                    data_object.issue_satus = (string)row["issue_satus"];//"issue_satus"
                    data_object.location = (string)row["location"];//"location"
                    data_object.description = DBNull.Value.Equals(row["description"]) ? null : (string)row["description"];//"description"
                    data_object.Name = (string)row["Name"];//"Name"
                    data_object.material_id = DBNull.Value.Equals(row["material_id"]) ? null : (string)row["material_id"];//"material_id"
                    data_object.machine_machine_id = DBNull.Value.Equals(row["machine_machine_id"]) ? null : (string)row["machine_machine_id"];//"machine_machine_id"
                    data_object.line_line_id = (int)row["line_line_id"];//"line_line_id"
                    data_object.issue_issue_ID = (int)row["issue_issue_ID"];//"issue_issue_ID"
                    data_object.responsible_person_confirm_status = (int)row["responsible_person_confirm_status"];//"responsible_person_confirm_status"
                    data_object.responsible_person_confirm_feedback = DBNull.Value.Equals(row["responsible_person_confirm_feedback"]) ? null : (string)row["responsible_person_confirm_feedback"];//"responsible_person_confirm_feedback"
                    data_object.solved_date = DBNull.Value.Equals(row["solved_date"]) ? (DateTime?)null : (DateTime)row["solved_date"];//"solved_date"
                    data_object.commented_date = DBNull.Value.Equals(row["commented_date"]) ? (DateTime?)null : (DateTime)row["commented_date"];//"commented_date"
                    data_object.department = row["department"] != DBNull.Value ? (string)row["department"] : "";//"department"
                    data_object.solved_emp = DBNull.Value.Equals(row["solved_emp"]) ? null : (string)row["solved_emp"];//"solved_emp"
                    data_object.buzzer_off_by = DBNull.Value.Equals(row["buzzer_off_by"]) ? null : (string)row["buzzer_off_by"];//"buzzer_off_by"
                    data_object.buzzer_off_time = DBNull.Value.Equals(row["buzzer_off_time"]) ? (DateTime?)null : (DateTime)row["buzzer_off_time"];//"buzzer_off_time"
                    data_object.dep_occured = (int)row["dep_occured"];//"dep_occured"
                    data_object.job_card = DBNull.Value.Equals(row["job_card"]) ? null : (string)row["job_card"];//"job_card"

                    dt1_data.Add(data_object);
                }

                foreach (DataRow row in dt2.Rows)
                {

                    var data_object = new TempClasses.tempClass5();
                    data_object.issue_date = (DateTime)row["issue_date"];//"issue_date"
                    data_object.issue_occurrence_id = (int)row["issue_occurrence_id"];//"issue_occurrence_id"
                    data_object.issue = (string)row["issue"];//"issue"
                    data_object.line_name = (string)row["line_name"];//"line_name"
                    data_object.issue_satus = (string)row["issue_satus"];//"issue_satus"
                    data_object.location = (string)row["location"];//"location"
                    data_object.description = DBNull.Value.Equals(row["description"]) ? null : (string)row["description"];//"description"
                    data_object.Name = (string)row["Name"];//"Name"
                    data_object.material_id = DBNull.Value.Equals(row["material_id"]) ? null : (string)row["material_id"];//"material_id"
                    data_object.machine_machine_id = DBNull.Value.Equals(row["machine_machine_id"]) ? null : (string)row["machine_machine_id"];//"machine_machine_id"
                    data_object.line_line_id = (int)row["line_line_id"];//"line_line_id"
                    data_object.issue_issue_ID = (int)row["issue_issue_ID"];//"issue_issue_ID"
                    data_object.responsible_person_confirm_status = (int)row["responsible_person_confirm_status"];//"responsible_person_confirm_status"
                    data_object.responsible_person_confirm_feedback = DBNull.Value.Equals(row["responsible_person_confirm_feedback"]) ? null : (string)row["responsible_person_confirm_feedback"];//"responsible_person_confirm_feedback"
                    data_object.solved_date = DBNull.Value.Equals(row["solved_date"]) ? (DateTime?)null : (DateTime)row["solved_date"];//"solved_date"
                    data_object.commented_date = DBNull.Value.Equals(row["commented_date"]) ? (DateTime?)null : (DateTime)row["commented_date"];//"commented_date"
                    data_object.department = row["department"] != DBNull.Value ? (string)row["department"] : "";//"department"
                    data_object.solved_emp = DBNull.Value.Equals(row["solved_emp"]) ? null : (string)row["solved_emp"];//"solved_emp"
                    data_object.buzzer_off_by = DBNull.Value.Equals(row["buzzer_off_by"]) ? null : (string)row["buzzer_off_by"];//"buzzer_off_by"
                    data_object.buzzer_off_time = DBNull.Value.Equals(row["buzzer_off_time"]) ? (DateTime?)null : (DateTime)row["buzzer_off_time"];//"buzzer_off_time"
                    data_object.dep_occured = (int)row["dep_occured"];//"dep_occured"
                    data_object.job_card = DBNull.Value.Equals(row["job_card"]) ? null : (string)row["job_card"];//"job_card"

                    dt1_data.Add(data_object);
                }
                //dt1_data.Sort("Column_name desc");
                cmd.Connection.Close();
                return Json(dt1_data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult notificationOnOff(string issue_line_person_id, string issue_occurrence_id, string status)
        {

            *//*
              String query = @"UPDATE c SET  c.
              = 0,c.call = 0,c.message = 0 FROM issue_line_person AS c ,issues,lines
                  WHERE c.assigned_date = (
                  SELECT MAX(d.assigned_date)
                  FROM issue_line_person d,lines,issues 
                  WHERE issues.issue_id = d.issue_id 
                  AND lines.line_id = d.line_id
                  AND lines.line_name ='"+line+@"' AND issues.issue = '"+issue+ @"' AND d.issue_line_person_id='"+issue_line_person_id+@"'
                  )AND lines.line_name ='" + line+@"' AND issues.issue ='"+issue+ @"' AND c.issue_line_person_id='"+issue_line_person_id+@"'";
            *//*


            String query = @"UPDATE notification_handling 
                            SET 
                            notification_handling.notification_status=" + status + @"
                            WHERE 
                            notification_handling.issue_occurrence_id='" + issue_occurrence_id + @"' AND 
                            notification_handling.EmployeeNumber='" + issue_line_person_id + @"'";

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                db.Database.ExecuteSqlCommand(query);
            }

            return Json("Changes Updated", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult checkNotificationList_formanagers(int empID, string issueOccID)
        {


            String query = @"SELECT CASE WHEN
                            (" + empID + @" IN(SELECT notification_handling.EmployeeNumber 
                                  FROM notification_handling 
                                  WHERE notification_handling.issue_occurrence_id = " + issueOccID + @"  
                                  AND notification_handling.notification_status=1))
                            THEN CAST(1 AS BIT)
                            ELSE CAST(0 AS BIT) END";
            Boolean resultVar;

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                resultVar = db.Database.SqlQuery<Boolean>(query).Single();
            }
            return Json(resultVar, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult totalNumberOfIssues(string startDate, string endDate, string plant)
        {

            String query = @"SELECT COUNT(issue_occurrence.issue_occurrence_id) AS issuesCount 
                            FROM issue_occurrence 
                            WHERE issue_occurrence.issue_date BETWEEN '" + startDate + @"' AND '" + endDate + @"'
                            AND issue_occurrence.location IN('" + plant + @"')";

            int resultVar;

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                resultVar = db.Database.SqlQuery<int>(query).Single();
            }
            return Json(resultVar, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult voiceDataFilter(String issue, String date)
        {

            String query = @"";
            List<String> resultVar;

            System.Diagnostics.Debug.Print("issue:" + issue + " date:" + date);

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                resultVar = db.Database.SqlQuery<String>(query).ToList();
            }

            return Json(resultVar, JsonRequestBehavior.AllowGet);
        }
    }
}*/