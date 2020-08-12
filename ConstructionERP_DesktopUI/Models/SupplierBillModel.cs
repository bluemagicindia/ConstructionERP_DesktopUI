using System;
using System.Collections.Generic;

namespace ConstructionERP_DesktopUI.Models
{
    public class SupplierBillModel
    {
        public long ID { get; set; }

        public long? SupplierID { get; set; }

        public long? ProjectID { get; set; }

        public string BillNo { get; set; }

        public DateTime BillDate { get; set; }

        public long Amount { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Status { get; set; }

        public SupplierModel Supplier { get; set; }

        public ProjectModel Project { get; set; }

        public IEnumerable<SupplierPaymentModel> SupplierPayments { get; set; }

    }
}
