using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoFlexiEntity.Model;
using AutoFlexiEntity.VMEntity;
using AutoFlexiManagementSystem.AutoFlexiEntity.CommonModel;
using AutoFlexiManagementSystem.Helper;
using AutoFlexiManagementSystem.Models;
using AutoFlexiManager.Manager;

namespace AutoFlexiManagementSystem.Controllers
{
    public class ResellerController : Controller
    {
        ResellerManager _resellerManager=new ResellerManager();
        PaymentManager _paymentManager = new PaymentManager();
        SecurityUserManager _securityUserManager=new SecurityUserManager();
       
        //
        // GET: /Reseller/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Reseller/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Reseller/Create
        public ActionResult Create(long Id = 0)
        {
            ViewBag.resellerid = Id;
            return View();
        }

        //
        // POST: /Reseller/Create
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

        [HttpPost]
        public JsonResult Save(ResellerVm GeneralInfo)
        {
           
                Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
                long companyId = (long)IDictionary[1].Id;
                long locationId = (long)IDictionary[2].Id;
                long userId = (long)IDictionary[3].Id;

                long loginResellerId = (long)IDictionary[4].Id;
                long loginResellerLevelId = (long)IDictionary[5].Id;

                Result result = new Result();

                UserRole userRole = _securityUserManager.LoadUserRole().Where(c=>c.RoleName=="Reseller").FirstOrDefault();

                if (GeneralInfo != null && GeneralInfo.ResellerId<=0)
                {
                    SecurityUser securityUser = new SecurityUser()
                    {
                        Name = GeneralInfo.Name,
                        CompanyId = companyId,
                        LocationId = locationId,
                        UserName = GeneralInfo.UserName,
                        Password = GeneralInfo.Password,
                        RoleId = userRole!=null?userRole.RoleId:1
                    };
                    result = _securityUserManager.Save(securityUser);
                    if (result.IsSuccess == true)
                    {

                        Reseller aReseller = new Reseller()
                        {
                            ResellerId = 0,
                            Name = GeneralInfo.Name,
                            Fathername = GeneralInfo.FatherName,
                            Mothername = GeneralInfo.MotherName,
                            Presentaddress = GeneralInfo.PresentAddress,
                            Parmanentaddress = GeneralInfo.ParmanentAddress,
                            Phone = GeneralInfo.ResellerMobile,
                            Email = GeneralInfo.Email,
                            DateofEntry = DateTime.Now,
                            CompanyId = companyId,
                            EntryBy = userId,
                            AuthCode = GeneralInfo.AuthCode,
                            Active = GeneralInfo.Active == 1 ? true : false,
                            EmailAuthYn = GeneralInfo.EmailAuth == 1 ? true : false,
                            PinAuthYn = GeneralInfo.PinAuth == 1 ? true : false,
                            ResellerlebelId = GeneralInfo.ResellerLevelId,
                            NidNumber = GeneralInfo.NidNumber,
                            SecurityUserId = securityUser.UserId,
                            ParentResellerId = loginResellerId
                        };
                        result = _resellerManager.Save(aReseller);
                    }

                }
                else
                {
                    //Update 
                    SecurityUser securityUser = new SecurityUser()
                    {
                        Name = GeneralInfo.Name,
                        CompanyId = companyId,
                        LocationId = locationId,
                        UserName = GeneralInfo.UserName,
                        Password = GeneralInfo.Password,
                        RoleId = userRole != null ? userRole.RoleId : 1
                    };

                    SecurityUser user = _securityUserManager.SelectSecurityUser(securityUser);
                    user.Password = securityUser.Password;

                    result = _securityUserManager.UpdateSecurityUsers(user);

                    Reseller aReseller = new Reseller()
                    {
                        ResellerId = GeneralInfo.ResellerId,
                        Name = GeneralInfo.Name,
                        Fathername = GeneralInfo.FatherName,
                        Mothername = GeneralInfo.MotherName,
                        Presentaddress = GeneralInfo.PresentAddress,
                        Parmanentaddress = GeneralInfo.ParmanentAddress,
                        Phone = GeneralInfo.ResellerMobile,
                        Email = GeneralInfo.Email,
                        DateofEntry = DateTime.Now,
                        CompanyId = companyId,
                        EntryBy = userId,
                        AuthCode = GeneralInfo.AuthCode,
                        Active = GeneralInfo.Active == 1 ? true : false,
                        EmailAuthYn = GeneralInfo.EmailAuth == 1 ? true : false,
                        PinAuthYn = GeneralInfo.PinAuth == 1 ? true : false,
                        ResellerlebelId = GeneralInfo.ResellerLevelId,
                        NidNumber = GeneralInfo.NidNumber,
                        SecurityUserId = user.UserId,
                        ParentResellerId = loginResellerId
                    };
                    result = _resellerManager.Update(aReseller);
                }
            return Json(result);

        }
        //
        // GET: /Reseller/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Reseller/Edit/5
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
        // GET: /Reseller/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Reseller/Delete/5
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

