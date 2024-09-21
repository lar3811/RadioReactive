using System;
using System.Collections.Generic;
using System.Linq;
using RadioReactive.Core;
using NUnit.Framework;

namespace RadioReactive.TestsUtility
{
	public sealed class TestReceiver<T> : IReceiver<T>
	{
		private bool _finished;

		private readonly IEnumerator<T> _expectedValue;

		public TestReceiver(params T[] expectedValues) : this(expectedValues.AsEnumerable())
		{
		}
		
		public TestReceiver(IEnumerable<T> expectedValues)
		{
			_expectedValue = expectedValues.GetEnumerator();
		}
		
		public void OnNext(T value)
		{
			Assert.False(_finished);
			Assert.True(_expectedValue.MoveNext());
			Assert.AreEqual(_expectedValue.Current, value);
		}

		public void OnError(Exception error)
		{
			Assert.False(_finished);
			Assert.False(_expectedValue.MoveNext());
			_finished = true;
		}

		public void OnCompleted()
		{
			Assert.False(_finished);
			Assert.False(_expectedValue.MoveNext());
			_finished = true;
		}

		public void AssertFinished(bool finished)
		{
			Assert.AreEqual(_finished, finished);
		}
	}
}