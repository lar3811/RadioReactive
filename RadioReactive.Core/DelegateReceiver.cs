using System;

namespace RadioReactive.Core
{
	public sealed class DelegateReceiver<T> : IReceiver<T>
	{
		private readonly Action<T> _next;
		private readonly Action<Exception> _error;
		private readonly Action _complete;
		private readonly Action _final;

		public DelegateReceiver(Action<T> next, 
			Action<Exception> error = null, 
			Action complete = null, 
			Action final = null)
		{
			_next = next;
			_error = error;
			_complete = complete;
			_final = final;
		}

		public void OnCompleted()
		{
			_complete?.Invoke();
			_final?.Invoke();
		}

		public void OnError(Exception error)
		{
			_error?.Invoke(error);
			_final?.Invoke();
		}

		public void OnNext(T value)
		{
			_next?.Invoke(value);
		}
	}
}