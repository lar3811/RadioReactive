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
		sealed class StreamingCore : IEmitterCore<T>
		{
#if DEBUG
			public EmitterCore<T> Core => _core;
#endif
				
			private readonly StreamEmitter<T> _shell;
			private readonly EmitterCore<T> _core;

			public StreamingCore(StreamEmitter<T> shell, EmitterCore<T> core)
			{
				_shell = shell;
				_core = core;
			}
			
			public void Connect(IReceiver<T> receiver, ICancel cancel)
			{
				_core.Connect(receiver, cancel);
			}
			
			public void OnNext(T value)
			{
				_core.OnNext(value);
			}

			public void OnError(Exception error)
			{
				_shell._core = new BrokenCore(error);
				_core.OnError(error);
			}

			public void OnCompleted()
			{
				_shell._core = DepletedCore.Instance;
				_core.OnCompleted();
			}
		}
	}
}