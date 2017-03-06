﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AHC.MailService;
using AHC.CD.WebUI.MVC.Models.ProviderViewModel.Shared;
using AHC.CD.Resources.Messages;
using System.Threading.Tasks;
using AHC.CD.Business.Email;
using Newtonsoft.Json;
using AHC.CD.WebUI.MVC.Models.EmailService;
using AHC.CD.Entities.EmailNotifications;
using AHC.CD.Exceptions;
using AHC.CD.ErrorLogging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using AHC.CD.WebUI.MVC.Models;
using System.Collections;
using AHC.CD.Business;
using AHC.CD.WebUI.MVC.Areas.Profile.Controllers;
using AHC.CD.Business.Search;
namespace AHC.CD.WebUI.MVC.Controllers
{

    public class EmailServiceController : Controller
    {
        private IEmailSender emailSender = null;
        private IEmailServiceManager emailServiceManager = null;
        private IErrorLogger errorLogger = null;
        private ISearchManager searchManager = null;
        protected ApplicationUserManager _authUserManager;
        protected ApplicationUserManager AuthUserManager
        {
            get
            {
                return _authUserManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _authUserManager = value;
            }
        }

        public EmailServiceController(IEmailSender emailSender, IErrorLogger errorLogger, IEmailServiceManager emailServiceManager, ISearchManager searchManager)
        {
            this.errorLogger = errorLogger;
            this.emailSender = emailSender;
            this.emailServiceManager = emailServiceManager;
            this.searchManager = searchManager;
        }

        // GET: EmailService

        public async Task<JsonResult> GetAllEmailIds()
        {
            try
            {
                var emails = await emailServiceManager.GetAllEmails();
                return Json(emails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorize(Roles = "ADM,CCO,CRA")]
        public async Task<ActionResult> Index()
        {
            //ViewBag.Emails = JsonConvert.SerializeObject(emailServiceManager.GetAllEmails());
            List<SearchUserforGroupMailDTO> searchResults = null;
            List<string> UserNames = new List<string>();
            var res = AuthUserManager.Users.ToList();
            res.ForEach(x => { var role = AuthUserManager.GetRoles(x.Id); if (!role.Contains("PRO")) UserNames.Add(x.FullName); });
            searchResults = await searchManager.GetAllUsersForGroupMailAsync();
            searchResults.ForEach(x => { res.ForEach(y => { x.FullName = y.Id == x.FullName ? y.FullName : x.FullName; }); });
            ViewBag.Searchdata = JsonConvert.SerializeObject(searchResults);
            return View();
        }
        
        public async Task<string> GetAllEmails()
        {
            var data = await emailServiceManager.GetAllEmailInfo();
            
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });
        }

