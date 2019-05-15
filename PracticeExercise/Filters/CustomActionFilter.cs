using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PracticeExercise.Enums;
using PracticeExercise.Models;

namespace PracticeExercise.Filters
{
    public class SessionExpireFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RedirectToRouteResult redirectToRouteResult = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
            HttpContext context = HttpContext.Current;

            if (context.Session.IsNewSession)
            {
                filterContext.Result = redirectToRouteResult;
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class AuthenticationFilter : ActionFilterAttribute, IActionFilter
    {
        private readonly string _userType;

        public AuthenticationFilter(String userType = "")
        {
            this._userType = userType;
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            RedirectToRouteResult redirectToRouteResult = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login", id = "0" }));

            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated || filterContext.HttpContext.Session["currentUser"] == null)
            if (filterContext.HttpContext.Session["currentUser"] == null)
            {
                filterContext.Result = redirectToRouteResult;
            }
            else if (!string.IsNullOrEmpty(_userType))
            {
                UserViewModel currentUser = filterContext.HttpContext.Session["currentUser"] as UserViewModel;

                if (currentUser == null)
                {
                    filterContext.Result = redirectToRouteResult;
                }
                else
                {
                    UserRoleEnum role;
                    Enum.TryParse(currentUser.RoleId, out role);

                    if (!string.IsNullOrEmpty(_userType) && !_userType.Contains("|" + role.ToString() + "|"))
                    {
                        filterContext.Result = redirectToRouteResult;
                    }
                }
            }

            this.OnActionExecuting(filterContext);
        }
    }
}