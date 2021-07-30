using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoFlexiEntity.Model;
using AutoFlexiEntity.VMEntity;
using AutoFlexiManagementSystem.AutoFlexiEntity.CommonModel;
using AutoFlexiManagementSystem.Helper;
using AutoFlexiManager.Manager;

namespace AutoFlexiManagementSystem.Controllers
{
    public class FlexiloadController : Controller
    {
        FlexiloadManager flexiloadManager=new FlexiloadManager();
        ResellerManager _resellerManager=new ResellerManager();       
        BkashManager _bkashManager = new BkashManager();
        PaymentManager paymentManager = new PaymentManager();
        //
        // GET: /Flexiload/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Flexiload/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Flexiload/Create
        public ActionResult Create()
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;
            long authYN=(long)IDictionary[6].Id;
            string authCode = IDictionary[7].Name;

            ViewBag.AuthYN = authYN;
            //ViewBag.AuthCode = authCode;
            return View();
        }

        //
        // POST: /Flexiload/Create
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
        // GET: /Flexiload/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Flexiload/Edit/5
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
        // GET: /Flexiload/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Flexiload/Delete/5
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
        public JsonResult Save(FlexiloadVM FlexiInfo)
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;
            long authYn = (long)IDictionary[6].Id;
            string authCode = IDictionary[7].Name;

