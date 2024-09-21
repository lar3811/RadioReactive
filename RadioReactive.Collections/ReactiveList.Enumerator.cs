using System;
using System.Collections;
using System.Collections.Generic;

namespace RadioReactive.Collections
{
	public partial class ReactiveList<T>
	{
		private sealed class Enumerator : IEnumerator<T>
		{
			public T Current { get; private set; }
			
			private int _current;
			private int _version;
			
			private ReactiveList<T> _list;

			public Enumerator(ReactiveList<T> list)
			{
				_list = list;
				Reset();
			}

			public bool MoveNext()
			{
				if (_list == null)
				{
					RadioReactive.Core.Environment.Report(new InvalidOperationException("Enumerator is disposed"));
					return false;
				}
				
				if (_version != _list._version)
				{
					RadioReactive.Core.Environment.Report(new InvalidOperationException(
						$"Enumerator/collection versions mismatch: {_version}/{_list._version}"));
					return false;
				}
				
				_current++;
				if (_current < _list._count)
				{
					Current = _list[_current];
					return true;
				}
				return false;
			}

			public void Reset()
			{
				_version = _list._version;
				_current = -1;
				Current = default(T);
			}

			public void Dispose()
			{
				if (_list == null)
					return;
				
				Reset();
				_list = null;
			}

			object IEnumerator.Current => Current;
		}
	}
}