using System;
using RadioReactive.Core;

namespace RadioReactive.Extensions
{
	public sealed class SelectEmitter<TInput, TOutput> : IEmitter<TOutput>
	{
		private readonly IEmitter<TInput> _emitter;
		private readonly Func<TInput, TOutput> _select;

		public SelectEmitter(IEmitter<TInput> emitter, Func<TInput, TOutput> select)
		{
			_emitter = emitter;
			_select = @select;
		}
		
		public void Connect(IReceiver<TOutput> receiver, ICancel cancel)
		{
			_emitter.Connect(new SelectReceiver(receiver, _select), cancel);
		}
		


		private sealed class SelectReceiver : IReceiver<TInput>
		{
			private readonly IReceiver<TOutput> _receiver;
			private readonly Func<TInput, TOutput> _select;

			public SelectReceiver(IReceiver<TOutput> receiver, Func<TInput, TOutput> @select)
			{
				_receiver = receiver;
				_select = @select;
			}
			
			public void OnCompleted()
			{
				_receiver.OnCompleted();
			}

			public void OnError(Exception error)
			{
				_receiver.OnError(error);
			}

			public void OnNext(TInput value)
			{
				_receiver.OnNext(_select(value));
			}
		}
	}
}