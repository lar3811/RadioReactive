using System;
using RadioReactive.Core;
using RadioReactive.Core.Disposables;

namespace RadioReactive.Extensions
{
	public sealed class SkipEmitter<T> : IEmitter<T>
	{
		private readonly IEmitter<T> _emitter;
		private readonly uint _count;

		public SkipEmitter(IEmitter<T> emitter, uint count)
		{
			_emitter = emitter;
			_count = count;
		}
		
		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			_emitter.Connect(new SkipReceiver(receiver, _count), cancel);
		}

		

		private sealed class SkipReceiver : IReceiver<T>
		{
			private readonly IReceiver<T> _receiver;
			private uint _count;

			public SkipReceiver(IReceiver<T> receiver, uint count)
			{
				_receiver = receiver;
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
				if (_count > 0)
				{
					_count--; 
					return;
				}
			
				_receiver.OnNext(value);
			}
		}
	}
}