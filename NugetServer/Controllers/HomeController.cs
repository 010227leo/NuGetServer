using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace NugetServer.Controllers
{
	using NugetServer.Models;
	using System.Web.Security;

	public class HomeController : Controller
	{
		[AdminAuth]
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[AdminAuth]
		public ActionResult Upload(HttpPostedFileBase upload)
		{
			if (upload == null)
			{
				ModelState.AddModelError("upload", "Please select a file");
			}
			else
			{
				string extension = Path.GetExtension(upload.FileName);
				if (!(extension ?? "").Equals(".nupkg", StringComparison.CurrentCultureIgnoreCase))
				{
					ModelState.AddModelError("upload", "Invalid extension. Only .nupkg files will be accepted.");
				}
			}

			if (ModelState.IsValid)
			{
				var path = Path.Combine(Server.MapPath("~/Packages"), upload.FileName);
				upload.SaveAs(path);
				ViewBag.StatusMessage = new StatusMessage
				{
					Success = true,
					Message = String.Format("{0} was uploaded successfully.", upload.FileName)
				};
			}

			return View("Index");
		}

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(string username = null, string password = null)
		{
			if ("admin".Equals(username) && "dfancy".Equals(password))
			{
				var ticket = new FormsAuthenticationTicket(
					1,
					username,
					DateTime.Now,
					DateTime.Now.AddDays(7),
					true,
					string.Empty,
					FormsAuthentication.FormsCookiePath);

				var encTicket = FormsAuthentication.Encrypt(ticket);

				Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

				return RedirectToAction("Index");
			}

			return View();
		}
	}
}
