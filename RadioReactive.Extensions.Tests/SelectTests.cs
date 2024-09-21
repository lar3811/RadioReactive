using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Extensions.Tests
{
	[TestFixture]
	public class SelectTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<string>("1", "2", "3");
			var emitter = new StreamEmitter<int>();
			var cancel = new Cancel();
			emitter.Select(value => value.ToString()).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);
			var snapshot = emitter.CreateSnapshot();
			
			emitter.OnNext(1);
			emitter.OnNext(2);
			emitter.OnNext(3);
			emitter.OnCompleted();
			snapshot.AssertComplete();
			receiver.AssertFinished(true);
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test2()
		{
			var receiver = new TestReceiver<string>("1", "2", "3");
			var emitter = new PlaybackEmitter<int>(1, 2, 3);
			var cancel = new Cancel();
			emitter.Select(value => value.ToString()).Connect(receiver, cancel);
			receiver.AssertFinished(true);
		}
	}
}