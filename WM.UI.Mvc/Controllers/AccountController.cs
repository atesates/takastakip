using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Security;
using WM.Core.CrossCuttingConcerns.Security.Web;
using WM.UI.Mvc.Models;
using WebMatrix.WebData;
using WM.Northwind.Business.Abstract.Authorization;
using System.Net.Mail;
using WM.Northwind.Business.Abstract;
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using Microsoft.Web.Helpers;
using System.Drawing.Imaging;
using WM.UI.Mvc.Areas.Kullanici.Models;
using WM.Northwind.Entities.Concrete.IlacTakip;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace WM.UI.Mvc.Controllers
{
    public class AccountController : Controller
    {

        #region ctor
        private IUserService _userService;
        private IGrupService _grupService;
        private IEczaneService _eczaneService;
        private IEczaneUserService _eczaneUserService;
        private IUserRoleService _userRoleService;
        private IEczaneGrupService _eczaneGrupService;
        private Random random = new Random();//güvenlik kdou

        public AccountController(IUserService userService,
                                 IEczaneService eczaneService,
                                 IGrupService grupService,
                                 IEczaneUserService eczaneUserService,
                                 IUserRoleService userRoleService,
                                 IEczaneGrupService eczaneGrupService)
        {
            _userService = userService;
            _grupService = grupService;
            _eczaneService = eczaneService;
            _eczaneUserService = eczaneUserService;
            _userRoleService = userRoleService;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion
        // GET: EczaneNobet/Account
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                Regex regex = new Regex("^/(?<Controller>[^/]*)(/(?<Action>[^/]*)(/(?<id>[^?]*)(/?(?<QueryString>.*))?)?)?$");
                Match match = regex.Match(returnUrl);

                // match.Groups["Controller"].Value is the controller, 
                // match.Groups["Action"].Value is the action,
                // match.Groups["id"].Value is the id
                // match.Groups["QueryString"].Value are the other parameters
            }
            return User.Identity.IsAuthenticated
            ? View("Login")
            : View("Login", new LoginViewModel());
            //? RedirectToAction("Index", "Teklifim", new { area = "Kullanici" })
            //: RedirectToAction("Login", new LoginViewModel());
        }

        // GET: Account
        [HttpPost]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetByEMailAndPassword(login.LoginItem);

                if (user != null)
                {
                    var utcNow = DateTime.UtcNow;

                    var utcExpires = login.LoginItem.RememberMe
                                            ? utcNow.AddDays(29)
                                            : utcNow.AddHours(8); ;

                    var rolIdler = _userService.GetUserRoles(user).OrderBy(s => s.RoleId).Select(u => u.RoleId).ToArray();
                    var roller = _userService.GetUserRoles(user).OrderBy(s => s.RoleId).Select(u => u.RoleName).ToArray();

                    AuthenticationHelper
                        .CreateAuthCookie(
                            new Guid(),
                            user.UserName,
                            user.Email,
                            utcExpires,
                            roller,
                            login.LoginItem.RememberMe,
                            user.FirstName,
                            user.LastName
                            );

                    var url = RedirectToAction("Index", "EczaneHome", new { area = "Kullanici" });

                    var rolId = rolIdler.FirstOrDefault();

                    switch (rolId)
                    {
                        case 1:
                            url = RedirectToAction("Index", "EczaneHome", new { area = "Kullanici"/* ,userId = user.Id, id = 11*/ });
                            break;
                        case 2:
                            url = RedirectToAction("Index", "EczaneHome", new { area = "Kullanici"/*, userId = user.Id, id = 11*/ });
                            break;
                        case 3:
                            url = RedirectToAction("Index", "EczaneHome", new { area = "Kullanici"/*, userId = user.Id, id = 11*/ });
                            break;
                        default:
                            break;
                    }

                    return returnUrl == null ? url : (ActionResult)Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("LoginUserError", "E-Posta ya da şifre hatalı !!!");
                    //ViewBag.Warn = "E-Posta ya da şifre hatalı !!! ";
                    return View(user);
                }
            }
            else
            {
                ViewBag.FormClass = "form-control is-invalid";
                return View(login);
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneGrupEczaneler = gruplar
                 .Join(eczaneGruplar, g => g.Id, ge => ge.GrupId, (g, ge) => new { g, ge })
                 .Join(eczaneler, pge => pge.ge.EczaneId, e => e.Id, (pge, e) => new { pge, e })
                 .Select(m => new
                 {
                     GrupAdi = m.pge.g.Adi,
                     EczaneAdi = m.e.Adi,
                     GrupId = m.pge.g.Id
                 });

            ViewBag.GrupId = new SelectList(eczaneGrupEczaneler, "GrupId", "GrupAdi");

            return View();
        }
        private string GenerateRandomCode() //güvenlik kodu
        {
            string s = "";
            for (int i = 0; i < 6; i++)
                s = String.Concat(s, this.random.Next(10).ToString());
            return s;
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]

        public ActionResult Register(RegisterViewModel Model)
        {
            User newUser = new User();
            newUser.Email = Model.User.Email;
            newUser.FirstName = Model.User.FirstName;
            newUser.LastName = Model.User.LastName;
            newUser.UserName = Model.User.Email;
            newUser.Password = GenerateRandomCode();
            try
            {
                _userService.UserRoleEczaneGrupRegister(newUser, Model.Eczane.EczaneGln, Model.User.Email,
                Model.Eczane.Adi, Model.GrupId);
                SendMail("kayit", "takastakip.com sistemine kayıt yaptınız. Parolanız: " + newUser.Password, newUser.Email);
                return RedirectToAction("Index", "EczaneHome", new { area = "Kullanici", id = newUser.Id });
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.Message.ToString();
            }
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneGrupEczaneler = gruplar
               .Join(eczaneGruplar, g => g.Id, ge => ge.GrupId, (g, ge) => new { g, ge })
               .Join(eczaneler, pge => pge.ge.EczaneId, e => e.Id, (pge, e) => new { pge, e })
               .Select(m => new
               {
                   GrupAdi = m.pge.g.Adi,
                   EczaneAdi = m.e.Adi,
                   EczaneGrupId = m.pge.ge.Id
               });

            ViewBag.GrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi");
            return View(Model);
        }

        public void SendMail(string subject, string body, string toEmail) // RegisterViewModel Model, string password)
        {
            var fromEmail = "yoneylemci@hotmail.com";
            var fromPW = "semihates2017";

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            var smtpClient = new SmtpClient("smtp.live.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new System.Net.NetworkCredential(fromEmail, fromPW)
            };

            smtpClient.Send(message.From.ToString(),
                            message.To.ToString(),
                            message.Subject,
                            message.Body);
        }
    }
}