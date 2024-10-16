using IssueManagementSystem.Models;
using System.Web.Mvc;
using System.Linq;

public class MonthlySummaryVDController : Controller
{
    private MonthlySummaryVDContext _context = new MonthlySummaryVDContext();

    // GET: MonthlySummaryVD
    public ActionResult MonthlySumVD()
    {
        var monthlySummaryVDList = _context.MonthlySummaryVD.ToList();
        return View(monthlySummaryVDList);  // Pass the list to the view
    }
}
