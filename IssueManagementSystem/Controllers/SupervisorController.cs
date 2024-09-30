using IssueManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using System.Net.Mime;
using Newtonsoft.Json.Linq;
using System.Collections;
using IssueManagementSystem.Models.IssueManagementSystem.Models;

namespace IssueManagementSystem.Controllers
{
    public class SupervisorController : Controller
    {

          
        




        Communication com = new Communication();
        // GET: Supervisor
        public ActionResult selectIssue(int lineid)// select issue view
        {
            ViewBag.lineid = lineid;

            if ((Session["userID"] == null) || ((string)Session["Role"] != "supervisor"))
            {
                return RedirectToAction("Index", "Login");

            }
        
            return View();
        }

        [HttpGet]
        public JsonResult GetIssues()
        {
            try
            {
                return Json(IssueService.GetIssue(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetBreakedown()
        {
            try
            {
                return Json(BreakeDownService.GetBreakedown(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MachinBreakdown(int lineid)//machine breakedown view
        {
            ViewBag.lineid = lineid;
            if ((Session["userID"] == null) || ((string)Session["Role"] != "supervisor"))
            {
                return RedirectToAction("Index", "Login");

            }
         
            return View();
        }
        public ActionResult QualityIssue(int lineid)//Qulity view
        {
            ViewBag.lineid = lineid;
            if ((Session["userID"] == null) || ((string)Session["Role"] != "supervisor"))
            {
                return RedirectToAction("Index", "Login");

            }
            return View();
        }

        public ActionResult TechnicalIssue(int lineid)//Technical Issue View
        {
            ViewBag.lineid = lineid;
            if ((Session["userID"] == null) || ((string)Session["Role"] != "supervisor"))
            {
                return RedirectToAction("Index", "Login");

            }
          
            return View();
        }

        public ActionResult MaterialDelay(int lineid)//MaterialDelay View
        {
            ViewBag.lineid = lineid;
            if ((Session["userID"] == null) || ((string)Session["Role"] != "supervisor"))
            {
                return RedirectToAction("Index", "Login");
            }

            dynamic jobCardsList = new System.Dynamic.ExpandoObject();
            try
            {
                using (FLINTEC_Prod_Order_Line_Context jobCardContext = new FLINTEC_Prod_Order_Line_Context())
                {
                    List<FLINTEC_Prod_Order_Line> jcList = jobCardContext.FLINTEC_Prod_Order_Line.Where(x => x.Status == 3).ToList();
                    jobCardsList.jobCards = jcList;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error fetching job cards: " + ex.Message;
            }

            return View(jobCardsList);
        }


        public ActionResult loadMaterialList(string selectedJobCard)
        {
            dynamic materials = new System.Dynamic.ExpandoObject();
            try
            {
                using (FLINTEC_Prod_Order_Component_Context jobCardContext = new FLINTEC_Prod_Order_Component_Context())
                {
                    List<FLINTEC_Prod_Order_Component> matList = jobCardContext.FLINTEC_Prod_Order_Component.Where(x => x.Prod_Order_No_.Equals(selectedJobCard)).ToList();
                    materials.materials = matList;
                }
            }
            catch (Exception ex)
            {
                materials = new { success = false, message = "Error loading materials: " + ex.Message };
            }

            return Json(materials);
        }
        public ActionResult ITIssue(int lineid)//IT Issue view
        {
            ViewBag.lineid = lineid;
            if ((Session["userID"] == null) || ((string)Session["Role"] != "supervisor"))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost] //add Breakedown to database
        public ActionResult AddIssueBreakedown(issue_occurrence issueModel, string issueJson, notification_handling notification_HandlingModel)
        {
            var time = DateTime.Now;
            string current_time = time.ToString("yyyy-MM-dd HH:mm");//get today to string variable
        
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                if (ModelState.IsValid)
                {
                    JArray issueData = JArray.Parse(issueJson) as JArray;
                    System.Diagnostics.Debug.WriteLine(issueData);

                    var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    var dayitem = current_time.Split(' ');
                    var day = dayitem[0];
                    var time1 = dayitem[1];
                    var HHMM = time1.Split(':');
                    try
                    {

                        foreach (JObject item in issueData)
                        {
                            int line_id = Int32.Parse(item["line_line_id"].ToString());
                            int issue_id = 1;
                            string group = item["group"].ToString();
                            var resp_person = db.issue_line_person.Where(x => x.levelOfResponsibility == 1 && x.line_id == line_id && x.issue_id == issue_id && x.person_level == 1).FirstOrDefault();
                            var responsible_person_emp_id = resp_person.EmployeeNumber; //get certain employee assigned for a certain issue in a certain line and the level of resp. should be one
                            issueModel.machine_machine_id = item["machine"].ToString();
                            issueModel.line_line_id = line_id;
                            issueModel.responsible_person_confirm_status = 1;
                            issueModel.department = "Maintenance";
                            issueModel.issue_satus = "1";
                            issueModel.loged_by = Int32.Parse(item["empId"].ToString());
                               
                            issueModel.issue_issue_ID = 1;//Issue id is 1 for Breakdown
                            issueModel.responsible_person_emp_id = resp_person.EmployeeNumber;
                            issueModel.location = item["location"].ToString();
                            issueModel.description = item["description"].ToString();
                            if (group != "") { issueModel.group = Int32.Parse(item["group"].ToString()); }
                            
                            issueModel.issue_date = date;
                            


                            string machine = issueModel.machine_machine_id;
                            var respPersonID = db.issue_line_person.Where(x => x.line_id == line_id && x.levelOfResponsibility == 1 && x.issue_id == 1 && x.person_level == 1).FirstOrDefault();
                            issueModel.responsible_person_emp_id = Int32.Parse(respPersonID.EmployeeNumber.ToString());
                            db.issue_occurrence.Add(issueModel);
                            db.SaveChanges();// end of the save

                            var issue_occour_id = issueModel.issue_occurrence_id;

                            if (issueModel.issue_occurrence_id > 0)
                            {
                                var line = db.lines.Where(x => x.line_id == line_id).FirstOrDefault();
                                string msg = " Breakdown has occurred.@@ Line : " + line.line_name + " Line @ Date : " + day + " @ Time : " + time1 + " @ Machine : " + machine + " @ Note : " + issueModel.description;
                              
                                string callNote = line.line_name + " line Breakdown has occurred at " + time1;
                                var displayInfo = db.displays.Where(x => x.line_id == line_id).FirstOrDefault();
                               
                                //turn on the Light
                                if (group == "") { com.lightON("1", displayInfo.raspberry_ip_address); 
                                }
                                else if (group != "") {
                                    com.lightONMachineshop("1", displayInfo.raspberry_ip_address, group );
                                    callNote = line.line_name + " Breakdown has occurred at " + time1;
                                    msg = " Breakdown has occurred.@@ Area : " + line.line_name + " @ Date : " + day + " @ Time : " + time1 + " @ Machine : " + machine + " @ Note : " + issueModel.description;

                                }


                               com.maintenancesbuzzerOn(item["location"].ToString());
                                msg = msg.Replace("@", Environment.NewLine);
                                ModelState.Clear();
                                sendCD(line_id, 1, msg, "Machine Breakdown has occurred", callNote, issue_occour_id, notification_HandlingModel);
                               
                            }
                            
                        }

                    }
                    catch (Exception ex)
                    {
                        return Content("Error Occured" + ex.ToString(), MediaTypeNames.Text.Plain);

                    }

                }
            }
            return Content("Breakedown Issue Recorded", MediaTypeNames.Text.Plain);
        }

        [HttpPost]//add Tecnical Issues to database
        public ActionResult AddIssueTechnical(string issueJson, issue_occurrence issueModel, notification_handling notification_HandlingModel)
        {
            var time = DateTime.Now;
            string current_time = time.ToString("yyyy-MM-dd HH:mm");

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                if (ModelState.IsValid)
                {
                    JArray issueData = JArray.Parse(issueJson) as JArray;
                    System.Diagnostics.Debug.WriteLine(issueData);

                    var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    var dayitem = current_time.Split(' ');
                    var day = dayitem[0];
                    var time1 = dayitem[1];
                    var HHMM = time1.Split(':');
                    try
                    {

                        foreach (JObject item in issueData)
                        {
                            int line_id = Int32.Parse(item["line_line_id"].ToString());
                            int issue_id = 3;
                            var resp_person = db.issue_line_person.Where(x => x.levelOfResponsibility == 1 && x.line_id == line_id && x.issue_id == issue_id && x.person_level == 1).FirstOrDefault();
                            var responsible_person_emp_id = resp_person.EmployeeNumber; //get certain employee assigned for a certain issue in a certain line and the level of resp. should be one
                            string group = item["group"].ToString();
                            issueModel.line_line_id = line_id;
                            issueModel.responsible_person_confirm_status = 1;
                            issueModel.department = "Engineering";
                            issueModel.machine_machine_id = item["machine"].ToString();
                            issueModel.issue_satus = "1";
                            issueModel.issue_issue_ID = 3;//Issue id is 3 for Tecnical
                            issueModel.responsible_person_emp_id = resp_person.EmployeeNumber;
                            issueModel.location = item["location"].ToString();
                            issueModel.description = item["description"].ToString();
                            issueModel.loged_by = Int32.Parse(item["empId"].ToString());
                            issueModel.issue_date = date;
                            if (group != "") { issueModel.group = Int32.Parse(item["group"].ToString()); }
                            db.issue_occurrence.Add(issueModel);
                            db.SaveChanges();// end of the save

                            if (issueModel.issue_occurrence_id > 0)
                            {
                                var issue_occour_id = issueModel.issue_occurrence_id;
                                var line = db.lines.Where(x => x.line_id == line_id).FirstOrDefault();
                                var displayInfo = db.displays.Where(x => x.line_id == line_id).FirstOrDefault();
                                string msg = " Technical issue has occurred @@ Line : " + line.line_name + " Line @ Date : " + day + " @ Time : " + HHMM[0] + ":" + HHMM[1] + "@ Special Note : " + issueModel.description;
                                string callNote = "Technical issue has occurred in " + line.line_name + " line at " + HHMM[0] + ":" + HHMM[1];
                                if (group == "") { com.lightON("3", displayInfo.raspberry_ip_address); }
                                else if (group != "") {
                                    msg = " Technical issue has occurred @@ Area  : " + line.line_name + " @ Date : " + day + " @ Time : " + HHMM[0] + ":" + HHMM[1] + "@ Special Note : " + issueModel.description;
                                    callNote = "Technical issue has occurred in " + line.line_name + " at " + HHMM[0] + ":" + HHMM[1];
                                    sendCD(line_id, 3, msg, "Tecnical Issue has occurred", callNote, issue_occour_id, notification_HandlingModel);
                                    com.lightONMachineshop("3", displayInfo.raspberry_ip_address, group);

                                }

                                msg = msg.Replace("@", Environment.NewLine);

                                ModelState.Clear();
                                //sendCD(line_id, 3, msg, "Tecnical Issue has occurred", callNote, issue_occour_id, notification_HandlingModel);
                               
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        return Content("Error Occured" + ex.ToString(), MediaTypeNames.Text.Plain);

                    }
                    

                }
            }
            return Content("Technical Issue Recorded", MediaTypeNames.Text.Plain);
        }
        [HttpPost]
        public ActionResult getServisorLins(int empId)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                var lineData = db.line_supervisor.Where(x => x.supervisor_emp_id == empId).ToList();


                return Json(lineData);
            }
        }
        [HttpPost]
        public ActionResult getServisorLinName(int lineid)
        {
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                string query = "SELECT line_name FROM lines WHERE line_id=" + lineid;
                var lineData = db.Database.SqlQuery<TempClasses.tempClass7>(query).ToList();
                return Json(lineData);
            }
        }
        [HttpPost]//add IT Issues to database
        public ActionResult AddIssueIT(string issueJson, issue_occurrence issueModel, notification_handling notification_HandlingModel)
        {
          
            var time = DateTime.Now;
            string current_time = time.ToString("yyyy-MM-dd HH:mm");

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                if (ModelState.IsValid)
                {
                    JArray issueData = JArray.Parse(issueJson) as JArray;
                    System.Diagnostics.Debug.WriteLine(issueData);


                    var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    var dayitem = current_time.Split(' ');
                    var day = dayitem[0];
                    var time1 = dayitem[1];
                    try
                    {
                        foreach (JObject item in issueData)
                        {
                            int line_id = Int32.Parse(item["line_line_id"].ToString());
                            int issue_id = 5;
                           
                            var resp_person = db.issue_line_person.Where(x => x.levelOfResponsibility == 1 && x.line_id == line_id && x.issue_id == issue_id && x.person_level == 1).FirstOrDefault();
                            var responsible_person_emp_id = resp_person.EmployeeNumber; //get certain employee assigned for a certain issue in a certain line and the level of resp. should be one
                             //save IT Issue to databse
                                issueModel.department = "IT";
                                issueModel.responsible_person_confirm_status = 1;
                                issueModel.line_line_id = line_id;
                                issueModel.issue_satus = "1";
                                issueModel.issue_issue_ID = 5;   //Issue id is 5 for IT Issue
                                issueModel.issue_date = date;
                                issueModel.responsible_person_emp_id = resp_person.EmployeeNumber;
                                issueModel.location = item["location"].ToString();
                                issueModel.description = item["description"].ToString();
                                issueModel.loged_by = Int32.Parse(item["empId"].ToString());
                            db.issue_occurrence.Add(issueModel);
                            db.SaveChanges();// end of the save
                                             //send notifications
                            var issue_occour_id = issueModel.issue_occurrence_id;
                                if (issueModel.issue_occurrence_id > 0)
                                {
                                    var line = db.lines.Where(x => x.line_id == line_id).FirstOrDefault();
                                    string msg = "IT/SoftWare issue has occurred @@ Line : " + line.line_name + " Line @ Date : " + day + "@ Time : " + time1 + "@ Note : " + issueModel.description;
                                    string callNote = "IT/Software issue has occurred in " + line.line_name + " line at " + time1;
                                    var displayInfo = db.displays.Where(x => x.line_id == line_id).FirstOrDefault();
                                    sendCD(line_id, 5, msg, "IT/Software Issue has occurred", callNote, issue_occour_id, notification_HandlingModel);

                                    com.lightON("5", displayInfo.raspberry_ip_address);//turn on the Light
                                    msg = msg.Replace("@", Environment.NewLine);

                                ModelState.Clear();
                                    //sendCD(line_id, 5, msg, "IT/Software Issue has occurred", callNote, issue_occour_id, notification_HandlingModel);
                                }
                            

                           
                        }
                    }
                    catch (Exception ex)
                    {
                        return Content("Error Occured" + ex.ToString(), MediaTypeNames.Text.Plain);
                    }
                }
            }
            return Content("IT Issue Recorded", MediaTypeNames.Text.Plain);
        }

        [HttpPost]//add Quality Issues to database
        public ActionResult AddIssueQuality(string issueJson, issue_occurrence issueModel, notification_handling notification_HandlingModel)
        {
            var time = DateTime.Now;
            string current_time = time.ToString("yyyy-MM-dd HH:mm");

            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
                if (ModelState.IsValid)
                {

                    JArray issueData = JArray.Parse(issueJson) as JArray;
                    System.Diagnostics.Debug.WriteLine(issueData);

                    var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    var dayitem = current_time.Split(' ');
                    var day = dayitem[0];
                    var time1 = dayitem[1];
                    var HHMM = time1.Split(':');
                    try
                    {

                        foreach (JObject item in issueData)
                        {
                            int line_id = Int32.Parse(item["line_line_id"].ToString());
                            int issue_id = 4;
                            var resp_person = db.issue_line_person.Where(x => x.levelOfResponsibility == 1 && x.line_id == line_id && x.issue_id == issue_id && x.person_level == 1).FirstOrDefault();
                            var responsible_person_emp_id = resp_person.EmployeeNumber; //get certain employee assigned for a certain issue in a certain line and the level of resp. should be one
                            string group = item["group"].ToString();
                            issueModel.line_line_id = line_id;
                            issueModel.responsible_person_confirm_status = 1;
                            issueModel.department = "Quality";
                            issueModel.issue_satus = "1";
                            issueModel.machine_machine_id = item["machine"].ToString();
                            issueModel.issue_issue_ID = 4;//Issue id is 4 for quality
                            issueModel.responsible_person_emp_id = resp_person.EmployeeNumber;
                            issueModel.location = item["location"].ToString();
                            issueModel.description = item["description"].ToString();
                            issueModel.group = Int32.Parse(item["group"].ToString());
                            issueModel.issue_date = date;
                            issueModel.loged_by = Int32.Parse(item["empId"].ToString());
                            db.issue_occurrence.Add(issueModel);
                            db.SaveChanges();// end of the save

                            if (issueModel.issue_occurrence_id > 0)
                            {
                                var issue_occour_id = issueModel.issue_occurrence_id;
                                var line = db.lines.Where(x => x.line_id == line_id).FirstOrDefault();
                                var displayInfo = db.displays.Where(x => x.line_id == line_id).FirstOrDefault();
                                string msg = " Quality issue has occurred @@ Area : "+ line.line_name + " @ Date : " + day + " @ Time : " + HHMM[0] + ":" + HHMM[1] + "@ Special Note : " + issueModel.description;
                                msg = msg.Replace("@", Environment.NewLine);
                                string callNote = "Quality issue has occurred in " + line.line_name + "at " + HHMM[0] + ":" + HHMM[1];
                                sendCD(line_id, 4, msg, "Quality Issue has occurred", callNote, issue_occour_id, notification_HandlingModel);

                                com.lightONMachineshop("4", displayInfo.raspberry_ip_address, group); 
                                
                                ModelState.Clear();
                                //sendCD(line_id, 4, msg, "Quality Issue has occurred", callNote, issue_occour_id, notification_HandlingModel);
                                
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        return Content("Error Occured" + ex.ToString(), MediaTypeNames.Text.Plain);

                    }
                }
            }
            return Content("Qulity Issue Recorded", MediaTypeNames.Text.Plain);
        }
        [HttpPost]
        public string getMachine(string machine)
        {
           
            using (schedulingEntities db = new schedulingEntities())
            {
                var location = db.Locations.Where(x => x.LocationName == machine).FirstOrDefault();
                var group = db.Locations.Where(x => x.LocationId == location.ParentId).FirstOrDefault();
                return group.LocationName;
            }
            
        }

        [HttpPost]//add Material Delay to database
        public ActionResult AddMaterialDelay(string issueJson, issue_occurrence issueModel ,notification_handling notification_HandlingModel)
         {
   
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1()) 
            {
                JArray issueData = JArray.Parse(issueJson) as JArray;
                System.Diagnostics.Debug.WriteLine(issueData);
                var time = DateTime.Now;
                string current_time = time.ToString("yyyy-MM-dd HH:mm");//get today to string variable
                var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                var dayitem = current_time.Split(' ');
                var day = dayitem[0];
                var time1 = dayitem[1];

                foreach (JObject item in issueData)
                {
                    
                    int line_id = Int32.Parse(item["line_line_id"].ToString());
                    int issue_id = Int32.Parse(item["issue_issue_ID"].ToString());
                    var resp_person = db.issue_line_person.Where(x => x.levelOfResponsibility == 1 && x.line_id == line_id && x.issue_id== issue_id && x.person_level == 1).FirstOrDefault();
                    try
                    {
                        var responsible_person_emp_id = resp_person.EmployeeNumber;
                        string group = item["group"].ToString();

                        if (ModelState.IsValid)
                        {

                            issueModel.responsible_person_confirm_status = 1;
                            issueModel.line_line_id = line_id;
                            issueModel.issue_satus = "1";
                            issueModel.issue_issue_ID = 2;
                            issueModel.issue_date = date;
                            issueModel.department = "Stores";
                            issueModel.machine_machine_id = item["machine"].ToString();
                            issueModel.job_card = item["jobcard"].ToString();
                            issueModel.material_id = item["material_id"].ToString();
                            issueModel.responsible_person_emp_id = resp_person.EmployeeNumber;
                            issueModel.location = item["location"].ToString();
                            issueModel.loged_by = Int32.Parse(item["empId"].ToString());
                            issueModel.description = item["description"].ToString();
                            if (group != "") { issueModel.group = Int32.Parse(item["group"].ToString()); }
                            db.issue_occurrence.Add(issueModel);
                            db.SaveChanges();// end of the save
                     
                       
                            var issue_occour_id = issueModel.issue_occurrence_id;
                            
                          System.Diagnostics.Debug.WriteLine("New Issue ID" + issue_occour_id);

                            if (issueModel.issue_occurrence_id > 0)
                            {
                                var displayInfo = db.displays.Where(x => x.line_id == line_id).FirstOrDefault();
                                var line = db.lines.Where(x => x.line_id == line_id).FirstOrDefault();
                                string msg = "MaterialDelay has occurred @@ Line : " + line.line_name + " Line @ Date :" + day + " @ Time : " + time1 + " @ JobCard :" + item["jobcard"] + " @ Material : " + item["material"] + " @ Note : " + item["description"];
                                string callNote = "MaterialDelay has occurred in " + line.line_name + " line at " + time1;
                                sendCD(line_id, 2, msg, "MaterialDelay has occurred", callNote, issue_occour_id, notification_HandlingModel);

                                if (group == "") { com.lightON("2", displayInfo.raspberry_ip_address); }
                                else if (group != "") {
                                    com.lightONMachineshop("2", displayInfo.raspberry_ip_address, group);
                                    callNote = "MaterialDelay has occurred in " + line.line_name + " at " + time1;
                                    msg = "MaterialDelay has occurred @@ Area : " + line.line_name + " @ Date :" + day + " @ Time : " + time1 + " @ JobCard :"+ item["jobcard"] + " @ Material : " + item["material"] + " @ Note : " + item["description"];

                                }
                                msg = msg.Replace("@", Environment.NewLine);

                                com.storesbuzzerOn(item["location"].ToString());
                                //sendCD(line_id, 2, msg, "MaterialDelay has occurred", callNote, issue_occour_id, notification_HandlingModel);
                            
                              
                                ModelState.Clear();
                            }
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        return Content("Error Occured" + ex.ToString(), MediaTypeNames.Text.Plain);
                    }
                }
                return Content("Material Delay Recorded", MediaTypeNames.Text.Plain);
            }
        }

        private void sendCD(int? line_line_id, int issueId, string msg, string subject, string callNote,int  issue_occour_id, notification_handling notification_HandlingModel)
        {
            var time = DateTime.Now;
            string current_time = time.ToString("yyyy-MM-dd HH:mm");
            var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            Thread t = new Thread(() =>
            {
                using (BigRedEntities db = new BigRedEntities())
                {
                    var userDetails = db.tbl_PPA_User.Where(x => x.Role == "manager").ToList();
                    using (issue_management_systemEntities1 ismdb = new issue_management_systemEntities1())
                    {
                        foreach (var item in userDetails)
                        {
                            string query = "INSERT INTO tbl_Notifications ([Status],[Message],[Type],[EmployeeNumber],[Date]) VALUES( 1,'" + msg + "','issue','" + item.EmployeeNumber + "','" + date + "') ";
                            ismdb.Database.ExecuteSqlCommand(query);
                        }
                    }
                }
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    try
                    {
                        var communicationInfo = db.issue_line_person.Where(x => x.line_id == line_line_id && x.issue_id == issueId).OrderBy(x => x.levelOfResponsibility).ToList();
                        Queue numberList = new Queue();
                        foreach (var item in communicationInfo)
                        {
                            if (item.person_level == 2)
                            {
                                notification_HandlingModel.issue_occurrence_id = issue_occour_id;
                                notification_HandlingModel.EmployeeNumber = item.EmployeeNumber;
                                notification_HandlingModel.notification_status = 1;
                                db.notification_handling.Add(notification_HandlingModel);
                                db.SaveChanges();
                            }
                            if (item.person_level == 1)
                            {
                                using (BigRedEntities BR = new BigRedEntities())
                                {

                                    var personInfo = BR.tbl_PPA_User.Where(y => y.EmployeeNumber == item.EmployeeNumber).FirstOrDefault();
                                    CommunicationData cd = new CommunicationData(personInfo.Phone, msg, personInfo.EMail, item.email, item.call, item.message, personInfo.EmployeeNumber, subject, callNote, item.callRepetitionTime, item.sendAlertAfter, issue_occour_id);
                                    numberList.Enqueue(cd);

                                }
                            }


                        }


                        com.setCD(numberList);
                    }
                    catch (Exception e) { }
                }
            });
            t.Start();
        }

        //solovedIssueMethod
        public JsonResult SolvedIssue(int? issueId, int? issueOccourId ,int? lineId,int? group)
        {
            System.Diagnostics.Debug.WriteLine("issueOccourId : " + issueOccourId);
            var time = DateTime.Now;
            string current_time = time.ToString("yyyy-MM-dd HH:mm");
            var date = DateTime.ParseExact(current_time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            //update Issueststus as 0
            issue_management_systemEntities1 db = new issue_management_systemEntities1();
            var issueoccourInfo = db.issue_occurrence.Where(x => x.issue_occurrence_id == issueOccourId).FirstOrDefault();
            issueoccourInfo.issue_satus = "0";
            issueoccourInfo.solved_date = date;
            db.SaveChanges();



            //get the list of Issuueoccurrence table
            var count = 0;
            if (group == 0)
            {
                count = db.issue_occurrence.Where(x => x.issue_issue_ID == issueId && x.line_line_id == lineId && x.issue_satus == "1").Count();
                if (count == 0)
                {
                    var displayInfo = db.displays.Where(x => x.line_id == lineId).FirstOrDefault();
                    com.lightOFF(issueId.ToString(), displayInfo.raspberry_ip_address);
                }
            }
            else {
                count = db.issue_occurrence.Where(x => x.issue_issue_ID == issueId && x.line_line_id == lineId && x.issue_satus == "1"&& x.group==group).Count();
                if (count == 0)
                {
                    var displayInfo = db.displays.Where(x => x.line_id == lineId).FirstOrDefault();
                    com.lightOFFMachineshop(issueId.ToString(), displayInfo.raspberry_ip_address,group.ToString());
                }
            }

        

           

           return Json(true);
        }
    }
}