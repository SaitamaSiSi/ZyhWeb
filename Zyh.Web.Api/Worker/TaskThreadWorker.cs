using Zyh.Common.Threading;

namespace Zyh.Web.Api.Worker
{
    public class TaskThreadWorker : QueueWorker<string>, ITaskThreadWorker
    {
        public override void Start()
        {
            base.Start();
        }

        protected override void DoWork()
        {
            Thread.Sleep(3000);
            Console.WriteLine("In DoWork");
        }
    }
}
