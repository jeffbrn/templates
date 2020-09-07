using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApplication.Models.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApplication.Application {
	public class Startup : IStartup {
		private readonly AppSettings _settings;
		private readonly ILogger _log;

		public Startup(IOptions<AppSettings> config, ILogger<Startup> log) {
			_settings = config.Value;
			_log = log;
		}

		#region Implementation of IStartup

		/// <inheritdoc />
		public void Run(CancellationToken cancel) {
			_log.LogInformation("config = {0}", _settings.TestSetting);
			var wait = Task.Delay(10000, cancel);
			wait.Wait(cancel);
			_log.LogWarning("Task finished.");
		}

		#endregion
	}
}
