﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AHC.CD.WebUI.MVC.Areas.Credentialing.Controllers
{
    public class CCMPortalController : Controller
    {
        //
        // GET: /Credentialing/CCMPortal/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SPAIndexPage()
        {
            return PartialView("~/Areas/Credentialing/Views/CCMPortal/_SPAIndex.cshtml");
        }
        public ActionResult SPA_CCMAction()
        {
            return PartialView("~/Areas/Credentialing/Views/CCMPortal/_SPA_CCMAction.cshtml");
        }
        public ActionResult SPA_PSV()
        {
            return PartialView("~/Areas/Credentialing/Views/CCMPortal/_SPA_PSV.cshtml");
        }
        public ActionResult SPA_Document()
        {
            return PartialView("~/Areas/Credentialing/Views/CCMPortal/_SPA_Document.cshtml");
        }

	}
}