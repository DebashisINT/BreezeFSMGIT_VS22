using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Areas.LMS.Controllers
{
    public class LMSQuestionsController : Controller
    {
        // GET: LMS/LMSQuestions
        public ActionResult Index()
        {
            return View();
        }
    }
}