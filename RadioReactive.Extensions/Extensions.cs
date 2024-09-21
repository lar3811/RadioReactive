using System;
using RadioReactive.Core;

namespace RadioReactive.Extensions
{
	public static class Extensions
	{
		public static IEmitter<T> Merge<T>(this IEmitter<T> emitter1, IEmitter<T> emitter2)
		{
			return new MergeEmitter<T>(emitter1, emitter2);
		}
		
		public static IEmitter<TOutput> Select<TInput, TOutput>(
			this IEmitter<TInput> emitter, Func<TInput, TOutput> select)
		{
			return new SelectEmitter<TInput, TOutput>(emitter, select);
		}

		public static IEmitter<T> Where<T>(this IEmitter<T> emitter, Predicate<T> filter)
		{
			return new WhereEmitter<T>(emitter, filter);
		}

		public static IEmitter<T> Skip<T>(this IEmitter<T> emitter, uint count)
		{
			return new SkipEmitter<T>(emitter, count);
		}

		public static IEmitter<TSource> SkipUntil<TSource, TSignal>(
			this IEmitter<TSource> source, IEmitter<TSignal> activator)
		{
			return new SkipUntilEmitter<TSource, TSignal>(source, activator);
		}

		public static IEmitter<T> Take<T>(this IEmitter<T> emitter, uint count)
		{
			return new TakeEmitter<T>(emitter, count);
		}

		public static IEmitter<TSource> TakeUntil<TSource, TSignal>(
			this IEmitter<TSource> source, IEmitter<TSignal> terminator)
		{
			return new TakeUntilEmitter<TSource, TSignal>(source, terminator);
		}
	}
}