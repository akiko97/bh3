using System;
using System.Collections.Generic;
using System.Threading;

namespace MoleMole
{
	public class BackGroundWorker
	{
		private List<Action> _list = new List<Action>();

		private Thread _workThread;

		private volatile bool _stillWorking;

		public int RemainCount
		{
			get
			{
				return _list.Count;
			}
		}

		public void StartBackGroundWork(string threadName = "")
		{
			_stillWorking = true;
			_list.Clear();
			_workThread = new Thread(WorkingThread);
			_workThread.Name = threadName;
			_workThread.Priority = ThreadPriority.Highest;
			_workThread.IsBackground = true;
			_workThread.Start();
		}

		public void AddBackGroundWork(Action work)
		{
			lock (_list)
			{
				_list.Add(work);
			}
		}

		public void StopBackGroundWork(bool clearList = true)
		{
			if (clearList)
			{
				lock (_list)
				{
					_list.Clear();
				}
			}
			_stillWorking = false;
			_workThread = null;
		}

		private void WorkingThread()
		{
			while (_stillWorking)
			{
				if (_list.Count == 0)
				{
					Thread.Sleep(1);
					continue;
				}
				Action action;
				lock (_list)
				{
					action = _list[0];
				}
				try
				{
					action();
				}
				catch (Exception)
				{
				}
				lock (_list)
				{
					_list.RemoveAt(0);
				}
				Thread.Sleep(1);
			}
		}
	}
}
