using System;

namespace RadioReactive.Core
{
	public sealed class SealedReceiver<T> : IReceiver<T>
	{
		public static readonly SealedReceiver<T> Instance = new SealedReceiver<T>();
		
		private SealedReceiver() { }
		
		public void OnCompleted()
		{
			Environment.Report(new InvalidOperationException());
		}

		public void OnError(Exception error)
		{
			Environment.Report(new InvalidOperationException());
		}

		public void OnNext(T value)
		{
			Environment.Report(new InvalidOperationException());
		}
	}
}