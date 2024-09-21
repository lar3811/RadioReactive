using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Emitters.Tests
{
	[TestFixture]
	public class PropertyEmitterTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<object>(null, 1, "2");
			var property = new PropertyEmitter<object>();
			var cancel = new Cancel();
			property.Connect(receiver, cancel);
			
			var snapshot = property.Stream.CreateSnapshot();
			property.Stream.AssertConnectionsCount(1);
			
			property.Value = null;
			property.Value = 1;
			property.Value = "2";
			cancel.Request();
			snapshot.AssertReceiverConnectionsDisposed();
			receiver.AssertFinished(false);

			property.Value = "3";
		}
	}
}