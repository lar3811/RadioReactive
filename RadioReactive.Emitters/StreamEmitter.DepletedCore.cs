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
		sealed class DepletedCore : IEmitterCore<T>
		{
			public static readonly DepletedCore Instance = new DepletedCore();
			
			private DepletedCore() { }
			
			public void Connect(IReceiver<T> receiver, ICancel cancel)
			{
				if (!cancel.IsRequested)
					receiver.OnCompleted();
			}

			public void OnNext(T value) { }

			public void OnError(Exception error) { }

			public void OnCompleted() { }
		}
	}
}