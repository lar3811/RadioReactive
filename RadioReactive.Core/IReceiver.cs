using System;

namespace RadioReactive.Core
{
	public interface IReceiver<in T>
	{
		void OnNext(T value);
		void OnError(Exception error);
		void OnCompleted();
	}
}