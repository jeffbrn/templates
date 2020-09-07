using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleApplication.Application {
	public abstract class ConsoleAppBase : IDisposable {
		private readonly object _sync = new object();
		private readonly ManualResetEventSlim _isRunning;
		private readonly IServiceCollection _services;
		private ServiceProvider _svcProvider;

		public ConsoleAppBase(string[] args) {
			var runtimeEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
			_services = new ServiceCollection();
			Configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
				.AddJsonFile($"appsettings.{runtimeEnv}.json", optional: true, reloadOnChange: false)
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();
			_services.AddOptions();
			_services.AddLogging(c => c.AddConsole());
			_isRunning = new ManualResetEventSlim(false);
		}

		public ManualResetEventSlim Run<Starter>(CancellationToken cancel) where Starter : IStartup {
			RegisterServices(_services);
			lock (_sync) {
				_svcProvider = _services.BuildServiceProvider();
				RootScope = _svcProvider.CreateScope();
			}

			var startup = InstantiateStarter(typeof(Starter));

			Task.Run(() => {
				try {
					startup.Run(cancel);
				} catch (OperationCanceledException) { }

				_isRunning.Set();
			}, cancel);

			return _isRunning;
		}


		private IStartup InstantiateStarter(Type starter) {
			var ctor = starter.GetConstructors().FirstOrDefault(x => x.IsPublic)
						?? throw new ArgumentException($"class '{starter.Name}' doesn't have public constructor");
			var parms = new List<object>();
			foreach (var p in ctor.GetParameters()) {
				var o = RootScope.ServiceProvider.GetService(p.ParameterType)
						?? throw new ArgumentException($"Unable to inject parameter {p.ParameterType.Namespace} in Starter");
				parms.Add(o);
			}

			var retval = ctor.Invoke(parms.ToArray());
			return (IStartup) retval;
		}

		protected IConfiguration Configuration { get; }
		protected IServiceScope RootScope { get; private set; }

		protected abstract void RegisterServices(IServiceCollection services);

		#region IDisposable

		protected virtual void Dispose(bool disposing) {
			lock (_sync) {
				if (disposing) {
					RootScope.Dispose();
					_svcProvider.Dispose();
					_isRunning.Dispose();
				}
			}
		}

		/// <inheritdoc />
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		~ConsoleAppBase() {
			Dispose(false);
		}

		#endregion
	}
}
