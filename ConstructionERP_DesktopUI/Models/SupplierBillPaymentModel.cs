using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionERP_DesktopUI.Models
{
    public class SupplierBillPaymentModel
    {
        public long ID { get; set; }
        public long PaymentID { get; set; }
        public DateTime Date { get; set; }
        public string BillNo { get; set; }
        public long BillAmount { get; set; }
        public long PaymentAmount { get; set; }
        public string PaymentMode { get; set; }
        public bool IsBill { get; set; }
        public long Pending { get; set; }
    }
}
