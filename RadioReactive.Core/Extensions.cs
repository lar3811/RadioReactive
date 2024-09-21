using System;

namespace RadioReactive.Core
{
	public static class Extensions
	{
		public static void OnLast<T>(this IReceiver<T> receiver, T value)
		{
			receiver.OnNext(value);
			receiver.OnCompleted();
		}

		public static void Connect<T>(this IEmitter<T> emitter, ICancel cancel, Action<T> handler)
		{
			emitter.Connect(new DelegateReceiver<T>(handler), cancel);
		}

		public static void Connect<T>(this IEmitter<T> emitter, Action<T> handler)
		{
			Connect(emitter, NeverCancel.Instance, handler);
		}

		public static void Connect<T>(this IEmitter<T> emitter, IReceiver<T> receiver)
		{
			emitter.Connect(receiver, NeverCancel.Instance);
		}
	}
}