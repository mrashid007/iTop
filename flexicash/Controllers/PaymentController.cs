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
    public class PaymentController : Controller
    {
        ResellerManager _resellerManager=new ResellerManager();
        PaymentManager paymentManager=new PaymentManager();        
        FlexiloadManager _flexiloadManager = new FlexiloadManager();
        BkashManager _bkashManager = new BkashManager();
        //
        // GET: /Payment/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Payment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Payment/Create
        public ActionResult Create(long Id=0)
        {
            ViewBag.resellerid = Id;
            return View();
        }

        //
        // POST: /Payment/Create
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
        // GET: /Payment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Payment/Edit/5
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
        // GET: /Payment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Payment/Delete/5
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
        public ActionResult Save(PaymentVM paymentDetail, string pin = "")
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;

            ResellerFund resellerFund = new ResellerFund
            {
                Amount = paymentDetail.Amount,
                DateOfEntry = DateTime.Now,
                EntryBy = userId,
                FundFor = paymentDetail.FundFor,
                FundType = paymentDetail.FundType,
                Remarks = paymentDetail.Remrks,
                ResellerId = paymentDetail.ResellerId,
                MobileAccountTypeId = paymentDetail.MobileAccountTypeId == 0 ? null : paymentDetail.MobileAccountTypeId
            };

            Result result = paymentManager.Save(resellerFund);

            //Reseller reseller = _resellerManager.SelectReseller(new Reseller {ResellerId = paymentDetail.ResellerId});
            //if (reseller.ResellerLevel.ResellerLevelNo!=5 && result.IsSuccess)
            //{
            //    resellerFund = new ResellerFund
            //    {
            //        Amount = -paymentDetail.Amount,
            //        DateOfEntry = DateTime.Now,
            //        EntryBy = userId,
            //        FundFor = paymentDetail.FundFor,
            //        FundType = paymentDetail.FundType,
            //        Remarks = paymentDetail.Remrks,
            //        ResellerId = loginResellerId,
            //        MobileAccountTypeId = paymentDetail.MobileAccountTypeId == 0 ? null : paymentDetail.MobileAccountTypeId
            //    };

            //    result = paymentManager.Save(resellerFund);
            //}

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult LoadResellers()
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;

            List<Reseller> vmlist = _resellerManager.SelectResellers(new Reseller { ParentResellerId = loginResellerId });
            if (IDictionary[3].Name.ToLower() == "admin")
            {
                List<Reseller> vml =_resellerManager.SelectResellers(new Reseller {ResellerId = loginResellerId}).ToList();
                if(vml.Count>=1)
                    vmlist.AddRange(vml);
            }
            var data = vmlist.Select(v => new {Name = v.Name, Value = v.ResellerId}).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LoadPayments(long resellerId, bool all = false)
        {
            List<PaymentVM> vm = new List<PaymentVM>();

            List<ResellerFund> resellerFunds = new List<ResellerFund>();
            
            
            resellerFunds = paymentManager.SelectResellerFunds(new ResellerFund { ResellerId = resellerId }).OrderBy(x => x.DateOfEntry).ToList();//.Take(10)
            
            //else
            //{
            //    resellerFunds = paymentManager.SelectResellerFunds(new ResellerFund { ResellerId = resellerId }).OrderBy(x => x.DateOfEntry).ToList();//
            //}
            double grandTotal = 0;
            long id = 0;
            foreach(ResellerFund rf in resellerFunds)
            {
                PaymentVM vms = new PaymentVM();
                vms.Id = id + 1 ;
                vms.Amount = rf.Amount;
                vms.PrevAmount = grandTotal;
                if(rf.FundType=="A")
                    grandTotal += rf.Amount;
                else
                    grandTotal -= rf.Amount;

                vms.ResellerId = rf.ResellerId;
                vms.FundFor = rf.FundFor == "F" ? "Flexi" : "Bkash";
                vms.FundType = rf.FundType == "A" ? "Addition" : "Deduction";
                vms.PaymentDate = rf.DateOfEntry.ToString();
                vm.Add(vms);
                id++;
            }

            if (all == false)
            {
                var data = vm.OrderByDescending(x=>x.Id).ToList().Take(10).OrderByDescending(x=>x.Id).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = vm.OrderByDescending(x => x.Id).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            //var data =
            //    resellerFunds.Where(x=>x.Amount>0).Select(
            //        x =>
            //            new PaymentVM
            //            {                            
            //                ResellerId = x.ResellerId,
            //                PrevAmount=x.Amount-x.Amount,
            //                Amount = x.Amount,
            //                FundFor = x.FundFor=="F"?"Flexi":"Bkash",
            //                FundType = x.FundType=="A"?"Addition":"Deduction",
            //                PaymentDate = x.DateOfEntry.ToString()
            //            }).ToList();
            //return Json(vm.OrderByDescending(x => x.DateOfEntry).toli, JsonRequestBehavior.AllowGet);
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
                paymentManager.SelectResellerFunds(new ResellerFund())
                    .Where(x => x.ResellerId == loginResellerId)//resellerIds.Contains(x.ResellerId))
                    .ToList();

            List<Flexiload> flexiloads =
                _flexiloadManager.SelectFlexiloads(new Flexiload())
                    .Where(x => resellerIds.Contains(x.ResellerId) && x.Status!="C")
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
            flexiBalance = resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "A").Sum(v => v.Amount) - resellerFunds.Where(x => x.FundFor == "F" && x.FundType == "D").Sum(v => v.Amount) - flexiloads.Sum(s => s.Amount);
            return Json(new { yourBalance = bKashBalance + flexiBalance, bkashBalance = bKashBalance, flexiBalance = flexiBalance, UserName = userName, ResellerLebel = resellerLebel }, JsonRequestBehavior.AllowGet);

        }
    }
}
