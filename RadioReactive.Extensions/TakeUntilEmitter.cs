using System;
using RadioReactive.Core;
using RadioReactive.Core.Disposables;

namespace RadioReactive.Extensions
{
	public sealed class TakeUntilEmitter<TSource, TSignal> : IEmitter<TSource>
	{
		private readonly IEmitter<TSource> _source;
		private readonly IEmitter<TSignal> _terminator;

		public TakeUntilEmitter(IEmitter<TSource> source, IEmitter<TSignal> terminator)
		{
			_source = source;
			_terminator = terminator;
		}

		public void Connect(IReceiver<TSource> receiver, ICancel cancel)
		{
			var subcancel = cancel.Propagate();
			_terminator.Connect(new TerminatorReceiver(receiver, subcancel), subcancel);
			_source.Connect(new SourceReceiver(receiver, subcancel), subcancel);
		}
		


		private sealed class TerminatorReceiver : IReceiver<TSignal>
		{
			private readonly IReceiver<TSource> _receiver;
			private readonly Cancel _cancel;

			public TerminatorReceiver(IReceiver<TSource> receiver, Cancel cancel)
			{
				_receiver = receiver;
				_cancel = cancel;
			}

			public void OnNext(TSignal value)
			{
				_receiver.OnCompleted();
				_cancel.Request();
			}

			public void OnError(Exception error)
			{
				_receiver.OnError(error);
				_cancel.Request();
			}

			public void OnCompleted()
			{
				_receiver.OnCompleted();
				_cancel.Request();
			}
		}

		private sealed class SourceReceiver : IReceiver<TSource>
		{
			private readonly IReceiver<TSource> _sourceReceiver;
			private readonly Cancel _cancel;

			public SourceReceiver(IReceiver<TSource> sourceReceiver, Cancel cancel)
			{
				_sourceReceiver = sourceReceiver;
				_cancel = cancel;
			}
			
			public void OnNext(TSource value)
			{
				_sourceReceiver.OnNext(value);
			}

			public void OnError(Exception error)
			{
				_sourceReceiver.OnError(error);
				_cancel.Request();
			}

			public void OnCompleted()
			{
				_sourceReceiver.OnCompleted();
				_cancel.Request();
			}
		}
	}
}