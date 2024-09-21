using System;
using RadioReactive.Core;
using RadioReactive.Core.Disposables;

namespace RadioReactive.Extensions
{
	public sealed class SkipUntilEmitter<TSource, TSignal> : IEmitter<TSource>
	{
		private readonly IEmitter<TSource> _source;
		private readonly IEmitter<TSignal> _activator;

		public SkipUntilEmitter(IEmitter<TSource> source, IEmitter<TSignal> activator)
		{
			_source = source;
			_activator = activator;
		}

		public void Connect(IReceiver<TSource> receiver, ICancel cancel)
		{
			new ActivatorReceiver(receiver, _source).ConnectSelf(_activator, cancel);
		}
		


		private sealed class ActivatorReceiver : IReceiver<TSignal>
		{
			private ICancel _sourceConnectionCancel;
			private Cancel _activatorConnectionCancel;
			
			private readonly IReceiver<TSource> _sourceReceiver;
			private readonly IEmitter<TSource> _sourceEmitter;

			public ActivatorReceiver(IReceiver<TSource> sourceReceiver, IEmitter<TSource> sourceEmitter)
			{
				_sourceReceiver = sourceReceiver;
				_sourceEmitter = sourceEmitter;
			}

			public void ConnectSelf(IEmitter<TSignal> activator, ICancel cancel)
			{
				_sourceConnectionCancel = cancel;
				_activatorConnectionCancel = cancel.Propagate();
				activator.Connect(this, _activatorConnectionCancel);
			}
			
			public void OnNext(TSignal value)
			{
				OnCompleted();
			}

			public void OnError(Exception error)
			{
				_sourceReceiver.OnError(error);
			}

			public void OnCompleted()
			{
				_activatorConnectionCancel.Request();
				_activatorConnectionCancel = null;
				_sourceEmitter.Connect(_sourceReceiver, _sourceConnectionCancel);
			}
		}
	}
}