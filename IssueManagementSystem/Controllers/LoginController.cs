using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IssueManagementSystem.Models;

namespace IssueManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        tbl_PPA_User authorized_user = new tbl_PPA_User();

        // GET: Login
        public ActionResult Index()
        {
            HttpCookie cookies_clients = Request.Cookies["login_data"];
            if (cookies_clients != null)
            {
                tbl_PPA_User obj = new tbl_PPA_User();
                obj.EmployeeNumber = Convert.ToInt32(cookies_clients["uid"]);
                obj.Password = cookies_clients["pwd"];

                ActionResult ar = Authorize(obj);
                return ar;
            }
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Authorize(tbl_PPA_User userModel)
        {
            try
            {
                using (BigRedEntities db = new BigRedEntities())
                {
                    HttpCookie cookie = new HttpCookie("login_data")
                    {
                        Expires = DateTime.Now.AddDays(30)
                    };

                    cookie["uid"] = userModel.EmployeeNumber.ToString();
                    cookie["pwd"] = userModel.Password;

                    var userDetails = db.tbl_PPA_User
                        .FirstOrDefault(x => x.EmployeeNumber == userModel.EmployeeNumber && x.Password == userModel.Password);

                    if (userDetails == null)
                    {
                        userModel.LoginErrorMessage = "Wrong Employee Number or Password."; // show login error message
                        return View("Index", userModel);
                    }
                    else
                    {
                        // Set session variables
                        SetSessionVariables(userDetails);

                        Response.Cookies.Add(cookie);

                        string username = Session["userName"].ToString();
                        string role = userDetails.Role.ToString().Trim(); // retrieve the user role

                        if (role.Equals("supervisor")) // if user is supervisor goto the supervisor page
                        {
                            SetSupervisorLine(userDetails);
                            return RedirectToAction("selectIssue", "Supervisor", new { lineid = Session["lineId"] });
                        }
                        else if (role.Equals("CellEngineer"))
                        {
                            SetCellEngineerLine(userDetails);
                            return RedirectToAction("DashBord", "CellEngineer", new { lineid = Session["lineId"] });
                        }
                        else if (role.Equals("display")) // if user is display goto the display page
                            return RedirectToAction("Rasp", "Display");
                        else if (role.Equals("admin"))
                            return RedirectToAction("Index", "Admin");
                        else if (role.Equals("manager"))
                            return RedirectToAction("Index", "Manager");
                        else if (role.Equals("responsiblePerson"))
                            return RedirectToAction("Index", "ResponsiblePerson");
                        else
                            return RedirectToAction("Index", "Login");
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "Error during user authorization.");
                userModel.LoginErrorMessage = "An error occurred during login. Please try again later.";
                return View("Index", userModel);
            }
        }

        private void SetSessionVariables(tbl_PPA_User userDetails)
        {
            Session["userID"] = userDetails.EmployeeNumber; // retrieve User ID of logged-in user
            Session["userName"] = userDetails.UserName.Trim(); // retrieve User Name of logged-in user
            Session["name"] = userDetails.Name.Trim();
            Session["email"] = userDetails.EMail.Trim();
            Session["location"] = userDetails.Location.Trim();
            Session["department"] = userDetails.Department.Trim();
            Session["Role"] = userDetails.Role.Trim();
        }

        private void SetSupervisorLine(tbl_PPA_User userDetails)
        {
            using (issue_management_systemEntities1 dbism = new issue_management_systemEntities1())
            {
                var lineInfo = dbism.line_supervisor.FirstOrDefault(x => x.supervisor_emp_id == userDetails.EmployeeNumber);
                if (lineInfo != null)
                {
                    Session["lineId"] = lineInfo.line_line_id;
                }
                else
                {
                    throw new Exception("Line information for the supervisor not found.");
                }
            }
        }

        private void SetCellEngineerLine(tbl_PPA_User userDetails)
        {
            using (issue_management_systemEntities1 dbism = new issue_management_systemEntities1())
            {
                var lineInfo = dbism.line_cell_eng.FirstOrDefault(x => x.cell_eng_emp_id == userDetails.EmployeeNumber);
                if (lineInfo != null)
                {
                    Session["lineId"] = lineInfo.line_id;
                }
                else
                {
                    throw new Exception("Line information for the Cell Engineer not found.");
                }
            }
        }

        public ActionResult LogOut()
        {
            try
            {
                Session.Abandon();
                if (Response.Cookies["login_data"] != null)
                {
                    Response.Cookies["login_data"].Expires = DateTime.Now.AddDays(-1);
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                LogException(ex, "Error during logout.");
                return RedirectToAction("Index", "Login"); // Redirect even on error to avoid blocking
            }
        }

        private void LogException(Exception ex, string additionalInfo = "")
        {
            // Log the exception with additional context information.
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}. {additionalInfo} StackTrace: {ex.StackTrace}");
            // Optionally, log to a file, external logging service, or database.
        }
    }
}
