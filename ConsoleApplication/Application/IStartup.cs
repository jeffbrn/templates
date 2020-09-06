using System;
using System.Threading;

namespace ConsoleApplication.Application {
	public interface IStartup {
		void Run(CancellationToken cancel);
	}
}
