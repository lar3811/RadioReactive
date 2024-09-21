using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Extensions.Tests
{
	[TestFixture]
	public class SkipUntilTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<string>("2", "3");
			var emitter = new StreamEmitter<string>();
			var trigger = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.SkipUntil(trigger).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(0);
			trigger.AssertConnectionsCount(1);
			
			var triggerSnapshot = trigger.CreateSnapshot();
			
			emitter.OnNext("1");
			trigger.OnNext("activate");
			triggerSnapshot.AssertReceiverConnectionsDisposed();
			emitter.AssertConnectionsCount(1);
			var emitterSnapshot = emitter.CreateSnapshot();
			trigger.OnCompleted();
			
			emitter.OnNext("2");
			emitter.OnNext("3");
			emitter.OnCompleted();
			emitterSnapshot.AssertComplete();
			receiver.AssertFinished(true);
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test2()
		{
			var receiver = new TestReceiver<string>();
			var emitter = new StreamEmitter<string>();
			var trigger = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.SkipUntil(trigger).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(0);
			trigger.AssertConnectionsCount(1);
			
			var triggerSnapshot = trigger.CreateSnapshot();
			
			emitter.OnNext("1");
			emitter.OnNext("2");
			emitter.OnNext("3");
			emitter.OnCompleted();
			
			trigger.OnNext("activate");
			triggerSnapshot.AssertEmitterConnectionsDisposed();
			emitter.AssertConnectionsCount(0);
			receiver.AssertFinished(true);
			trigger.OnCompleted();
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test3()
		{
			var receiver = new TestReceiver<string>();
			var emitter = new StreamEmitter<string>();
			var trigger = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.SkipUntil(trigger).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(0);
			trigger.AssertConnectionsCount(1);
			
			var triggerSnapshot = trigger.CreateSnapshot();
			cancel.Request();
			triggerSnapshot.AssertReceiverConnectionsDisposed();
			
			emitter.OnNext("1");
			emitter.OnNext("2");
			emitter.OnNext("3");
			emitter.OnCompleted();
			
			trigger.OnNext("activate");
			trigger.OnCompleted();
			
			receiver.AssertFinished(false);
		}
		
		[Test]
		public void Test4()
		{
			var receiver = new TestReceiver<string>("1", "2", "3");
			var emitter = new PlaybackEmitter<string>("1", "2", "3");
			var trigger = new PlaybackEmitter<string>("activate");
			var cancel = new Cancel();
			emitter.SkipUntil(trigger).Connect(receiver, cancel);
			receiver.AssertFinished(true);
		}
	}
}