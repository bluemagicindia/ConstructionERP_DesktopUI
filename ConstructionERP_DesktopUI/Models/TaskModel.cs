using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class TaskModel
    {
        public long ID { get; set; }

        public long? ProjectID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public long? TaskStatusID { get; set; }

        public long? TaskTypeID { get; set; }

        public long? TeamID { get; set; }

        public int? UserID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public long? StampID { get; set; }

        public long? SheetID { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public SheetModel Sheet { get; set; }

        public ProjectModel Project { get; set; }

        public StampModel Stamp { get; set; }

        public StatusModel Status { get; set; }

        public TeamModel Team { get; set; }

        public TypeModel Type { get; set; }

        public LoggedInUser User { get; set; }



    }
}
