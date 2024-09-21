using System;
using RadioReactive.Core;

namespace RadioReactive.Emitters
{
	partial class StreamEmitter<T>
	{
#if DEBUG
		public
#else
		private
#endif
		sealed class BrokenCore : IEmitterCore<T>
		{
#if DEBUG
			public Exception Error => _error;
#endif
			private readonly Exception _error;

			public BrokenCore(Exception error)
			{
				_error = error;
			}
			
			public void Connect(IReceiver<T> receiver, ICancel cancel)
			{
				if (!cancel.IsRequested)
					receiver.OnError(_error);
			}

			public void OnNext(T value) { }

			public void OnError(Exception error) { }

			public void OnCompleted() { }
		}
	}
}