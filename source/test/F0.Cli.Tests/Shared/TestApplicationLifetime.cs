using System;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace F0.Tests.Shared
{
	internal sealed class TestApplicationLifetime : IHostApplicationLifetime
	{
		private readonly Action onStopping;

		public TestApplicationLifetime(Action onStopping)
		{
			this.onStopping = onStopping;
		}

		CancellationToken IHostApplicationLifetime.ApplicationStarted { get; }
		CancellationToken IHostApplicationLifetime.ApplicationStopping { get; }
		CancellationToken IHostApplicationLifetime.ApplicationStopped { get; }

		void IHostApplicationLifetime.StopApplication()
		{
			onStopping();
		}
	}
}
