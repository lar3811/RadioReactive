using System;
using RadioReactive.Core;
using RadioReactive.Core.Reporting;
using RadioReactive.Emitters;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Emitters.Tests
{
	[TestFixture]
	public class StreamEmitterTests
	{
		[Test]
		public void Test1()
		{
			var receiver = new TestReceiver<object>(null, 1, "2");
			var emitter = new StreamEmitter<object>();
			var cancel = new Cancel();
			emitter.Connect(receiver, cancel);
			
			var snapshot = emitter.CreateSnapshot();
			emitter.OnNext(null);
			emitter.OnNext(1);
			emitter.OnNext("2");
			emitter.OnCompleted();
			snapshot.AssertComplete();
			receiver.AssertFinished(true);
		}
		
		[Test]
		public void Test2()
		{
			var receiver = new TestReceiver<string>();
			var exception = new Exception("testException");
			var emitter = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.Connect(receiver, cancel);
			
			var snapshot = emitter.CreateSnapshot();
			emitter.OnError(exception);
			snapshot.AssertError(exception);
			receiver.AssertFinished(true);
		}
		
		[Test]
		public void Test3()
		{
			var receiver = new TestReceiver<string>();
			var emitter = new StreamEmitter<string>();
			var cancel = new Cancel();
			emitter.Connect(receiver, cancel);
			
			var snapshot = emitter.CreateSnapshot();
			emitter.OnCompleted();
			snapshot.AssertComplete();
			receiver.AssertFinished(true);
		}
	}
}