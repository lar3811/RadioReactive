using System;

namespace RadioReactive.Core.Reporting
{
	public sealed class ConsoleReporter : IReporter
	{
		public static readonly ConsoleReporter Singleton = new ConsoleReporter();

		private ConsoleReporter() { }
		
		public void Report(Exception error)
		{
			Console.Error.WriteLine(error.ToString());
		}
	}
}