using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Extensions.Tests
{
	[TestFixture]
	public class TakeUntilTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<string>("1");
			var emitter = new StreamEmitter<string>();
			var terminator = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.TakeUntil(terminator).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);
			terminator.AssertConnectionsCount(1);

			var emitterSnapshot = emitter.CreateSnapshot();
			var terminatorSnapshot = terminator.CreateSnapshot();
			
			emitter.OnNext("1");
			terminator.OnNext("terminate");
			receiver.AssertFinished(true);
			emitterSnapshot.AssertReceiverConnectionsDisposed();
			terminatorSnapshot.AssertReceiverConnectionsDisposed();
			terminator.OnCompleted();
			
			emitter.OnNext("2");
			emitter.OnNext("3");
			emitter.OnCompleted();
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test2()
		{
			var receiver = new TestReceiver<string>("1", "2", "3");
			var emitter = new StreamEmitter<string>();
			var terminator = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.TakeUntil(terminator).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);
			terminator.AssertConnectionsCount(1);

			var emitterSnapshot = emitter.CreateSnapshot();
			var terminatorSnapshot = terminator.CreateSnapshot();
			
			emitter.OnNext("1");
			emitter.OnNext("2");
			emitter.OnNext("3");
			emitter.OnCompleted();
			emitterSnapshot.AssertComplete();
			terminatorSnapshot.AssertReceiverConnectionsDisposed();
			receiver.AssertFinished(true);
			
			terminator.OnNext("terminate");
			terminator.OnCompleted();
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test3()
		{
			var receiver = new TestReceiver<string>();
			var emitter = new StreamEmitter<string>();
			var terminator = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.TakeUntil(terminator).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);
			terminator.AssertConnectionsCount(1);

			var emitterSnapshot = emitter.CreateSnapshot();
			var terminatorSnapshot = terminator.CreateSnapshot();
			cancel.Request();
			receiver.AssertFinished(false);
			emitterSnapshot.AssertReceiverConnectionsDisposed();
			terminatorSnapshot.AssertReceiverConnectionsDisposed();
			
			emitter.OnNext("1");
			emitter.OnNext("2");
			emitter.OnNext("3");
			emitter.OnCompleted();
			
			terminator.OnNext("terminate");
			terminator.OnCompleted();
		}
		
		[Test]
		public void Test4()
		{
			var receiver = new TestReceiver<string>();
			var sourceEmitter = new PlaybackEmitter<string>("1", "2", "3");
			var signalEmitter = new PlaybackEmitter<string>("terminate");
			var cancel = new Cancel();
			sourceEmitter.TakeUntil(signalEmitter).Connect(receiver, cancel);
			receiver.AssertFinished(true);
		}
	}
}