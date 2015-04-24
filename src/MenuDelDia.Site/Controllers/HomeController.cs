using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using MenuDelDia.Site.Helpers;
using MenuDelDia.Site.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace MenuDelDia.Site.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(MessageModel model)
        {
            var dic = new Dictionary<string, string>
            {
                {"emailMessage",""},
                {"subjectMessage",""},
                {"messageMessage",""},
                {"returnMessage","¡Gracias por contactarnos! Nos comunicaremos con usted a la brevedad."},

            };

            if (ModelState.IsValid)
            {
                var smtpClient = new SmtpClient("smtp.googlemail.com", Convert.ToInt32(587));
                var credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mailAccount"],
                                                                   ConfigurationManager.AppSettings["mailPassword"]);

                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;

                MailMessage mail = new MailMessage();

                //Setting From , To and CC
                mail.From = new MailAddress(ConfigurationManager.AppSettings["mailAccount"], "Menú del Día");
                mail.To.Add(new MailAddress("menudeldia.uy@gmail.com"));

                var mailsCc = ConfigurationManager.AppSettings["mailCC"];
                if (string.IsNullOrEmpty(mailsCc) == false)
                {
                    var mailsCcSplit = mailsCc.Split('|');

                    foreach (var mailCc in mailsCcSplit)
                    {
                        if (string.IsNullOrEmpty(mailCc) == false && mailCc.Contains("@"))
                        {
                            mail.CC.Add(new MailAddress(mailCc));
                        }
                    }
                }

                var ip = System.Web.HttpContext.Current.Request.GetIPAddress();

                mail.Subject = "Formulario de contacto";

                var str = new StringBuilder("");
                str.Append("<table>");
                str.Append("<thead><tr><th></th><th></th></tr>");
                str.Append("</thead>");
                str.Append("<tbody>");
                str.Append("<tr><td>Email:</td><td>{0}</td></tr>");
                str.Append("<tr><td>Asunto:</td><td>{1}</td></tr>");
                str.Append("<tr><td>Mensaje:</td><td>{2}</td></tr>");
                str.Append("<tr><td>Ip:</td><td>{3}</td></tr>");
                str.Append("</tbody>");
                str.Append("</table>");

                mail.Body = string.Format(str.ToString(), model.Email, model.Subject, model.Message, ip);
                mail.IsBodyHtml = true;

                smtpClient.Send(mail);
            }


            if (ModelState["Email"].Errors.Any()) { dic["emailMessage"] = "El correo ingresado no es válido"; }
            if (ModelState["Subject"].Errors.Any()) { dic["subjectMessage"] = "El asunto ingresado no es válido"; }
            if (ModelState["Message"].Errors.Any()) { dic["messageMessage"] = "El mensaje ingresado no es válido"; }

            return Content(JsonConvert.SerializeObject(dic));
        }

    }
}