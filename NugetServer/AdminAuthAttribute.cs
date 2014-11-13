using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NugetServer
{
	public class AdminAuthAttribute : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (!LoginAuthorizeCore(filterContext.HttpContext))
			{
				this.RedirectToLoginPage(filterContext);
			}
		}

		protected virtual bool LoginAuthorizeCore(HttpContextBase httpContext)
		{
			return HttpContext.Current.User.Identity.IsAuthenticated;
		}

		protected void RedirectToLoginPage(AuthorizationContext filterContext)
		{
			var routeValue = new RouteValueDictionary 
			{ 
				{ "Controller", "Home" }, 
				{ "Action", "Login" }
			};

			filterContext.Result = new RedirectToRouteResult(routeValue);
		}
	}
}