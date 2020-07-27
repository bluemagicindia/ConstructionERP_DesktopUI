namespace ConstructionERP_DesktopUI.Models
{
    public class ProjectSuppliersModel
    {

        public long ID { get; set; }

        public long? ProjectID { get; set; }

        public long? SupplierID { get; set; }

        public ProjectModel Project { get; set; }

        virtual public SupplierModel Supplier { get; set; }
    }
}
