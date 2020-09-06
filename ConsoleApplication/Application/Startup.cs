using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApplication.Models.Config;
using Microsoft.Extensions.Options;

namespace ConsoleApplication.Application {
	public class Startup : IStartup {
		private readonly AppSettings _settings;

		public Startup(IOptions<AppSettings> config) {
			_settings = config.Value;
		}

		#region Implementation of IStartup

		/// <inheritdoc />
		public void Run(CancellationToken cancel) {
			Console.WriteLine("config = {0}", _settings.TestSetting);
			var wait = Task.Delay(10000, cancel);
			wait.Wait(cancel);
			Console.WriteLine("Task finished.");
		}

		#endregion
	}
}
