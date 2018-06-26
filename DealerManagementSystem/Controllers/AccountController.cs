using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DealerManagementSystem.Models;
using DealerManagementSystem.Utils;
using System.Data.SqlClient;

namespace DealerManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Contragent contragent)
        {
            if (ModelState.IsValid)
            {
                int? contragentId = AuthorizeUser(contragent.UserName, contragent.Password);

                if (contragentId.HasValue)
                {
                    Session.Add("currentUser", new Contragent { Id = contragentId.Value });
                    return RedirectToAction("index", "home");
                }
                else
                {
                    ViewBag.Error = "შესვლის სახელი ან პაროლი არასწორია!";
                    return View("login");
                }
            }
            else
            {
                ViewBag.Error = "შესვლის სახელი და პაროლი აუცილებელია!";
                return View("login");
            }
        }

        int? AuthorizeUser(string userName, string password)
        {
            using (NTContext _nt = new NTContext())
            {
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@userName", Value = userName },
                    new SqlParameter { ParameterName = "@password", Value = password /*HashHelper.Calc(password)*/ }
                };

                int? contragentId = _nt.GetScalar<int>("SELECT c.id FROM book.Contragents AS c WHERE @userName = c.usr_column_542 AND @password = c.usr_column_541", sqlParams); //TODO change Password(usr_column_536) and UserName(usr_column_537)

                return contragentId;
            }
        }
                
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("currentUser");
            return RedirectToAction("login", "account");
        }
    }
}