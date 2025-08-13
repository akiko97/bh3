using System;
using System.Threading;

namespace UniRx
{
	public static class Observer
	{
		private class AnonymousObserver<T> : IObserver<T>
		{
			private readonly Action<T> onNext;

			private readonly Action<Exception> onError;

			private readonly Action onCompleted;

			private int isStopped;

			public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
			{
				this.onNext = onNext;
				this.onError = onError;
				this.onCompleted = onCompleted;
			}

			public void OnNext(T value)
			{
				if (isStopped == 0)
				{
					onNext(value);
				}
			}

			public void OnError(Exception error)
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					onError(error);
				}
			}

			public void OnCompleted()
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					onCompleted();
				}
			}
		}

		private class EmptyOnNextAnonymousObserver<T> : IObserver<T>
		{
			private readonly Action<Exception> onError;

			private readonly Action onCompleted;

			private int isStopped;

			public EmptyOnNextAnonymousObserver(Action<Exception> onError, Action onCompleted)
			{
				this.onError = onError;
				this.onCompleted = onCompleted;
			}

			public void OnNext(T value)
			{
			}

			public void OnError(Exception error)
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					onError(error);
				}
			}

			public void OnCompleted()
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					onCompleted();
				}
			}
		}

		private class DelegatedOnNextObserver<T, TRoot> : IObserver<T>
		{
			private readonly Action<T> onNext;

			private readonly IObserver<TRoot> observer;

			private readonly IDisposable disposable;

			private int isStopped;

			public DelegatedOnNextObserver(Action<T> onNext, IObserver<TRoot> observer, IDisposable disposable)
			{
				this.onNext = onNext;
				this.observer = observer;
				this.disposable = disposable;
			}

			public void OnNext(T value)
			{
				if (isStopped == 0)
				{
					try
					{
						onNext(value);
					}
					catch
					{
						disposable.Dispose();
						throw;
					}
				}
			}

			public void OnError(Exception error)
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						disposable.Dispose();
					}
				}
			}

			public void OnCompleted()
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						disposable.Dispose();
					}
				}
			}
		}

		private class AutoDetachObserver<T> : IObserver<T>
		{
			private readonly IObserver<T> observer;

			private readonly IDisposable disposable;

			private int isStopped;

			public AutoDetachObserver(IObserver<T> observer, IDisposable disposable)
			{
				this.observer = observer;
				this.disposable = disposable;
			}

			public void OnNext(T value)
			{
				if (isStopped == 0)
				{
					try
					{
						observer.OnNext(value);
					}
					catch
					{
						disposable.Dispose();
						throw;
					}
				}
			}

			public void OnError(Exception error)
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						disposable.Dispose();
					}
				}
			}

			public void OnCompleted()
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						disposable.Dispose();
					}
				}
			}
		}

		public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
		{
			if (onNext == new Action<T>(Stubs.Ignore))
			{
				return new EmptyOnNextAnonymousObserver<T>(onError, onCompleted);
			}
			return new AnonymousObserver<T>(onNext, onError, onCompleted);
		}

		public static IObserver<T> Create<T, TRoot>(Action<T> onNext, IObserver<TRoot> rootObserver)
		{
			return new DelegatedOnNextObserver<T, TRoot>(onNext, rootObserver, Disposable.Empty);
		}

		public static IObserver<T> CreateAutoDetachObserver<T>(IObserver<T> observer, IDisposable disposable)
		{
			return new AutoDetachObserver<T>(observer, disposable);
		}
	}
}
