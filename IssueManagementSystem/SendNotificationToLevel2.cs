﻿using IssueManagementSystem.Controllers;
using IssueManagementSystem.Models;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IssueManagementSystem
{
    public class SendNotificationToLevel2 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            System.Diagnostics.Debug.WriteLine("");

            try
            {
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    DateTime current = DateTime.Now;
                    var current_issue_list = db.issue_occurrence.Where(x => x.issue_satus == "1").ToList();
                    Queue numberList = new Queue();
                    Communication com = new Communication();

                    foreach (var items in current_issue_list)
                    {
                        try
                        {
                            DateTime issueDate = Convert.ToDateTime(items.issue_date);
                            TimeSpan span = current.Subtract(issueDate);
                            var DD = span.Days;
                            var HH = span.Hours;
                            double timeInHours = (DD * 24) + HH + (span.Minutes / 60.0);

                            // Wrap the communicationInfo query in a try-catch
                            List<issue_line_person> communicationInfo = null;
                            try
                            {
                                communicationInfo = db.issue_line_person
                                    .Where(x => x.line_id == items.line_line_id && x.issue_id == items.issue_issue_ID && x.person_level == 2)
                                    .OrderBy(x => x.levelOfResponsibility)
                                    .ToList();
                            }
                            catch (Exception ex)
                            {
                                LogException(ex, $"Error fetching communication info for Line ID: {items.line_line_id} and Issue ID: {items.issue_issue_ID}");
                                continue; // Skip this iteration if fetching communication info fails
                            }

                            foreach (var i in communicationInfo)
                            {
                                try
                                {
                                    int responseTime = Int32.Parse(i.sendAlertAfter);
                                    if (timeInHours >= responseTime)
                                    {
                                        var lineInfo = db.lines.FirstOrDefault(x => x.line_id == items.line_line_id);
                                        var issueInfo = db.issues.FirstOrDefault(x => x.issue_id == items.issue_issue_ID);
                                        var notificationHandlingInfo = db.notification_handling
                                            .FirstOrDefault(x => x.issue_occurrence_id == items.issue_occurrence_id && x.EmployeeNumber == i.EmployeeNumber);

                                        if (notificationHandlingInfo != null)
                                        {
                                            using (BigRedEntities BR = new BigRedEntities())
                                            {
                                                var responsiblePersonInfo = BR.tbl_PPA_User.FirstOrDefault(y => y.EmployeeNumber == items.responsible_person_emp_id);
                                                string msg = ConstructMessage(items, lineInfo, issueInfo, responsiblePersonInfo);

                                                if (notificationHandlingInfo.notification_status == 1)
                                                {
                                                    var personInfo = BR.tbl_PPA_User.FirstOrDefault(y => y.EmployeeNumber == i.EmployeeNumber);
                                                    CommunicationData cd = new CommunicationData(
                                                        personInfo.Phone, msg, personInfo.EMail, i.email, i.call, i.message,
                                                        personInfo.EmployeeNumber, "Unsolved Issue", $"{lineInfo.line_name} Line {issueInfo.issue1} not solved yet",
                                                        "0", "0", items.issue_occurrence_id);

                                                    numberList.Enqueue(cd);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException(ex, $"Error processing communication info for Employee: {i.EmployeeNumber}");
                                }
                            }

                            System.Diagnostics.Debug.WriteLine(DD);
                        }
                        catch (Exception ex)
                        {
                            LogException(ex, "Error processing issue occurrence: " + items.issue_occurrence_id);
                        }
                    }

                    if (numberList.Count > 0)
                    {
                        com.setCD(numberList);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "Error in Execute method of SendNotificationToLevel2.");
            }
        }

        private string ConstructMessage(issue_occurrence items, line lineInfo, issue issueInfo, tbl_PPA_User responsiblePersonInfo)
        {
            try
            {
                string msg = $"Good morning!@@ {issueInfo.issue1} issue is not resolved @ Line : {lineInfo.line_name} @ Responsible person : {responsiblePersonInfo.Name} @ Date : {items.issue_date} @ Thank You!";

                if (items.issue_issue_ID == 2)
                {
                    msg = $"Good morning!@@ Material delay is not resolved @ Line : {lineInfo.line_name} @ Material : {items.material_id} @ Responsible person : {responsiblePersonInfo.Name} @ Date : {items.issue_date} @ Thank You!";
                }
                if (items.issue_issue_ID == 1)
                {
                    msg = $"Good morning!@@ Machine Breakdown is not resolved @ Line : {lineInfo.line_name} @ Machine : {items.machine_machine_id} @ Responsible person : {responsiblePersonInfo.Name} @ Date : {items.issue_date} @ Thank You!";
                }

                return msg.Replace("@", Environment.NewLine);
            }
            catch (Exception ex)
            {
                LogException(ex, "Error constructing notification message.");
                return "Error constructing message";
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








/*using IssueManagementSystem.Controllers;
using IssueManagementSystem.Models;
using Quartz;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace IssueManagementSystem
{
    public class SendNotificationToLevel2 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            System.Diagnostics.Debug.WriteLine("");
            using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
            {
               DateTime current =DateTime.Now;
               var current_issue_list = db.issue_occurrence.Where(x => x.issue_satus == "1").ToList();
                Queue numberList = new Queue();
                Communication com = new Communication();
                foreach (var items in current_issue_list)
                {
                    DateTime issueDate = Convert.ToDateTime(items.issue_date);
                    TimeSpan span = current.Subtract(issueDate);
                    var DD = span.Days;
                    var MM = span.Minutes;
                    var HH = span.Hours;
                    double timeInHours = (DD * 24) + HH + (DD / 60.0);

                    var communicationInfo = db.issue_line_person.Where(x => x.line_id == items.line_line_id && x.issue_id == items.issue_issue_ID && x.person_level==2).OrderBy(x => x.levelOfResponsibility).ToList();
                    foreach (var i in communicationInfo)
                    {
                        int responceTime = Int32.Parse(i.sendAlertAfter) ;
                        if (timeInHours >= responceTime)
                        {
                           
                            var lineInfo=db.lines.Where(x=>x.line_id== items.line_line_id).FirstOrDefault();
                            var issueInfo=db.issues.Where(x=>x.issue_id== items.issue_issue_ID).FirstOrDefault();
                            var notificationHandalingInfo = db.notification_handling.Where(x => x.issue_occurrence_id == items.issue_occurrence_id && x.EmployeeNumber == i.EmployeeNumber).FirstOrDefault();
                            if (notificationHandalingInfo != null)
                            { 
                                using (BigRedEntities BR = new BigRedEntities())
                                {
                                    var responsiblePersonInfo = BR.tbl_PPA_User.Where(y => y.EmployeeNumber == items.responsible_person_emp_id).FirstOrDefault();
                                    string msg = "Good morning!@@ "+ issueInfo.issue1 + " issue is not resolved @ Line : " + lineInfo.line_name + "@ Responsible person : " + responsiblePersonInfo.Name + " @ Date : "+ items.issue_date + " @ Thank You!";
                                
                               
                                    if (items.issue_issue_ID == 2)
                                    {
                                    
                                        msg = "Good morning!@@ Material delay is not resolved @ Line : " + lineInfo.line_name + " @ Material : " + items.material_id + "@ Responsible person : " + responsiblePersonInfo.Name + "@ Date : " + items.issue_date + " @ Thank You!";


                                    }
                                    if (items.issue_issue_ID == 1)
                                    {
                                   
                                        msg = "Good morning!@@ Machine Breakdown is not resolved @ Line : " + lineInfo.line_name + " @ Machine : " + items.machine_machine_id + "@ Responsible person : " + responsiblePersonInfo.Name + "@ Date : " + items.issue_date + " @ Thank You!";
                                    
                                    }
                                    string callNote = lineInfo.line_name + " Line "+ issueInfo.issue1 + " not solved yet";
                              
                                    msg = msg.Replace("@", Environment.NewLine);
                                    if (notificationHandalingInfo.notification_status==1)
                                    {
                                     var personInfo = BR.tbl_PPA_User.Where(y => y.EmployeeNumber == i.EmployeeNumber).FirstOrDefault();
                                     CommunicationData cd = new CommunicationData(personInfo.Phone, msg, personInfo.EMail, i.email, i.call, i.message, personInfo.EmployeeNumber, "Unsolved Issue", callNote, "0", "0", items.issue_occurrence_id);
                                    numberList.Enqueue(cd);
                                    }

                                }
                            }
                        }
                    }
                   
                    System.Diagnostics.Debug.WriteLine(DD);
                }
                if (numberList != null)
                {
                    com.setCD(numberList);
                }
              
            }

        }
    }
}*/

