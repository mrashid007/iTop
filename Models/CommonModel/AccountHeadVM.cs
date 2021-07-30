using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoFlexiManagementSystem.Models.CommonModel
{
    public class AccountHeadVM
    {
        public string Code { set; get; }
        public string Name { set; get; }
        public long AccountGroupId { set; get; }
        public long AccountSubGroupId { set; get; }
        public long AccountControlId { set; get; }
        public long AccountSubSidiaryId { set; get; }
        public double OpeningBalance { set; get; }
    }
}