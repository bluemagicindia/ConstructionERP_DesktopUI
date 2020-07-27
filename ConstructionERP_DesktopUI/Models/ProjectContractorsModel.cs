using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionERP_DesktopUI.Models
{
    public class ProjectContractorsModel
    {

        public long ID { get; set; }

        public long? ProjectID { get; set; }

        public long? ContractorID { get; set; }

        public ProjectModel Project { get; set; }

        virtual public ContractorModel Contractor { get; set; }
    }
}
