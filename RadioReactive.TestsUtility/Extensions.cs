using RadioReactive.Emitters;
using NUnit.Framework;

namespace RadioReactive.TestsUtility
{
	public static class Extensions
	{
		public static StreamEmitterSnapshot<T> CreateSnapshot<T>(this StreamEmitter<T> stream)
		{
			return new StreamEmitterSnapshot<T>(stream);
		}
		
		public static void AssertConnectionsCount<T>(this StreamEmitter<T> stream, int count)
		{
			var core = stream.Core as StreamEmitter<T>.StreamingCore;
			if (core == null)
				Assert.AreEqual(count, 0);
			else
				Assert.AreEqual(count, core.Core.Connections.Count);
		}
	}
}