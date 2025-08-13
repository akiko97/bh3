using System;

namespace UniRx
{
	public static class Observable
	{
		private class AnonymousObservable<T> : IObservable<T>, IOptimizedObservable<T>
		{
			private readonly bool isRequiredSubscribeOnCurrentThread;

			private readonly Func<IObserver<T>, IDisposable> subscribe;

			public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribe)
				: this(subscribe, false)
			{
			}

			public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribe, bool isSchedulerlessObservable)
			{
				this.subscribe = subscribe;
				isRequiredSubscribeOnCurrentThread = isSchedulerlessObservable;
			}

			public bool IsRequiredSubscribeOnCurrentThread()
			{
				return isRequiredSubscribeOnCurrentThread;
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				IObserver<T> arg = Observer.CreateAutoDetachObserver(observer, singleAssignmentDisposable);
				singleAssignmentDisposable.Disposable = subscribe(arg);
				return singleAssignmentDisposable;
			}
		}

		private class ConnectableObservable<T> : IObservable<T>, IConnectableObservable<T>
		{
			private readonly IObservable<T> source;

			private readonly ISubject<T> subject;

			public ConnectableObservable(IObservable<T> source, ISubject<T> subject)
			{
				this.source = source;
				this.subject = subject;
			}

			public IDisposable Connect()
			{
				return source.Subscribe(subject);
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				return subject.Subscribe(observer);
			}
		}

		public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new AnonymousObservable<T>(subscribe);
		}

		public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new AnonymousObservable<T>(subscribe, isRequiredSubscribeOnCurrentThread);
		}
	}
}
