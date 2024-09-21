using System.Collections.Generic;

namespace RadioReactive.Collections
{
	partial class ReactiveList<T>
	{
		private List<IListCommand> _commands;
		private bool _locked;

		private void EnqueueItemCommand<TCommand>(TCommand command) where TCommand : IItemCommand
		{
			if (_locked && _commands != null)
			{
				var position = command.Position;
				for (int i = 0, count = _commands.Count; i < count; i++)
					position = _commands[i].Shift(position);
				command.Position = position;
			}
			EnqueueListCommand(command);
		}

		private void EnqueueListCommand<TCommand>(TCommand command) where TCommand : IListCommand 
		{
			if (_locked)
			{
				if (_commands == null)
					_commands = new List<IListCommand>(4);
				
				_commands.Add(command);
			}
			else
			{
				_locked = true;
				command.Execute(this);
				while (_commands != null && _commands.Count > 0)
				{
					var queuedCommand = _commands[0];
					_commands.RemoveAt(0);
					queuedCommand.Execute(this);
				}
				_locked = false;
			}
		}
		
		
		
		private interface IListCommand
		{
			void Execute(ReactiveList<T> target);
			int Shift(int position);
		}
		private interface IItemCommand : IListCommand
		{
			int Position { get; set; }
		}
			
		
		
		private struct SetCommand : IItemCommand
		{
			public int Position { get; set; }
			public T Item { get; set; }

			public void Execute(ReactiveList<T> target)
			{
				target.SetInternal(Position, Item);
			}

			public int Shift(int position)
			{
				return position;
			}
		}


		
		private struct InsertCommand : IItemCommand
		{
			public int Position { get; set; }
			public T Item { get; set; }

			public void Execute(ReactiveList<T> target)
			{
				target.InsertInternal(Position, Item);
			}

			public int Shift(int position)
			{
				return position >= Position ? position + 1 : position;
			}
		}

		
		
		private struct RemoveCommand : IItemCommand
		{
			public int Position { get; set; }

			public void Execute(ReactiveList<T> target)
			{
				target.RemoveInternal(Position);
			}

			public int Shift(int position)
			{
				return position >= Position ? position - 1 : position;
			}
		}
		
		

		private struct ClearCommand : IListCommand
		{
			public int Count { get; set; }
			
			public void Execute(ReactiveList<T> target)
			{
				target.ClearInternal();
			}

			public int Shift(int position)
			{
				return position - Count;
			}
		}
		
		

		private struct DisposeCommand : IListCommand
		{
			public void Execute(ReactiveList<T> target)
			{
				target.DisposeInternal();
			}

			public int Shift(int position)
			{
				return -1;
			}
		}
	}
}