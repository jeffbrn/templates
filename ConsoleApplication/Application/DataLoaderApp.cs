using System;
using ConsoleApplication.Models.Config;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApplication.Application {
	public class DataLoaderApp : ConsoleAppBase {
		/// <inheritdoc />
		public DataLoaderApp(string[] args) : base(args) { }

		protected override void RegisterServices(IServiceCollection services) {
			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
		}
	}
}
