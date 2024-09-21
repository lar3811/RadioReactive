using RadioReactive.Core.Reporting;
using NUnit.Framework;

namespace RadioReactive.Extensions.Tests
{
	[SetUpFixture]
	public sealed class Environment
	{
		[OneTimeSetUp]
		public void Setup()
		{
			RadioReactive.Core.Environment.SetReporter(ExceptionReporter.Singleton);
		}
	}
}