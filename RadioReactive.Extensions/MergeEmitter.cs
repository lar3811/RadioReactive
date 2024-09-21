using System;
using RadioReactive.Core;

namespace RadioReactive.Extensions
{
	public sealed class MergeEmitter<T> : IEmitter<T>
	{
		private readonly IEmitter<T> _emitter1;
		private readonly IEmitter<T> _emitter2;

		public MergeEmitter(IEmitter<T> emitter1, IEmitter<T> emitter2)
		{
			_emitter1 = emitter1;
			_emitter2 = emitter2;
		}
		
		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			var subcancel = cancel.Propagate();
			receiver = new MergeReceiver(receiver, subcancel);
			_emitter1.Connect(receiver, subcancel);
			_emitter2.Connect(receiver, subcancel);
		}



		private sealed class MergeReceiver : IReceiver<T>
		{
			private int _emitters;
			
			private readonly IReceiver<T> _receiver;
			private readonly Cancel _cancel;

			public MergeReceiver(IReceiver<T> receiver, Cancel cancel)
			{
				_receiver = receiver;
				_cancel = cancel;
				_emitters = 2;
			}
			
			public void OnNext(T value)
			{
				_receiver.OnNext(value);
			}

			public void OnError(Exception error)
			{
				_emitters = 0;
				_cancel.Request();
				_receiver.OnError(error);
			}

			public void OnCompleted()
			{
				if (_emitters > 0 && --_emitters <= 0) 
					_receiver.OnCompleted();
			}
		}
	}
}