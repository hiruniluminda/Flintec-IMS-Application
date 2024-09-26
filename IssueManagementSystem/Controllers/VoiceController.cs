using IssueManagementSystem.Models;
using System;
using System.Web.Mvc;

namespace IssueManagementSystem.Controllers
{
    public class VoiceController : Controller
    {
        [HttpGet]
        public JsonResult GetIssues()
        {
            try
            {
                var issues = IssueService.GetIssue();
                return Json(issues, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging purposes
                System.Diagnostics.Debug.WriteLine("Error in GetIssues: " + ex.Message);

                // Return a JSON response with error information
                return Json(new { success = false, message = "An error occurred while retrieving issues.", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
