using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoFlexiEntity.Model;
using AutoFlexiEntity.VMEntity;
using AutoFlexiManagementSystem.Helper;
using AutoFlexiManager.Manager;

namespace AutoFlexiManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        PaymentManager _paymentManager=new PaymentManager();
        ResellerManager _resellerManager=new ResellerManager();
        FlexiloadManager _flexiloadManager=new FlexiloadManager();
        BkashManager _bkashManager=new BkashManager();
        SecurityUserManager _securityUserManager=new SecurityUserManager();
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Home()
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;
            SecurityUser securityUser = _securityUserManager.SelectSecurityUser(new SecurityUser {UserId = userId});
            ViewBag.loginReseller = securityUser.UserRole.RoleName=="Admin"?0:securityUser.RoleId;
            return View();
        }

        public JsonResult GetSummeryBalance()
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;
            
            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;

            double yourBalance = 0;
            double bKashBalance = 0;
            double flexiBalance = 0;

            List<long> resellerIds = _resellerManager.SelectChildReselersIdsWithParentReseller(loginResellerId);
            List<ResellerFund> resellerFunds =
                _paymentManager.SelectResellerFunds(new ResellerFund())
                    .Where(x =>x.ResellerId==loginResellerId)// resellerIds.Contains(x.ResellerId))
                    .ToList();

            List<Flexiload> flexiloads =
                _flexiloadManager.SelectFlexiloads(new Flexiload())
                    .Where(x => x.ResellerId == loginResellerId && x.Status != "C")
                    .ToList();
            List<Bkash> bkashes =
                _bkashManager.SelectBkashs(new Bkash())
                    .Where(x => x.ResellerId == loginResellerId && x.Status != "C")
                    .ToList();

            Reseller reseller = _resellerManager.SelectReseller(new Reseller{ResellerId = loginResellerId});

            string userName = reseller != null ? reseller.Name + "(" + IDictionary[3].Name + ")" : IDictionary[3].Name;
                //resellers.Where(x => x.ResellerId == loginResellerId).Select(x => x.ResellerName).FirstOrDefault()+"("+IDictionary[3].Name+")";
            string resellerLebel = reseller != null ? "Level " + reseller.ResellerLevel.ResellerLevelNo.ToString() : "Level " + "0";

            bKashBalance = resellerFunds.Where(x => x.FundFor == "B" && x.FundType == "A").Sum(v => v.Amount) - resellerFunds.Where(x => x.FundFor == "B" && x.FundType == "D").Sum(v => v.Amount) - bkashes.Sum(x => x.Amount);
            flexiBalance = resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "A").Sum(v => v.Amount) - resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "D").Sum(v => v.Amount) - flexiloads.Sum(s => s.Amount);
            return Json(new { yourBalance = bKashBalance + flexiBalance, bkashBalance = bKashBalance, flexiBalance = flexiBalance, UserName = userName, ResellerLebel = resellerLebel }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Welcome()
        {
            return View();
        }

        //
        // GET: /Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Home/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create
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
        // GET: /Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Home/Edit/5
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
        // GET: /Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Home/Delete/5
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

        public JsonResult CheckSessionTimeOut()
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            if (companyId != 0)
            {
                return Json("False");
            }
            else
                return Json("True");
        }
    }
}
