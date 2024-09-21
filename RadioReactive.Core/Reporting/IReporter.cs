using System;

namespace RadioReactive.Core.Reporting
{
	public interface IReporter
	{
		void Report(Exception error);
	}
}