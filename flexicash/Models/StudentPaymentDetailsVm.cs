using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoFlexiManagementSystem.Models
{
    public class ResellerPaymentDetailsVm
    {
        public long ResellerPaymentId { set; get; }
        public string PaymentReceiptNo { set; get; }
        public string PaymentDate { set; get; }
        public long EntryBy { set; get; }
        public string SecurityUser { set; get; }
        public double PaymentAmount { set; get; }
        public string NextPaymentDate { set; get; }
        public string DateOfEntry { set; get; }
        public string Print { set; get; }
        public string Edit { set; get; }
        public string Delete { set; get; }
    }
}