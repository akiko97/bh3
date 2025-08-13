namespace UniRx
{
	public interface IProgress<T>
	{
		void Report(T value);
	}
}
