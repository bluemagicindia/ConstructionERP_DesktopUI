using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class CustomerPaymentModel
    {
        public long ID { get; set; }

        public long? CustomerID { get; set; }

        public long? FlatID { get; set; }

        public decimal AggregateAmountReceived { get; set; }

        public decimal ExtraWorkTotal { get; set; }

        public decimal ExtraWorkReceived { get; set; }

        public decimal GSTReceived { get; set; }

        public string StampDutyBalance { get; set; }

        public DateTime PaymentDate { get; set; }

        public long? PaymentModeID { get; set; }

        public PaymentModeModel PaymentMode { get; set; }
        public string Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Status { get; set; }

        public CustomerModel Customer { get; set; }

        public FlatModel Flat { get; set; }
    }
}
