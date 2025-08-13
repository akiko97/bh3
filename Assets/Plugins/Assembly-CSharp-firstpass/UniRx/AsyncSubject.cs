using System;
using UniRx.InternalUtil;

namespace UniRx
{
	public sealed class AsyncSubject<T> : IObserver<T>, ISubject<T>, ISubject<T, T>, IObservable<T>, IOptimizedObservable<T>
	{
		private class Subscription : IDisposable
		{
			private readonly object gate = new object();

			private AsyncSubject<T> parent;

			private IObserver<T> unsubscribeTarget;

			public Subscription(AsyncSubject<T> parent, IObserver<T> unsubscribeTarget)
			{
				this.parent = parent;
				this.unsubscribeTarget = unsubscribeTarget;
			}

			public void Dispose()
			{
				lock (gate)
				{
					if (parent == null)
					{
						return;
					}
					lock (parent.observerLock)
					{
						ListObserver<T> listObserver = parent.outObserver as ListObserver<T>;
						if (listObserver != null)
						{
							parent.outObserver = listObserver.Remove(unsubscribeTarget);
						}
						else
						{
							parent.outObserver = new EmptyObserver<T>();
						}
						unsubscribeTarget = null;
						parent = null;
					}
				}
			}
		}

		private object observerLock = new object();

		private T lastValue;

		private bool hasValue;

		private bool isStopped;

		private bool isDisposed;

		private Exception lastError;

		private IObserver<T> outObserver = new EmptyObserver<T>();

		public T Value
		{
			get
			{
				ThrowIfDisposed();
				if (!isStopped)
				{
					throw new InvalidOperationException("AsyncSubject is not completed yet");
				}
				if (lastError != null)
				{
					throw lastError;
				}
				return lastValue;
			}
		}

		public bool HasObservers
		{
			get
			{
				return !(outObserver is EmptyObserver<T>) && !isStopped && !isDisposed;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return isStopped;
			}
		}

		public void OnCompleted()
		{
			IObserver<T> observer;
			T value;
			bool flag;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (isStopped)
				{
					return;
				}
				observer = outObserver;
				outObserver = new EmptyObserver<T>();
				isStopped = true;
				value = lastValue;
				flag = hasValue;
			}
			if (flag)
			{
				observer.OnNext(value);
				observer.OnCompleted();
			}
			else
			{
				observer.OnCompleted();
			}
		}

		public void OnError(Exception error)
		{
			if (error == null)
			{
				throw new ArgumentNullException("error");
			}
			IObserver<T> observer;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (isStopped)
				{
					return;
				}
				observer = outObserver;
				outObserver = new EmptyObserver<T>();
				isStopped = true;
				lastError = error;
			}
			observer.OnError(error);
		}

		public void OnNext(T value)
		{
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (!isStopped)
				{
					hasValue = true;
					lastValue = value;
				}
			}
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException("observer");
			}
			Exception ex = null;
			T value = default(T);
			bool flag = false;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (!isStopped)
				{
					ListObserver<T> listObserver = outObserver as ListObserver<T>;
					if (listObserver != null)
					{
						outObserver = listObserver.Add(observer);
					}
					else
					{
						IObserver<T> observer2 = outObserver;
						if (observer2 is EmptyObserver<T>)
						{
							outObserver = observer;
						}
						else
						{
							outObserver = new ListObserver<T>(new ImmutableList<IObserver<T>>(new IObserver<T>[2] { observer2, observer }));
						}
					}
					return new Subscription(this, observer);
				}
				ex = lastError;
				value = lastValue;
				flag = hasValue;
			}
			if (ex != null)
			{
				observer.OnError(ex);
			}
			else if (flag)
			{
				observer.OnNext(value);
				observer.OnCompleted();
			}
			else
			{
				observer.OnCompleted();
			}
			return Disposable.Empty;
		}

		public void Dispose()
		{
			lock (observerLock)
			{
				isDisposed = true;
				outObserver = new DisposedObserver<T>();
				lastError = null;
				lastValue = default(T);
			}
		}

		private void ThrowIfDisposed()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException(string.Empty);
			}
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}
	}
}
