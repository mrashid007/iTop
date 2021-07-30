using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoFlexiEntity.Model;
using AutoFlexiEntity.Models.CommonModel;
using AutoFlexiManager.Manager;
using Microsoft.AspNet.Identity;

namespace AutoFlexiManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        SecurityUserManager userManager = new SecurityUserManager();
        ResellerManager resellerManager=new ResellerManager();
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Account/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Account/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Account/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Account/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
         [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
             if (ModelState.IsValid)
             {
                 SecurityUser securityUser = userManager.GetSecurityUser(new SecurityUser { UserName = model.UserName, Password = model.Password });

                 if (securityUser != null)
                 {
                     Reseller reseller = resellerManager.SelectReseller(new Reseller{SecurityUserId = securityUser.UserId});
                     System.Web.HttpContext.Current.Session["LoginCompany"] = securityUser.CompanyId;
                     System.Web.HttpContext.Current.Session["LoginLocation"] = securityUser.LocationId;
                     System.Web.HttpContext.Current.Session["LoginUser"] = securityUser.UserId;
                     System.Web.HttpContext.Current.Session["LoginUserName"] = securityUser.UserName;
                     System.Web.HttpContext.Current.Session["LoginReseller"] =reseller!=null?reseller.ResellerId:0;
                     System.Web.HttpContext.Current.Session["LoginResellerLevel"] = reseller!=null?reseller.ResellerlebelId:0;
                     System.Web.HttpContext.Current.Session["LoginResellerName"] =reseller!=null?reseller.Name:"";
                     System.Web.HttpContext.Current.Session["PinAuth"] = reseller != null ? reseller.PinAuthYn==true?1:0 :0;
                     System.Web.HttpContext.Current.Session["AuthCode"] = reseller != null ? reseller.AuthCode :" ";
                     
                     if (securityUser.UserId > 0)
                         return RedirectToAction("Home", "Home");
                     else
                     {
                         ModelState.AddModelError("LOGIN_ERROR", "Incorrect User Name or Password.");
                         return RedirectToAction("Login", "Account");
                     }
                 }
                 else
                 {
                     ModelState.AddModelError("LOGIN_ERROR", "Incorrect User Name or Password.");
                     return RedirectToAction("Login", "Account", 1);
                 }
             }
             else
             {
                 return RedirectToAction("LogIn");
             }
        }

        public ActionResult Login(int value=0)
        {
            if (value == 1)
                ViewBag.Message = "User Name Or Password is incorrect";
            else
                ViewBag.Message = "";
            return View();
        }
        public ActionResult LogOff()
        {
            return RedirectToAction("LogIn");
        }
    }
}
