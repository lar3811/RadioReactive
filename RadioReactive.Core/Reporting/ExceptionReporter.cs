using System;

namespace RadioReactive.Core.Reporting
{
	public sealed class ExceptionReporter : IReporter
	{
		public static readonly ExceptionReporter Singleton = new ExceptionReporter();

		private ExceptionReporter() { }

		public void Report(Exception error)
		{
			throw error;
		}
	}
}