using System.ComponentModel;

namespace task_management_api.Enums;

public enum TaskProgressStatus
{
    [Description("Pending")]
    PENDING,
    [Description("In Progress")]
    INPROGRESS,
    [Description("Completed")]
    COMPLETED,
    [Description("Overdue")]
    OVERDUE

}
