using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoFlexiManagementSystem.Models
{
    public class ResellerVm
    {
        public long ResellerId { set; get; }
        public string Name { set; get; }
        public string FatherName { set; get; }
        public string MotherName { set; get; }
        public string PresentAddress { set; get; }
        public string ParmanentAddress { set; get; }
        public string ResellerMobile { set; get; }
        public string NidNumber { set; get; }
        public string Email { set; get; }
        public string Remarks { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public int Active { set; get; }
        public string AuthCode { set; get; }
        public int EmailAuth { set; get; }
        public int PinAuth { set; get; }       
        public long ResellerLevelId { set; get; }
    }
}