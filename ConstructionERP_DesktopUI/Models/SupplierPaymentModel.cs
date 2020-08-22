using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionERP_DesktopUI.Models
{
    public class SupplierPaymentModel
    {
        public long ID { get; set; }

        public long? SupplierBillID { get; set; }

        public long Amount { get; set; }

        public string Remarks { get; set; }

        public DateTime PaidOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Status { get; set; }

        public SupplierBillModel SupplierBill { get; set; }


    }
}
