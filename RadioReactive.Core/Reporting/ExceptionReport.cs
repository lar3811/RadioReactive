using System;

namespace RadioReactive.Core.Reporting
{
	public sealed class ExceptionReport : Exception
	{
		public ExceptionReport(string message) : base(message)
		{
			
		}
	}
}