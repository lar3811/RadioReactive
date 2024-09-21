using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Extensions.Tests
{
	[TestFixture]
	public sealed class WhereTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<string>("22", "44");
			var emitter = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.Where(value => value.Length == 2).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);

			var snapshot = emitter.CreateSnapshot();
			emitter.OnNext("1");
			emitter.OnNext("22");
			emitter.OnNext("333");
			emitter.OnNext("44");
			emitter.OnNext("5");
			
			emitter.OnCompleted();
			receiver.AssertFinished(true);
			snapshot.AssertComplete();
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test2()
		{
			var receiver = new TestReceiver<string>("22");
			var emitter = new StreamEmitter<string>();
			var cancel = new Cancel(); 
			emitter.Where(value => value.Length == 2).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);

			var snapshot = emitter.CreateSnapshot();
			emitter.OnNext("1");
			emitter.OnNext("22");
			emitter.OnNext("333");
			cancel.Request();
			receiver.AssertFinished(false);
			snapshot.AssertReceiverConnectionsDisposed();
			
			emitter.OnNext("44");
			emitter.OnNext("5");
			emitter.OnCompleted();
		}

		[Test]
		public void Test3()
		{
			var receiver = new TestReceiver<string>("22", "44");
			var emitter = new PlaybackEmitter<string>("1", "22", "333", "44", "5");
			var cancel = new Cancel();
			emitter.Where(value => value.Length == 2).Connect(receiver, cancel);
			receiver.AssertFinished(true);
		}
	}
}