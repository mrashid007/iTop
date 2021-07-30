using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoFlexiEntity.Model;
using AutoFlexiEntity.VMEntity;
using AutoFlexiManagementSystem.AutoFlexiEntity.CommonModel;
using AutoFlexiManager.Manager;

namespace AutoFlexiManagementSystem.Controllers
{
    public class ApprovalController : Controller
    {
        FlexiloadManager _flexiloadManager=new FlexiloadManager();
        BkashManager _bkashManager=new BkashManager();
        PaymentManager _paymentManager=new PaymentManager();
        //
        // GET: /Approval/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Approval/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Approval/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Approval/Create
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
        // GET: /Approval/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Approval/Edit/5
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
        // GET: /Approval/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Approval/Delete/5
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

        public ActionResult FlexiloadApproval()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoadUnApprovedFlexiloads()
        {
            List<Flexiload> flexiloads = _flexiloadManager.SelectUnApprovedFlexiloads(new Flexiload());

            var data =
                flexiloads.Select(
                    x =>
                        new FlexiloadVM
                        {
                            Amount = x.Amount,
                            MobileNumber = x.MobileNumber,
                            Status = x.Status == "N" ? "Processing" : x.Status == "Y" ? "Success" : "Processing",
                            Remarks = x.Remarks,
                            ResellerName = x.Reseller.Name,
                            SendButton = "<div><input type='button' class='br-buttons' value='Send' onclick='ApproveFlexiload("+x.FlexiloadId+")'/> </div>",
                            CancelButton ="<div><input type='button' class='br-buttons' value='Cancel' onclick='CancelFlexiload(" + x.FlexiloadId + ")'/> </div>"
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BkashApproval()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoadUnApprovedBkash()
        {
            List<Bkash> bkashes = _bkashManager.SelectUnApprovedBkashes(new Bkash());

            var data =
                bkashes.Select(
                    x =>
                        new BkashVM
                        {
                            Amount = x.Amount,
                            MobileNumber = x.BkashNumber,
                            Status = x.Status == "N" ? "Processing" : x.Status == "Y" ? "Success" :x.Status == "C" ? "Cancel":"Processing",
                            Remarks = x.Remarks,
                            ResellerName = x.Reseller.Name,
                            //TransectionId = x.TransectionId == null ?" " : x.TransectionId,
                            SendButton = x.TransectionId==null?"<div><input type='button' class='br-buttons' value='Send' onclick='ApproveBkash(" + x.BkashId + ")'/> </div>":"",
                            CancelButton = x.TransectionId==null?"<div><input type='button' class='br-buttons' value='Cancel' onclick='CancelBkash(" + x.BkashId + ")'/> </div>":""
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Approved(long id, string type, string status, string transactionId)
        {
            Result result=new Result();

            switch (type)
            {
                case "F":
                    Flexiload flexiload =
                        _flexiloadManager.SelectFlexiloads(new Flexiload {FlexiloadId = id}).FirstOrDefault();
                    
                    flexiload.Status = status;
                    flexiload.DateofAction = DateTime.Now;
                    flexiload.TransectionId = transactionId;
                   result= _flexiloadManager.Update(flexiload);
                    break;
                case "B":
                   Bkash bkash =
                        _bkashManager.SelectBkashs(new Bkash{ BkashId = id }).FirstOrDefault();
                   bkash.Status = status;
                   bkash.DateofAction = DateTime.Now;
                    bkash.TransectionId = transactionId;
                   result = _bkashManager.Update(bkash);
                    break;
                  
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
