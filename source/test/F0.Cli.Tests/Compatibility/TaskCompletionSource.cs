#if !HAS_TASKCOMPLETIONSOURCE
namespace System.Threading.Tasks
{
	internal class TaskCompletionSource
	{
		private readonly TaskCompletionSource<object?> tcs;

		public TaskCompletionSource()
		{
			tcs = new TaskCompletionSource<object?>();
		}

		public Task Task => tcs.Task;

		public void SetResult()
		{
			tcs.SetResult(null);
		}

		public bool TrySetResult()
		{
			return tcs.TrySetResult(null);
		}
	}
}
#endif
