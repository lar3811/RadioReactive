using System;
using RadioReactive.Core;

namespace RadioReactive.Emitters.Signals
{
#if DEBUG
	public
#else
	internal
#endif
	struct ErrorSignal<T> : ISignal<T>
	{
#if DEBUG
		public Exception Exception => _exception; 
#endif

		private readonly Exception _exception;

		public ErrorSignal(Exception exception)
		{
			_exception = exception;
		}

		public void Tune(IReceiver<T> receiver)
		{
			receiver.OnError(_exception);
		}
	}
}