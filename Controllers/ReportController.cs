using AutoFlexiEntity.Model;
using AutoFlexiManagementSystem.Helper;
using AutoFlexiManager.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoFlexiManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        ResellerManager _resellerManager = new ResellerManager();
        PaymentManager _paymentManager = new PaymentManager();
        FlexiloadManager _flexiloadManager = new FlexiloadManager();
        BkashManager _bkashManager = new BkashManager();
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReportView(long Id = 0)
        {
            ViewBag.resellerid = Id;
            return View();
        }
        public JsonResult ShowReport(int reportType,string fromDate,string toDate,int resellerId)
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;
            
            if (reportType==1)//Reseller List
            {
                //if (resellerId == 0)
                //{
                //    resellerId = (int)loginResellerId;
                //}
                if (fromDate != "" && toDate != "")
                {
                    var list = _resellerManager.SelectAllResellers(new Reseller { ParentResellerId = resellerId }).Where(x => x.DateofEntry > Convert.ToDateTime(fromDate) && x.DateofEntry < Convert.ToDateTime(toDate)).Select(x => new { Name = x.Name, Amount = x.Name, ContactNo = x.Phone, DateOfEntry = x.DateofEntry.ToShortDateString(), ResellerLevelNo = x.ResellerLevel.ResellerLevelNo });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list = _resellerManager.SelectAllResellers(new Reseller { ParentResellerId = resellerId }).Select(x => new { Name = x.Name, Amount = x.Name, ContactNo = x.Phone, DateOfEntry = x.DateofEntry.ToShortDateString(), ResellerLevelNo = x.ResellerLevel.ResellerLevelNo });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

            }
            else if(reportType==2)//Payment List
            {

                if (fromDate != "" && toDate != "")
                {
                    var list = _paymentManager.SelectResellerFunds(new ResellerFund() { ResellerId = resellerId }).Where(x => x.DateOfEntry > Convert.ToDateTime(fromDate) && x.DateOfEntry < Convert.ToDateTime(toDate)).Select(x => new { Amount = x.Amount,ResellerName=x.Reseller.Name, DateOfEntry = x.DateOfEntry.ToShortDateString(), FundFor = x.FundFor == "B" ? "Bkash" : "Flexi", MobileAccountType = x.MobileAccountType != null ? x.MobileAccountType.Name : "", Remarks = x.Remarks, GivenBy = x.SecurityUser.Name });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list = _paymentManager.SelectResellerFunds(new ResellerFund() { ResellerId = resellerId }).Select(x => new { Amount = x.Amount, ResellerName = x.Reseller.Name, DateOfEntry = x.DateOfEntry.ToShortDateString(), FundFor = x.FundFor == "B" ? "Bkash" : "Flexi", MobileAccountType = x.MobileAccountType != null ? x.MobileAccountType.Name : "", Remarks = x.Remarks,GivenBy=x.SecurityUser.Name });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            else if (reportType == 3)//Flexiload Detail
            {
                if (fromDate != "" && toDate != "")
                {
                    var list = _flexiloadManager.SelectFlexiloads(new Flexiload() { ResellerId = resellerId }).Where(x => x.Dateofentry > Convert.ToDateTime(fromDate) && x.Dateofentry < Convert.ToDateTime(toDate)).Select(x => new { Amount = x.Amount, SentBy = x.Reseller.Name, DateOfEntry = x.DateofAction.ToString(), MobileNumber = x.MobileNumber, Status = x.Status == "Y" ? "Approved" : x.Status == "C" ? "Cancel" : "Pending", Remarks = x.Remarks });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list = _flexiloadManager.SelectFlexiloads(new Flexiload() { ResellerId = resellerId }).Select(x => new { Amount = x.Amount,SentBy = x.Reseller.Name, DateOfEntry = x.DateofAction.ToString(), MobileNumber = x.MobileNumber, Status = x.Status == "Y" ? "Approved" : x.Status == "C" ? "Cancel" : "Pending", Remarks = x.Remarks });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            else if (reportType == 4)//bKash Detail
            {
                if (fromDate != "" && toDate != "")
                {
                    var list = _bkashManager.SelectBkashs(new Bkash() { ResellerId = resellerId }).Where(x => x.Dateofentry > Convert.ToDateTime(fromDate) && x.Dateofentry < Convert.ToDateTime(toDate)).Select(x => new { Amount = x.Amount, SentBy = x.Reseller.Name, DateOfEntry = x.DateofAction.ToString(), BkashNumber = x.BkashNumber, Status = x.Status == "Y" ? "Approved" : x.Status == "C" ? "Cancel" : "Pending", Remarks = x.Remarks, TransectionId = x.TransectionId });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list = _bkashManager.SelectBkashs(new Bkash() { ResellerId = resellerId }).Select(x => new { Amount = x.Amount, SentBy = x.Reseller.Name, DateOfEntry = x.DateofAction.ToString(), BkashNumber = x.BkashNumber, Status = x.Status == "Y" ? "Approved" : x.Status == "C" ? "Cancel" : "Pending", Remarks = x.Remarks, TransectionId = x.TransectionId });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            //else if (reportType == 5)//Balance Detail
            //{
            //    var list = _bkashManager.SelectBkashs(new Bkash() { ResellerId = resellerId }).Select(x => new { Amount = x.Amount, DateOfEntry = x.DateofAction, x.BkashNumber, Status = x.Status == "Y" ? "Approved" : x.Status == "C" ? "Cancel" : "Pending", Remarks = x.Remarks, TransectionId = x.TransectionId });
            //    return Json(list, JsonRequestBehavior.AllowGet);
            //}
            else
                return Json("Not Found", JsonRequestBehavior.AllowGet);
        }

    }
}