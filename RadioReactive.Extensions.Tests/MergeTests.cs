using System.ComponentModel;
using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Extensions.Tests
{
	[TestFixture]
	public class MergeTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<object>(null, "1", 2);
			var emitter1 = new StreamEmitter<object>();
			var emitter2 = new StreamEmitter<object>();
			var emitter3 = new StreamEmitter<object>();
			var cancel = new Cancel();
			
			emitter1.Merge(emitter2).Merge(emitter3).Connect(receiver);
			var snapshot1 = emitter1.CreateSnapshot();
			var snapshot2 = emitter2.CreateSnapshot();
			var snapshot3 = emitter3.CreateSnapshot();
			
			emitter1.OnNext(null);
			emitter1.OnCompleted();
			emitter2.OnNext("1");
			emitter2.OnCompleted();
			emitter3.OnNext(2);
			emitter3.OnCompleted();

			receiver.AssertFinished(true);
			snapshot1.AssertComplete();
			snapshot2.AssertComplete();
			snapshot3.AssertComplete();
			Assert.False(cancel.IsRequested);
		}
		
		[Test]
		public void Test2()
		{
			var receiver = new TestReceiver<object>(null, "1", 2);
			var emitter1 = new StreamEmitter<object>();
			var emitter2 = new StreamEmitter<object>();
			var emitter3 = new StreamEmitter<object>();
			var cancel = new Cancel();
			
			emitter1.Merge(emitter2).Merge(emitter3).Connect(receiver, cancel);
			emitter1.OnNext(null);
			emitter2.OnNext("1");
			emitter3.OnNext(2);
			
			cancel.Request();
			
			emitter1.OnCompleted();
			emitter2.OnCompleted();
			emitter3.OnCompleted();

			receiver.AssertFinished(false);
			Assert.True(cancel.IsRequested);
		}
	}
}