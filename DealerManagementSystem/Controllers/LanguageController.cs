using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DealerManagementSystem.Utils;
using System.Threading;
using System.Globalization;

namespace DealerManagementSystem.Controllers
{
    public class LanguageController : BaseController
    {      
        public ActionResult Change(string lang)
        {                       
            if (!string.IsNullOrEmpty(lang))
            {
                HttpCookie currentLanguage = Request.Cookies["Language"];

                if(currentLanguage != null)
                {
                    if (lang != currentLanguage.Value)
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

                        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(lang);                        
                    }
                }

                HttpCookie cookie = new HttpCookie("Language");
                cookie.Value = lang;
                cookie.Expires.AddYears(100);
                Response.Cookies.Add(cookie);
            }           

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}