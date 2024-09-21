using System;
using RadioReactive.Core;

namespace RadioReactive.Emitters
{
	public sealed partial class StreamEmitter<T> : IEmitter<T>, IReceiver<T>
	{
#if DEBUG
		public IEmitterCore<T> Core => _core;
#endif

		private IEmitterCore<T> _core;

		public StreamEmitter(int listeners) : this(new EmitterCore<T>(listeners)) { }
		public StreamEmitter() : this(new EmitterCore<T>()) { }
		private StreamEmitter(EmitterCore<T> core)
		{
			_core = new StreamingCore(this, core);
		}
		
		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			_core.Connect(receiver, cancel);
		}

		public void OnCompleted()
		{
			_core.OnCompleted();
		}

		public void OnError(Exception error)
		{
			_core.OnError(error);
		}

		public void OnNext(T value)
		{
			_core.OnNext(value);
		}
	}
}