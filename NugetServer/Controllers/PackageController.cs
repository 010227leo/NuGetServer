using System;
using System.IO;
using System.Web.Mvc;
using System.Xml;

namespace NugetServer.Controllers
{
	using NugetServer.Models;

	public class PackageController : Controller
	{
		[AdminAuth]
		public ActionResult Index()
		{
			return View();
		}

		[ChildActionOnly]
		public ActionResult PackageList()
		{
			var doc = new XmlDocument();
			var path = GetUrl("/nuget/Packages").ToString();

			doc.Load(path);

			return new XmlActionResult
			{
				Document = doc,
				TransformSource = Server.MapPath("~/Content/xsl/packages.xsl")
			};
		}

		[AdminAuth]
		public ActionResult Delete(string id)
		{
			if (string.IsNullOrWhiteSpace(id) || !id.Contains("/api/v2/package/"))
			{
				ViewBag.StatusMessage = new StatusMessage
				{
					Success = false,
					Message = "id is illegal."
				};
			}
			else
			{
				var filename = id.Split(new string[] { "/api/v2/package/" }, StringSplitOptions.None)[1];

				filename = string.Format("{0}.nupkg", filename.Replace("/", "."));

				var path = Path.Combine(Server.MapPath("~/Packages"), filename);

				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);

					ViewBag.StatusMessage = new StatusMessage
					{
						Success = true,
						Message = String.Format("{0} was deleted successfully.", filename)
					};
				}
				else
				{
					ViewBag.StatusMessage = new StatusMessage
					{
						Success = false,
						Message = String.Format("{0} does not exist.", filename)
					};
				}
			}

			return View("Index");
		}

		private static Uri GetUrl(string relativePath)
		{
			if (System.Web.HttpContext.Current != null)
			{
				var uri = System.Web.HttpContext.Current.Request.Url;
				return new UriBuilder(uri.Scheme, uri.Host, uri.Port, relativePath).Uri;
			}

			var defaultUri = new Uri("http://localhost");
			return new UriBuilder(defaultUri.Scheme, defaultUri.Host, defaultUri.Port, relativePath).Uri;
		}
	}
}
