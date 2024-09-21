using System;

namespace RadioReactive.Core
{
	public sealed class VoidReceiver<T> : IReceiver<T>
	{
		public static readonly VoidReceiver<T> Instance = new VoidReceiver<T>();
		
		private VoidReceiver() { }
		
		public void OnCompleted() { }

		public void OnError(Exception error) { }

		public void OnNext(T value) { }
	}
}