        public async Task<string> GetAllEmailTemplates()
        {
            var data = await emailServiceManager.GetAllEmailTemplatesAsync();
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });
        }

        /// <summary>
        /// Method to fetch active emails with recurrence
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAllActiveFollowUpEmails()
        {
            var data = await emailServiceManager.GetAllActiveFollowUpEmailsAsync();
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });
        }

        /// <summary>
        /// Method to fetch recieved emails
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAllInboxEmails()
        {
            var data = await emailServiceManager.GetAllInboxEmailsAsync(await GetUserAuthId());
            return JsonConvert.SerializeObject(data);
        }

        public async Task<string> GetAllCDusers()
        {
            try
            {
                var result = await emailServiceManager.GetAllCDusersasync();
                return JsonConvert.SerializeObject(result, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region GroupMail
        [HttpGet]
        public string GetAllGroupMailNames()
        {
            try
            {
                var res = emailServiceManager.GetAllGroupMailNamesasync();
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        [HttpPost]
        public async Task<string> AddGroupEmail(EmailGroupViewModel groupemail)
        {
            string status = "true";
            EmailGroup datagroupemail = new EmailGroup();
            object obj = new object();
            string AuthId;
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, string> dict = null;
                    if (groupemail.EmailIds != "")
                    {
                        dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(groupemail.EmailIds);
                    }
                    datagroupemail = AutoMapper.Mapper.Map<EmailGroupViewModel, EmailGroup>(groupemail);
                    AuthId = await GetUserAuthId();
                    //var res = profilemanager.GetCDUserIdFromAuthId(AuthId);
                    obj = await emailServiceManager.AddNewGroupEmail(datagroupemail, dict, AuthId);
                }
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            return JsonConvert.SerializeObject(new { obj, status }, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        }
        [HttpPost]
        public async Task<string> UpdateGroupMail(EmailGroupViewModel groupemail)
        {
            try
            {
                string status = "";
                EmailGroup datagroupemail = new EmailGroup();
                //Dictionary<string, string> result = new Dictionary<string, string>();
                object result = null;
                string AuthId;
                try
                {
                    if (ModelState.IsValid)
                    {
                        Dictionary<string, string> dict = null;
                        if (groupemail.EmailIds != "")
                        {
                            dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(groupemail.EmailIds);
                        }
                        datagroupemail = AutoMapper.Mapper.Map<EmailGroupViewModel, EmailGroup>(groupemail);
                        AuthId = await GetUserAuthId();
                        result = await emailServiceManager.UpdateGroupMailasync(datagroupemail, dict, AuthId);
                        status = "true";
                    }
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                }
                return JsonConvert.SerializeObject(new { status, result });
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<string> GetAllGroupMails()
        {
            try
            {
                string AuthId = await GetUserAuthId();
                List<string> UserNames = new List<string>();
                var res = AuthUserManager.Users.ToList();
                res.ForEach(x => { var role = AuthUserManager.GetRoles(x.Id); if (!role.Contains("PRO")) UserNames.Add(x.FullName); });
                var result = await emailServiceManager.GetAllGroupMailsAsync(AuthId);
                result.ForEach(x => { x.GroupMailUserDetails.ForEach(y => { res.ForEach(z => { y.FullName = y.FullName == z.Id ? z.FullName : y.FullName; }); }); });
                return JsonConvert.SerializeObject(result, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<string> InactivateEmailGroup(int EmailGroupId)
        {
            try
            {
                string AuthId = await GetUserAuthId();
                var res = await emailServiceManager.InactivateEmailGroupAsync(EmailGroupId, AuthId);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<string> ActivateEmailGroup(int EmailGroupId)
        {
            try
            {
                string AuthId = await GetUserAuthId();
                var res = await emailServiceManager.ActivateEmailGroupAsync(EmailGroupId, AuthId);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<string> RemoveIndividualMailFromGroup(int Cduser_GroupMailId)
        {
            try
            {
                var res = await emailServiceManager.RemoveIndividualMailFromGroupAsync(Cduser_GroupMailId);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Method to stack an email to the DB
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ActionResult> AddEmail(EmailServiceViewModel email)
        {
            
                //EmailAttachmentViewModel EmailAttachment = new EmailAttachmentViewModel();
                //EmailAttachment.AttachmentServerPath = HttpContext.Server.MapPath(EmailAttachment.AttachmentRelativePath);
                //EmailAttachment.StatusType = AHC.CD.Entities.MasterData.Enums.StatusType.Active.ToString();
                //if (email.EmailAttachments == null)
                //{
                //    email.EmailAttachments = new List<EmailAttachmentViewModel>();
                //}
                //email.EmailAttachments.Add(EmailAttachment);
                         
            if (email.IntervalFactor == null)
            {
                email.IntervalFactor = 1;
            }
            EmailInfo dataEmailInfo = null;
            string status = "true";
            if (email.Body != null)
            {
                email.Body = email.Body.Replace("&lt;", "<");
                email.Body = email.Body.Replace("&gt;", ">");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    dataEmailInfo = AutoMapper.Mapper.Map<EmailServiceViewModel, EmailInfo>(email);
                    dataEmailInfo.SendingDate = DateTime.Now;
                    dataEmailInfo.From = System.Configuration.ConfigurationManager.AppSettings["ProductEmailID"];
                    if (dataEmailInfo.IsRecurrenceEnabled == Entities.MasterData.Enums.YesNoOption.YES.ToString())
                    {
                        dataEmailInfo.EmailRecurrenceDetail = new EmailRecurrenceDetail();
                        dataEmailInfo.EmailRecurrenceDetail = AutoMapper.Mapper.Map<EmailServiceViewModel, EmailRecurrenceDetail>(email);
                        dataEmailInfo.EmailRecurrenceDetail.IsRecurrenceScheduledYesNoOption = Entities.MasterData.Enums.YesNoOption.NO;
                        if (dataEmailInfo.EmailRecurrenceDetail.RecurrenceIntervalType == Entities.MasterData.Enums.RecurrenceIntervalType.Daily.ToString())
                        {
                            dataEmailInfo.EmailRecurrenceDetail.NextMailingDate = dataEmailInfo.SendingDate.Value.AddDays(dataEmailInfo.EmailRecurrenceDetail.IntervalFactor.Value);
                        }
                        else if (dataEmailInfo.EmailRecurrenceDetail.RecurrenceIntervalType == Entities.MasterData.Enums.RecurrenceIntervalType.Weekly.ToString())
                        {
                            dataEmailInfo.EmailRecurrenceDetail.NextMailingDate = dataEmailInfo.SendingDate.Value.AddDays(7 * dataEmailInfo.EmailRecurrenceDetail.IntervalFactor.Value);
                        }
                        else if (dataEmailInfo.EmailRecurrenceDetail.RecurrenceIntervalType == Entities.MasterData.Enums.RecurrenceIntervalType.Monthly.ToString())
                        {
                            dataEmailInfo.EmailRecurrenceDetail.NextMailingDate = dataEmailInfo.SendingDate.Value.AddMonths(dataEmailInfo.EmailRecurrenceDetail.IntervalFactor.Value);
                        }
                        else if (dataEmailInfo.EmailRecurrenceDetail.RecurrenceIntervalType == Entities.MasterData.Enums.RecurrenceIntervalType.Quarterly.ToString())
                        {
                            // dataEmailInfo.EmailRecurrenceDetail.IntervalFactor = 1;
                            dataEmailInfo.EmailRecurrenceDetail.NextMailingDate = dataEmailInfo.SendingDate.Value.AddMonths(4 * dataEmailInfo.EmailRecurrenceDetail.IntervalFactor.Value);
                        }
                        else if (dataEmailInfo.EmailRecurrenceDetail.RecurrenceIntervalType == Entities.MasterData.Enums.RecurrenceIntervalType.Yearly.ToString())
                        {
                            dataEmailInfo.EmailRecurrenceDetail.NextMailingDate = dataEmailInfo.SendingDate.Value.AddYears(dataEmailInfo.EmailRecurrenceDetail.IntervalFactor.Value);
                        }
                        else
                        {
                            dataEmailInfo.EmailRecurrenceDetail.NextMailingDate = email.DateForCustomRecurrence;
                        }
                    }

                    dataEmailInfo.EmailRecipients = new List<EmailRecipientDetail>();
                    List<string> toList = null;
                    //email.To = "testingsingh285@gmail.com";
                    toList = email.To.Split(';').ToList();
                    Dictionary<string, List<string>> Toemailids = await emailServiceManager.CheckGroupMailId(toList);
                    if (Toemailids != null && Toemailids.Count != 0)
                    {
                        foreach (var dictn in Toemailids)
                        {
                            toList.Remove(dictn.Key);
                            toList.AddRange(dictn.Value);
                        }
                    }
                    foreach (var to in toList)
                    {
                        if (to != "" && to != null)
                        {
                            EmailRecipientDetail recipient = new EmailRecipientDetail();
                            recipient.Recipient = to;
                            recipient.RecipientTypeCategory = Entities.MasterData.Enums.RecipientType.To;
                            recipient.StatusType = Entities.MasterData.Enums.StatusType.Active;
                            dataEmailInfo.EmailRecipients.Add(recipient);
                        }
                    }

                    List<string> ccList = null;
                    if (email.CC != null)
                    {
                        //email.CC = "testingsingh285@gmail.com";
                        ccList = email.CC.Split(';').ToList();
                        Dictionary<string, List<string>> CCemailids = await emailServiceManager.CheckGroupMailId(ccList);
                        if (CCemailids != null && CCemailids.Count != 0)
                        {
                            foreach (var dictn in CCemailids)
                            {
                                ccList.Remove(dictn.Key);
                                ccList.AddRange(dictn.Value);
                            }
                        }
                        foreach (var cc in ccList)
                        {
                            if (cc != "" && cc != null)
                            {
                                EmailRecipientDetail recipient = new EmailRecipientDetail();
                                recipient.Recipient = cc;
                                recipient.RecipientTypeCategory = Entities.MasterData.Enums.RecipientType.CC;
                                recipient.StatusType = Entities.MasterData.Enums.StatusType.Active;
                                dataEmailInfo.EmailRecipients.Add(recipient);
                            }
                        }
                    }

                    List<string> bccList = null;
                    if (email.BCC != null)
                    {
                        //email.BCC = "testingsingh285@gmail.com";
                        bccList = email.BCC.Split(';').ToList();
                        Dictionary<string, List<string>> BCCemailids = await emailServiceManager.CheckGroupMailId(bccList);
                        if (BCCemailids != null && BCCemailids.Count != 0)
                        {
                            foreach (var dictn in BCCemailids)
                            {
                                bccList.Remove(dictn.Key);
                                bccList.AddRange(dictn.Value);
                            }
                        }
                        foreach (var bcc in bccList)
                        {
                            if (bcc != ""&& bcc != null)
                            {
                                EmailRecipientDetail recipient = new EmailRecipientDetail();
                                recipient.Recipient = bcc;
                                recipient.RecipientTypeCategory = Entities.MasterData.Enums.RecipientType.BCC;
                                recipient.StatusType = Entities.MasterData.Enums.StatusType.Active;
                                dataEmailInfo.EmailRecipients.Add(recipient);
                            }
                        }
                    }

                     emailServiceManager.SaveComposedEmail(dataEmailInfo);
                }
            }
            catch (DatabaseValidationException ex)
            {
                errorLogger.LogError(ex);
                status = ex.ValidationError;
            }
            catch (ApplicationException ex)
            {
                errorLogger.LogError(ex);
                status = ex.Message;
            }
            catch (Exception ex)
            {
                errorLogger.LogError(ex);
                status = ExceptionMessage.EMAIL_SENT_EXCEPTION;
            }
            return Json(new { status = status, addedEmail = dataEmailInfo }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to stop a followup email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ActionResult> StopFollowUpEmail(int emailInfoID)
        {
            EmailInfo dataEmailInfo = null;
            string status = "true";
            try
            {
                dataEmailInfo = await emailServiceManager.StopFollowUpEmailAsync(emailInfoID);
            }
            catch (DatabaseValidationException ex)
            {
                errorLogger.LogError(ex);
                status = ex.ValidationError;
            }
            catch (ApplicationException ex)
            {
                errorLogger.LogError(ex);
                status = ex.Message;
            }
            catch (Exception ex)
            {
                errorLogger.LogError(ex);
                status = ExceptionMessage.EMAIL_FOLLOW_UP_STOP_EXCEPTION;
            }
            return Json(new { status = status, email = dataEmailInfo }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to stop a followup email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ActionResult> StopFollowUpEmailForSelectReceiversAsync(int emailInfoID, int recipientID)
        {
            EmailInfo dataEmailInfo = null;
            List<int> emailRecipientIDs = new List<int>();
            emailRecipientIDs.Add(recipientID);
            string status = "true";
            try
            {
                dataEmailInfo = await emailServiceManager.StopFollowUpEmailForSelectReceiversAsync(emailInfoID, emailRecipientIDs);
            }
            catch (DatabaseValidationException ex)
            {
                errorLogger.LogError(ex);
                status = ex.ValidationError;
            }
            catch (ApplicationException ex)
            {
                errorLogger.LogError(ex);
                status = ex.Message;
            }
            catch (Exception ex)
            {
                errorLogger.LogError(ex);
                status = ExceptionMessage.EMAIL_FOLLOW_UP_FOR_RECEIVERS_STOP_EXCEPTION;
            }
            return Json(new { status = status, email = dataEmailInfo }, JsonRequestBehavior.AllowGet);
        }

        public String Send(EmailViewModel emailViewModel)
        {
            // validate the view model
            // convert the view model to EMailModel

            string message;

            try
            {
                EMailModel emailModel = null;
                // emailModel = EmailTransformer.TransformToEmailModel(emailViewModel);
                emailSender.SendMail(emailModel);
                message = CompletionMessages.EMAIL_SENT_SUCCESSFULLY;
            }
            catch (Exception)
            {
                message = CompletionMessages.EMAIL_SENT_UNSUCCESSFULLY;

            }

            return message;

        }

        private async Task<string> GetUserAuthId()
        {
            var currentUser = HttpContext.User.Identity.Name;
            var appUser = new ApplicationUser() { UserName = currentUser };
            var user = await AuthUserManager.FindByNameAsync(appUser.UserName);

            return user.Id;
        }

    }
}