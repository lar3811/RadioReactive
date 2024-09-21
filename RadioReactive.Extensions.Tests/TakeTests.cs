using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Extensions.Tests
{
	[TestFixture]
	public class TakeTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<string>("1", "2");
			var emitter = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.Take(2).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);

			var snapshot = emitter.CreateSnapshot();
			emitter.OnNext("1");
			emitter.OnNext("2");
			receiver.AssertFinished(true);
			snapshot.AssertReceiverConnectionsDisposed();
			
			emitter.OnNext("3");
			emitter.OnNext("4");
			emitter.OnCompleted();
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test2()
		{
			var receiver = new TestReceiver<string>();
			var emitter = new StreamEmitter<string>();
			var cancel = new Cancel();

			emitter.OnCompleted();
			
			emitter.Take(2).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(0);
			receiver.AssertFinished(true);
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test3()
		{
			var receiver = new TestReceiver<string>("1");
			var emitter = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.Take(2).Connect(receiver, cancel);
			emitter.AssertConnectionsCount(1);
			
			var snapshot = emitter.CreateSnapshot();
			emitter.OnNext("1");
			cancel.Request();
			snapshot.AssertReceiverConnectionsDisposed();
			
			emitter.OnNext("2");
			emitter.OnNext("3");
			emitter.OnNext("4");
			emitter.OnCompleted();
			receiver.AssertFinished(false);
		}
		
		[Test]
		public void Test4()
		{
			var receiver = new TestReceiver<string>("1", "2");
			var emitter = new PlaybackEmitter<string>("1", "2", "3", "4");
			var cancel = new Cancel(); 
			emitter.Take(2).Connect(receiver, cancel);
			receiver.AssertFinished(true);
			Assert.DoesNotThrow(() => cancel.Request());
		}

		[Test]
		public void Test5()
		{
			var receiver = new TestReceiver<object>("1");
			var property = new PropertyEmitter<object>("1");
			var cancel = new Cancel();
			property.Take(1).Connect(receiver, cancel);
			property.Stream.AssertConnectionsCount(0);
		}
	}
}