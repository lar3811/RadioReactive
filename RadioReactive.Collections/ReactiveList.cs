using System;
using System.Collections;
using System.Collections.Generic;
using RadioReactive.Core;
using RadioReactive.Emitters;

namespace RadioReactive.Collections
{
	public sealed partial class ReactiveList<T> : IList<T>, IList, IDisposable
	{
		private const int InitialCapacity = 12;
		private const int GrowthFactor = 2;

		public bool IsReadOnly => false;
		public bool IsFixedSize => false;

		public object SyncRoot => this;
		public bool IsSynchronized => false;
		
		public int Count => _count;

		public IEmitter<T> ItemAdded => _itemAdded;
		public IEmitter<T> ItemRemoved => _itemRemoved;

		object IList.this[int index]
		{
			get => this[index];
			set
			{
				if (ReportTypeCheck(value))
					this[index] = (T) value;
			}
		}

		public T this[int index]
		{
			get => ReportIndexCheck(index) ? _items[index] : default(T);
			set => EnqueueItemCommand(new SetCommand { Position = index, Item = value });
		}

		private T[] _items;
		private int _count;
		private int _version;

		private StreamEmitter<T> _itemAdded;
		private StreamEmitter<T> _itemRemoved;

		public ReactiveList(int capacity)
		{
			if (capacity < 0)
			{
				RadioReactive.Core.Environment.Report(new ArgumentOutOfRangeException(
					$"capacity ({capacity})"));
				capacity = 0;
			}
			
			_items = new T[capacity];
			
			Construct();
		}
		public ReactiveList(T[] items)
		{
			if (items == null)
			{
				RadioReactive.Core.Environment.Report(new ArgumentNullException(nameof(items)));
				items = Array.Empty<T>();
			}
			
			if (items.Length > 0)
			{
				_count = items.Length;
				_items = new T[items.Length];
				Array.Copy(items, _items, items.Length);
			}
			else
			{
				_items = items;
			}
			
			Construct();
		}
		public ReactiveList() : this(InitialCapacity) { }

		private void Construct()
		{
			_version = int.MinValue;
			_itemAdded = new StreamEmitter<T>();
			_itemRemoved = new StreamEmitter<T>();
		}

		private static bool TypeCheck(object value)
		{
			return value == null || value.GetType() == typeof(T);
		}

		private static bool ReportTypeCheck(object value)
		{
			var isValid = TypeCheck(value);
			if (!isValid)
			{
				RadioReactive.Core.Environment.Report(new ArrayTypeMismatchException(
					$"Argument type is invalid: {value.GetType().FullName}"));
			}
			return isValid;
		}

		private bool ReportIndexCheck(int index)
		{
			var isValid = index >= 0 && index < _count;
			if (!isValid)
			{
				RadioReactive.Core.Environment.Report(new IndexOutOfRangeException(
					$"Index out of range: {index}/{_count}"));
			}
			return isValid;
		}

		private void EnsureCapacity(int capacity)
		{
			var finalCapacity = _items.Length;
			while (finalCapacity < capacity)
				finalCapacity *= GrowthFactor;
			if (finalCapacity <= _items.Length)
				return;

			var newArray = new T[finalCapacity];
			Array.Copy(_items, newArray, _count);
			_items = newArray;
		}

		private void SetInternal(int index, T item)
		{
			if (!ReportIndexCheck(index))
				return;

			var replaced = _items[index];
			if (EqualityComparer<T>.Default.Equals(replaced, item))
				return;

			_version++;
			_items[index] = item;
			_itemRemoved.OnNext(replaced);
			_itemAdded.OnNext(item);
		}

		public void AddRange(T[] items)
		{
			
		}

		private void AddRangeInternal(T[] items)
		{
			
		}

		int IList.Add(object value)
		{
			if (!ReportTypeCheck(value)) 
				return -1;

			Add((T) value);
			return _count; // TODO: return proper index
		}

		public void Add(T item)
		{
			Insert(_count, item);
		}

		void IList.Insert(int index, object value)
		{
			if (ReportTypeCheck(value))
				Insert(index, (T)value);
		}

		public void Insert(int index, T item)
		{
			EnqueueItemCommand(new InsertCommand { Position = index, Item = item });
		}

		private void InsertInternal(int index, T item)
		{
			if (_items == null)
			{
				RadioReactive.Core.Environment.Report(new ObjectDisposedException(nameof(ReactiveList<T>)));
				return;
			}
			
			if (index < 0 || index > _count)
			{
				RadioReactive.Core.Environment.Report(new IndexOutOfRangeException(
					$"Index out of range: {index}/{_count}"));
				return;
			}
			
			EnsureCapacity(_count + 1);
			Array.Copy(_items, index, _items, index + 1, _count - index);
			_items[index] = item;
			_count++;
			_version++;
			_itemAdded.OnNext(item);
		}

		void IList.Remove(object value)
		{
			if (TypeCheck(value))
				Remove((T) value);
		}

		public bool Remove(T item)
		{
			var index = IndexOf(item);
			if (index < 0)
				return false;

			RemoveAt(index);
			return true;
		}

		public void RemoveAt(int index)
		{
			EnqueueItemCommand(new RemoveCommand { Position = index });
		}

		private void RemoveInternal(int index)
		{
			if (!ReportIndexCheck(index))
				return;
			
			_version++;
			_count--;
			var item = _items[index];
			Array.Copy(_items, index + 1, _items, index, _count - index);
			_items[_count] = default(T);
			_itemRemoved.OnNext(item);
		}

		public void Clear()
		{
			EnqueueListCommand(new ClearCommand { Count = _count });
		}

		private void ClearInternal()
		{
			if (_items == null)
				return;
			
			var count = _count;
			_count = 0;
			_version++;
			
			for (int i = 0; i < count; i++)
			{
				var item = _items[i];
				_items[i] = default(T);
				_itemRemoved.OnNext(item);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array != null && array.GetType() != typeof(T[]))
			{
				RadioReactive.Core.Environment.Report(new ArrayTypeMismatchException());
				return;
			}

			CopyTo((T[]) array, index);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				RadioReactive.Core.Environment.Report(new ArgumentNullException(nameof(array)));
				return;
			}

			if (arrayIndex < 0 || array.Length - arrayIndex < _count)
			{
				RadioReactive.Core.Environment.Report(new ArgumentException(nameof(arrayIndex)));
				return;
			}
			
			if (_items != null && _count > 0)
				Array.Copy(_items, 0, array, arrayIndex, _count);
		}

		bool IList.Contains(object value)
		{
			return TypeCheck(value) && Contains((T) value);
		}

		public bool Contains(T item)
		{
			return IndexOf(item) != -1;
		}

		int IList.IndexOf(object value)
		{
			return TypeCheck(value) ? IndexOf((T) value) : -1;
		}

		public int IndexOf(T item)
		{
			if (_items == null)
				return -1;
			
			for (int i = 0; i < _count; i++)
				if (EqualityComparer<T>.Default.Equals(_items[i], item))
					return i;

			return -1;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(this);
		}

		public void Dispose()
		{
			EnqueueListCommand(new DisposeCommand());
		}

		private void DisposeInternal()
		{
			if (_items == null)
				return;
			
			ClearInternal();
			_commands = null;
			_items = null;
			_itemAdded.OnCompleted();
			_itemRemoved.OnCompleted();
		}
	}
}