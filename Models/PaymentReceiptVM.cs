using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoFlexiManagementSystem.Models
{
    public class PaymentReceiptVM
    {
        public string ReceiptNo { set; get; }
        public string ResellerName { set; get; }
        public string ResellerRoll { set; get; }
        public DateTime NextPaymentDate { set; get; }
        public double AdmissionFee { set; get; }
        public double PaymentAmount { set; get; }
        public double Discount { set; get; }
        public double Due { set; get; }
        public string DepartmentName { set; get; }
        public double Vat { set; get; }
        public DateTime PaymentDate { set; get; }
    }
}