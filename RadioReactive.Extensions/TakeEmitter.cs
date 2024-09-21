using System;
using RadioReactive.Core;
using RadioReactive.Core.Disposables;

namespace RadioReactive.Extensions
{
	public sealed class TakeEmitter<T> : IEmitter<T>
	{
		private readonly IEmitter<T> _emitter;
		private readonly uint _count;

		public TakeEmitter(IEmitter<T> emitter, uint count)
		{
			if (count <= 0) 
				return;

			_emitter = emitter;
			_count = count;
		}
		
		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			var subcancel = cancel.Propagate();
			_emitter.Connect(new TakeReceiver(receiver, subcancel, _count), subcancel);
		}
		


		private sealed class TakeReceiver : IReceiver<T>
		{
			private uint _count;
			
			private readonly IReceiver<T> _receiver;
			private readonly Cancel _cancel;

			public TakeReceiver(IReceiver<T> receiver, Cancel cancel, uint count)
			{
				_receiver = receiver;
				_cancel = cancel;
				_count = count;
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
				if (_count == 0)
					return;
				
				if (_count-- > 1)
				{
					_receiver.OnNext(value);
				}
				else
				{
					_cancel.Request();
					_receiver.OnNext(value);
					_receiver.OnCompleted();
				}
			}
		}
	}
}