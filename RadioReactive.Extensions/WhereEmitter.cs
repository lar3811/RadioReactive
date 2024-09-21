using System;
using RadioReactive.Core;

namespace RadioReactive.Extensions
{
	public sealed class WhereEmitter<T> : IEmitter<T>
	{
		private readonly IEmitter<T> _emitter;
		private readonly Predicate<T> _filter;

		public WhereEmitter(IEmitter<T> emitter, Predicate<T> filter)
		{
			_emitter = emitter;
			_filter = filter;
		}
		
		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			_emitter.Connect(new WhereReceiver(receiver, _filter), cancel);
		}


		
		private sealed class WhereReceiver : IReceiver<T>
		{
			private readonly IReceiver<T> _receiver;
			private readonly Predicate<T> _filter;

			public WhereReceiver(IReceiver<T> receiver, Predicate<T> filter)
			{
				_receiver = receiver;
				_filter = filter;
			}

			public void OnCompleted()
			{
				_receiver.OnCompleted();
			}

			public void OnError(Exception error)
			{
				_receiver.OnError(error);
			}

			public void OnNext(T value)
			{
				if (_filter(value))
					_receiver.OnNext(value);
			}
		}
		
	}
}