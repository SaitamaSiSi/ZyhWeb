using System.ComponentModel;

namespace Zyh.Common.Threading
{
    public enum WorkerStatus
    {
        [Description("Created")]
        Created = 0,

        [Description("Running")]
        Running,

        [Description("Paused")]
        Paused,

        [Description("Stopped")]
        Stopped,

        [Description("Disposed")]
        Disposed
    }
}