            Result result=new Result();
            if (authYn == 1 && authCode != FlexiInfo.PIN)
            {
                result.IsSuccess = false;
                result.Message = "Wrong Authentication";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
            Flexiload flexoload = new Flexiload
            {
                ResellerId = loginResellerId,
                Amount = FlexiInfo.Amount,
                Dateofentry = DateTime.Now,
                EntryBy = userId,
                MobileNumber = FlexiInfo.MobileNumber,
                Remarks = FlexiInfo.Remarks,
                MobileNumberType = FlexiInfo.MobileType,
                Status = "N"
            };

            result=flexiloadManager.Save(flexoload);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadFlexiload(long resellerid = 0, string accno = "",bool all=false)
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;
            Flexiload flexiload = new Flexiload();
            List<Flexiload> list=new List<Flexiload>();
            List<long> resellerIds = new List<long>();
            if (resellerid > 0)
            {
                resellerIds.Add(resellerid);
            }
            else
            {
                resellerIds = _resellerManager.SelectChildReselersIdsWithParentReseller(loginResellerId);
            }

            if (accno != "")
            {
                if (all==false)
                list = flexiloadManager.SelectFlexiloads(flexiload).OrderByDescending(x => x.Dateofentry).Where(x => resellerIds.Contains(x.ResellerId)).Take(10).Where(x => x.MobileNumber.Contains(accno)).ToList();
                else
                    list = flexiloadManager.SelectFlexiloads(flexiload).OrderByDescending(x => x.Dateofentry).Where(x => resellerIds.Contains(x.ResellerId)).Where(x => x.MobileNumber.Contains(accno)).ToList();
            }
            else
            {
                if(all==false)
                list = flexiloadManager.SelectFlexiloads(flexiload).OrderByDescending(x => x.Dateofentry).Where(x => resellerIds.Contains(x.ResellerId)).Take(10).ToList();
                else
                    list = flexiloadManager.SelectFlexiloads(flexiload).OrderByDescending(x => x.Dateofentry).Where(x => resellerIds.Contains(x.ResellerId)).ToList();
            }

            

            var data = list.Select(x => new { FlexiId = x.FlexiloadId, Time = x.Dateofentry.ToShortDateString(), ActionTime = x.DateofAction != null ? Convert.ToDateTime(x.DateofAction).ToShortDateString() : "", TransactionId = x.TransectionId, Type = x.MobileNumberType == "pre" ? "Pre-Paid" : "Post-Paid", Amount = x.Amount, MobileNo = x.MobileNumber, Status = x.Status == "N" ? "Processing" : x.Status == "Y" ? "<font face='verdana' color='green'>Success</font>" : "<font face='verdana' color='red'>Failed</font>", ResellerName = x.Reseller.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetSummeryBalance()
        //{
        //    Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
        //    long companyId = (long)IDictionary[1].Id;
        //    long locationId = (long)IDictionary[2].Id;
        //    long userId = (long)IDictionary[3].Id;

        //    long loginResellerId = (long)IDictionary[4].Id;
        //    long loginResellerLevelId = (long)IDictionary[5].Id;

        //    double yourBalance = 0;
        //    double bKashBalance = 0;
        //    double flexiBalance = 0;

        //    List<long> resellerIds = _resellerManager.SelectChildReselersIdsWithParentReseller(loginResellerId);
        //    List<ResellerFund> resellerFunds =
        //        paymentManager.SelectResellerFunds(new ResellerFund())
        //            .Where(x =>x.ResellerId==loginResellerId) //resellerIds.Contains(x.ResellerId))
        //            .ToList();

        //    List<Flexiload> flexiloads =
        //        flexiloadManager.SelectFlexiloads(new Flexiload())
        //            .Where(x => resellerIds.Contains(x.ResellerId) && x.Status != "C")
        //            .ToList();
        //    List<Bkash> bkashes =
        //        _bkashManager.SelectBkashs(new Bkash())
        //            .Where(x => resellerIds.Contains(x.ResellerId) && x.Status != "C")
        //            .ToList();

        //    Reseller reseller = _resellerManager.SelectReseller(new Reseller { ResellerId = loginResellerId });

        //    string userName = reseller != null ? reseller.Name + "(" + IDictionary[3].Name + ")" : IDictionary[3].Name;
        //    //resellers.Where(x => x.ResellerId == loginResellerId).Select(x => x.ResellerName).FirstOrDefault()+"("+IDictionary[3].Name+")";
        //    string resellerLebel = reseller != null ? "Level " + reseller.ResellerLevel.ResellerLevelNo.ToString() : "Level " + "0";

        //    bKashBalance = resellerFunds.Where(x => x.FundFor == "B" && x.FundType == "A").Sum(v => v.Amount) - resellerFunds.Where(x => x.FundFor == "B" && x.FundType == "D").Sum(v => v.Amount) - bkashes.Sum(x => x.Amount);

        //    double amnt = resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "A").Sum(v => v.Amount);
        //    double amnt2 = resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "D").Sum(v => v.Amount);
        //    double amnt3 = flexiloads.Sum(s => s.Amount);
        //    flexiBalance = amnt - amnt2 - amnt3; //resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "A").Sum(v => v.Amount) - resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "D").Sum(v => v.Amount) - flexiloads.Sum(s => s.Amount);
        //    return Json(new { yourBalance = bKashBalance + flexiBalance, bkashBalance = bKashBalance, flexiBalance = flexiBalance, UserName = userName, ResellerLebel = resellerLebel }, JsonRequestBehavior.AllowGet);

        //}
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

            long rootResellerId = _resellerManager.SelectRootResellerId(loginResellerId);

            List<long> resellerIds = _resellerManager.SelectChildReselersIdsWithParentReseller(rootResellerId);
            List<ResellerFund> resellerFunds =
                paymentManager.SelectResellerFunds(new ResellerFund())
                    .Where(x => x.ResellerId == rootResellerId) //resellerIds.Contains(x.ResellerId))
                    .ToList();

            List<Flexiload> flexiloads =
                flexiloadManager.SelectFlexiloads(new Flexiload())
                    .Where(x => resellerIds.Contains(x.ResellerId) && x.Status != "C")
                    .ToList();
            List<Bkash> bkashes =
                _bkashManager.SelectBkashs(new Bkash())
                    .Where(x => resellerIds.Contains(x.ResellerId) && x.Status != "C")
                    .ToList();

            Reseller reseller = _resellerManager.SelectReseller(new Reseller { ResellerId = loginResellerId });

            string userName = reseller != null ? reseller.Name + "(" + IDictionary[3].Name + ")" : IDictionary[3].Name;
            //resellers.Where(x => x.ResellerId == loginResellerId).Select(x => x.ResellerName).FirstOrDefault()+"("+IDictionary[3].Name+")";
            string resellerLebel = reseller != null ? "Level " + reseller.ResellerLevel.ResellerLevelNo.ToString() : "Level " + "0";

            bKashBalance = resellerFunds.Where(x => x.FundFor == "B" && x.FundType == "A").Sum(v => v.Amount) - resellerFunds.Where(x => x.FundFor == "B" && x.FundType == "D").Sum(v => v.Amount) - bkashes.Sum(x => x.Amount);

            //double amnt = resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "A").Sum(v => v.Amount);
            //double amnt2 = resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "D").Sum(v => v.Amount);
            //double amnt3 = flexiloads.Sum(s => s.Amount);
            flexiBalance = resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "A").Sum(v => v.Amount) - resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "D").Sum(v => v.Amount) - flexiloads.Sum(s => s.Amount);
            return Json(new { yourBalance = bKashBalance + flexiBalance, bkashBalance = bKashBalance, flexiBalance = flexiBalance, UserName = userName, ResellerLebel = resellerLebel }, JsonRequestBehavior.AllowGet);
        }
    }
}
