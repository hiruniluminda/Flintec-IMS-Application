﻿using IssueManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IssueManagementSystem.Controllers
{
    public class MonthlySummaryVDController : Controller
    {
        private MonthlySummaryVDContext _context = new MonthlySummaryVDContext();

        // GET: MonthlySummaryVD
        public ActionResult Index()
        {
            var MonthlySummaryVD = _context.MonthlySummaryVD.ToList();
            return View();
        }
    }
}