using Newtonsoft.Json;
using PracticeExercise.Enums;
using PracticeExercise.Filters;
using PracticeExercise.Models;
using PracticeExercise.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;

namespace PracticeExercise.Controllers
{
    public class HomeController : Controller
    {

        public static SelectList ManagerList { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Login";

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string id)
        {
            if (ModelState.IsValid)
            {
                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter() { ParameterName = "UserId", SqlDbType = SqlDbType.VarChar, Size = 50, Value = model.UserId },
                    new SqlParameter() { ParameterName = "Password", SqlDbType = SqlDbType.VarChar, Size = 50, Value = model.Password }
                };
                var dbHelper = new DbHelper();
                DataSet ds = dbHelper.ExecuteQuery("SELECT * FROM Users WHERE UserId=@UserId AND Password=@Password AND IsActive=1", parameters);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    UserRoleEnum roleIdEnum;
                    Enum.TryParse(row["RoleId"].ToString(), out roleIdEnum);

                    //Session["RoleId"] = roleIdEnum;
                    Session["RoleId"] = row["RoleId"].ToString();
                    Session["Userid"] = row["UserId"].ToString();

                    Session["CurrentUser"] = new UserViewModel
                    {
                        UserId = row["UserId"].ToString(),
                        UserName = row["UserName"].ToString(),
                        EmailId = row["Email"].ToString(),
                        RoleId = row["RoleId"].ToString(),
                        MobileNo = row["MobileNo"].ToString(),
                        Password = model.Password
                    };

                    switch (roleIdEnum)
                    {
                        case UserRoleEnum.User:
                            return RedirectToAction("TimeSheetDetails", "Home");

                        case UserRoleEnum.Manager:
                            return RedirectToAction("TimeSheetDetails", "Home");

                        case UserRoleEnum.CEO:
                            return RedirectToAction("TimeSheetDetails", "Home");

                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.errorMessage = "Invalid credentials.";
                }
            }

            return View(model);
        }

        [AuthenticationFilter]
        public ActionResult AddTimeSheet()
        {
            if (Session["RoleId"] != null)
                ViewBag.Role = Session["RoleId"].ToString();
            else
                ViewBag.Role = "1";

            if (Session["Userid"] != null)
                ViewBag.Userid = Session["Userid"].ToString();
            else
                ViewBag.Userid = "Arun";

            ViewBag.Message = "Time Sheet";
            DbHelper dbHelper = new DbHelper();
            DataSet ds = dbHelper.ExecuteQuery("SELECT * FROM Users where RoleId=2", null);
            List<SelectListItem> managerItems = new List<SelectListItem>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                managerItems.Add(new SelectListItem() { Text = row["UserName"].ToString(), Value = row["UserId"].ToString() });
            }
            ManagerList = new SelectList(managerItems, "Value", "Text");

            ViewBag.ManagerList = ManagerList;

            return View();
        }

        [AuthenticationFilter]
        public ActionResult TimeSheetDetails()
        {
            if (Session["RoleId"] != null)
                ViewBag.Role = Session["RoleId"].ToString();
            else
                ViewBag.Role = "1";

            if (Session["Userid"] != null)
                ViewBag.Userid = Session["Userid"].ToString();
            else
                ViewBag.Userid = "Arun";

            ViewBag.Message = "Time Sheet";

            return View();
        }


        [HttpPost]
        public ActionResult GetPractice(string Role, string Userid)
        {
            var dbHelper = new DbHelper();
            DataSet ds = dbHelper.ExecuteQuery("select * from [dbo].[TimeSheet] where UserId='" + Userid + "';", null);
            var strJson = JsonConvert.SerializeObject(ds);
            return Content(strJson);
        }

        [HttpPost]
        public ActionResult AddPractice(string UserId, string Date, string HoursWorked, string ManagerID)
        {
            if (Session["Userid"] != null)
                UserId = Session["Userid"].ToString();
            var strRes = new DbHelper().ExecuteNonQuery("INSERT INTO dbo.TimeSheet (UserId,Date,HoursWorked,ManagerID) VALUES ('" + UserId + "','" + Date.Substring(0, 10) + "','" + HoursWorked + "','" + ManagerID + "')", null);
            if (!string.IsNullOrEmpty(strRes))
            {
                return Content(JsonConvert.SerializeObject(new ServiceResponse() { status = "0", errorMessage = strRes }));
            }

            return Content(JsonConvert.SerializeObject(new ServiceResponse() { status = "1" }));
        }

        [HttpPost]
        public ActionResult GetAllTimeSheet(string Role, string Userid)
        {
            var parameters = new List<SqlParameter>()
                {
                    new SqlParameter() { ParameterName = "Role", SqlDbType = SqlDbType.VarChar, Size = 50, Value = Role },
                    new SqlParameter() { ParameterName = "Userid", SqlDbType = SqlDbType.VarChar, Size = 50, Value =Userid }
                };
            var dbHelper = new DbHelper();
            DataSet ds = dbHelper.ExecuteStoredProcedure("SP_GET_TIMESHEET", parameters);

            var strJson = JsonConvert.SerializeObject(ds);
            return Content(strJson);
        }

        //select t.*,u.UserName from dbo.TimeSheet t inner join dbo.Users u on t.UserId=u.UserId
        [AuthenticationFilter]
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

    }
}