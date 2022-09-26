using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSKPOC.Models
{
    public class ItemPrice
    {
        public int WorkOrderNo { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal NewPrice { get; set; }
        public bool? IsAdminApproved { get; set; }
        public bool? IsSupervisorApproved { get; set; }
    }
}
