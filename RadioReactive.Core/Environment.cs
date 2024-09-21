using System;
using RadioReactive.Core.Reporting;

namespace RadioReactive.Core
{
	public static class Environment
	{
		private static IReporter _reporter = ConsoleReporter.Singleton;

		public static void SetReporter(IReporter reporter)
		{
			_reporter = reporter;
		}

		public static void Report(Exception exception)
		{
			_reporter.Report(exception);
		}
	}
}