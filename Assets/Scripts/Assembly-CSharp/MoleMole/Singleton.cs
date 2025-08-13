using System;

namespace MoleMole
{
	public static class Singleton<T> where T : class
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				return _instance;
			}
		}

		static Singleton()
		{
		}

		public static void CreateByInstance(T instance)
		{
			_instance = instance;
		}

		public static void Create()
		{
			_instance = (T)Activator.CreateInstance(typeof(T), true);
		}

		public static T GetInstance()
		{
			return _instance;
		}

		public static void Destroy()
		{
			_instance = (T)null;
		}
	}
}
