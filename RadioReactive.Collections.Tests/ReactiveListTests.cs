using System;
using RadioReactive.Core;
using RadioReactive.Extensions;
using RadioReactive.TestsUtility;
using NUnit.Framework;

namespace RadioReactive.Collections.Tests
{
	[TestFixture]
	public class ReactiveListTests
	{
		[Test]
		public void Test1()
		{
			var cancel = new Cancel();
			var addedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var removedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var list = new ReactiveList<object>();
			list.ItemAdded.Connect(addedItemsReceiver, cancel);
			list.ItemRemoved.Connect(removedItemsReceiver, cancel);
			list.Add(null);
			list.Add("1");
			list.Add(2);
			list.Clear();
			Assert.AreEqual(list.Count, 0);
			
			list.Dispose();
			addedItemsReceiver.AssertFinished(true);
			removedItemsReceiver.AssertFinished(true);
		}
		
		[Test]
		public void Test2()
		{
			var cancel = new Cancel();
			var addedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var removedItemsReceiver = new TestReceiver<object>(2, "1", null);
			var list = new ReactiveList<object>();
			list.ItemAdded.Connect(addedItemsReceiver, cancel);
			list.ItemRemoved.Connect(removedItemsReceiver, cancel);
			list.Add(null);
			list.Add("1");
			list.Add(2);
			list.RemoveAt(2);
			list.RemoveAt(1);
			list.RemoveAt(0);
			Assert.AreEqual(list.Count, 0);
			
			list.Dispose();
			addedItemsReceiver.AssertFinished(true);
			removedItemsReceiver.AssertFinished(true);
		}
		
		[Test]
		public void Test3()
		{
			var cancel = new Cancel();
			var addedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var removedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var list = new ReactiveList<object>();
			list.ItemAdded.Connect(addedItemsReceiver, cancel);
			list.ItemRemoved.Connect(removedItemsReceiver, cancel);
			list.Add(null);
			list.Add("1");
			list.Add(2);
			list.Remove(null);
			list.Remove("1");
			list.Remove(2);
			Assert.AreEqual(list.Count, 0);
			
			list.Dispose();
			addedItemsReceiver.AssertFinished(true);
			removedItemsReceiver.AssertFinished(true);
		}
		
		[Test]
		public void Test4()
		{
			var cancel = new Cancel();
			var addedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var removedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var list = new ReactiveList<object>();
			list.ItemAdded.Connect(addedItemsReceiver, cancel);
			list.ItemRemoved.Connect(removedItemsReceiver, cancel);
			list.Add(null);
			list[0] = null;
			list[0] = "1";
			list[0] = 2;
			Assert.AreEqual(list.Count, 1);
			
			list.Dispose();
			Assert.AreEqual(list.Count, 0);
			addedItemsReceiver.AssertFinished(true);
			removedItemsReceiver.AssertFinished(true);
		}
		
		[Test]
		public void Test5()
		{
			var cancel = new Cancel();
			var addedItemsReceiver = new TestReceiver<object>("1", null, 2);
			var removedItemsReceiver = new TestReceiver<object>(null, "1", 2);
			var list = new ReactiveList<object>();
			list.ItemAdded.Connect(addedItemsReceiver, cancel);
			list.ItemRemoved.Connect(removedItemsReceiver, cancel);
			list.Insert(0, "1");
			list.Insert(0, null);
			list.Insert(2, 2);
			list.Clear();
			Assert.AreEqual(list.Count, 0);
			
			list.Dispose();
			addedItemsReceiver.AssertFinished(true);
			removedItemsReceiver.AssertFinished(true);
		}
		
		[Test]
		public void Test6()
		{
			var receiver = new TestReceiver<object>(null, "1", 2);
			var list = new ReactiveList<object>();
			list.ItemAdded.Take(1).Connect(item => list.Add("1"));
			list.ItemAdded.Skip(1).Take(1).Connect(item => list.Add(2));
			list.ItemAdded.Connect(receiver);
			list.Add(null);
			Assert.AreEqual(list.Count, 3);
			
			list.Dispose();
			receiver.AssertFinished(true);
		}

		[Test]
		public void Test7()
		{
			var list = new ReactiveList<object>();
			
			foreach (var item in list)
			{
				Assert.Fail();
			}

			list.Add(null);
			list.Add("1");
			list.Add(2);
			var iterations = 0;
			foreach (var item in list)
			{
				switch (iterations++)
				{
					case 0: Assert.AreEqual(item, null); break;
					case 1: Assert.AreEqual(item, "1"); break;
					case 2: Assert.AreEqual(item, 2); break;
					default: Assert.Fail(); break;
				}
			}

			var enumerator = list.GetEnumerator();
			Assert.True(enumerator.MoveNext());
			list.Clear();
			Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
		}
	}
}