        public JsonResult LoadReseller()
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;

            //Reseller reseller = new Reseller { ParentResellerId = loginResellerId };
            List<long> resellerIds = _resellerManager.SelectChildReselersIdsWithParentReseller(loginResellerId);
            List<ResellerEntityVm> list = _resellerManager.SelectResellerVms(new Reseller()).Where(x=>resellerIds.Contains(x.ResellerId)).ToList();
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadResellerLevels()
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;

            List<ResellerLevel> resellerLevels = _resellerManager.SelectResellerLevel(loginResellerLevelId);

            var data = resellerLevels.Select(x => new {val = x.ResellerLevelId, text = x.ResellerLevelNo}).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
         [HttpPost]
        public JsonResult LoadResellerForEdit(long id)
        {
            Dictionary<int, CheckSessionData> IDictionary = CheckSessionData.GetSessionValues();
            long companyId = (long)IDictionary[1].Id;
            long locationId = (long)IDictionary[2].Id;
            long userId = (long)IDictionary[3].Id;

            long loginResellerId = (long)IDictionary[4].Id;
            long loginResellerLevelId = (long)IDictionary[5].Id;


            Reseller reseller = _resellerManager.SelectReseller(new Reseller { ResellerId = id });
            if(reseller!=null)
            {
                ResellerVm vm = new ResellerVm
                {
                    ResellerId=reseller.ResellerId,
                    ResellerLevelId = reseller.ResellerlebelId,
                    Active = reseller.Active == true ? 1 : 0,
                    AuthCode = reseller.AuthCode,
                    EmailAuth = reseller.EmailAuthYn == true ? 1 : 0,
                    FatherName = reseller.Fathername,
                    MotherName = reseller.Mothername,
                    Name = reseller.Name,
                    Email = reseller.Email,
                    UserName = reseller.SecurityUser.Name,
                    ParmanentAddress = reseller.Parmanentaddress,
                    PresentAddress = reseller.Presentaddress,
                    Password = reseller.SecurityUser.Password,
                    PinAuth = reseller.PinAuthYn == true ? 1 : 0,
                    ResellerMobile = reseller.Phone,
                    NidNumber = reseller.NidNumber
                };

                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("vm", JsonRequestBehavior.AllowGet);
            }
            

        }
         
         public ActionResult TotalBalanceDetail(long Id)
         {
             ViewBag.resellerid = Id;
             return View();
         }
         public ActionResult BkashBalanceDetail(long Id)
         {
             ViewBag.resellerid = Id;
             return View();
         }
         [HttpPost]
         public JsonResult LoadBkashTotalBalanceDetail(long Id)
         {
             List<ResellerFund> resellerFunds = _paymentManager.SelectResellerFunds(new ResellerFund { ResellerId = Id }).Where(x => x.FundFor != "F").ToList();

             var data =
                 resellerFunds.Where(x => x.Amount > 0).Select(
                     x =>
                         new PaymentVM
                         {
                             ResellerId = x.ResellerId,
                             Amount = x.Amount,
                             FundFor = x.FundFor == "F" ? "Flexi" : "Bkash",
                             FundType = x.FundType == "A" ? "Addition" : "Deduction",
                             PaymentDate = x.DateOfEntry.ToString()
                         }).ToList();
             return Json(data, JsonRequestBehavior.AllowGet);
         }
         public ActionResult LoadFlexiTotalBalanceDetail(long Id)
         {
             ViewBag.resellerid = Id;
             return View();
         }
         [HttpPost]
         public JsonResult FlexiTotalBalanceDetail(long Id)
         {
             List<ResellerFund> resellerFunds = _paymentManager.SelectResellerFunds(new ResellerFund { ResellerId = Id }).Where(x=>x.FundFor=="F").ToList();

             var data =
                 resellerFunds.Where(x => x.Amount > 0).Select(
                     x =>
                         new PaymentVM
                         {
                             ResellerId = x.ResellerId,
                             Amount = x.Amount,
                             FundFor = x.FundFor == "F" ? "Flexi" : "Bkash",
                             FundType = x.FundType == "A" ? "Addition" : "Deduction",
                             PaymentDate = x.DateOfEntry.ToString()
                         }).ToList();
             return Json(data, JsonRequestBehavior.AllowGet);
         }

    }
}
