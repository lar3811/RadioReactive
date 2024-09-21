namespace RadioReactive.Core
{
	public interface IEmitter<out T>
	{
		void Connect(IReceiver<T> receiver, ICancel cancel);
	}
}