using System;
using System.Threading;
using ConsoleApplication.Application;

namespace ConsoleApplication {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Press any key to quit ...");
			using var app = new DataLoaderApp(args);
			var cancel = new CancellationTokenSource();
			var wait = app.Run<Startup>(cancel.Token);
			while (!wait.IsSet) {
				if (Console.KeyAvailable) {
					cancel.Cancel();
				}
				Thread.Sleep(500);
			}
			// ReSharper disable once MethodSupportsCancellation
			wait.Wait();
			Console.WriteLine("Exiting");
		}
	}
}
