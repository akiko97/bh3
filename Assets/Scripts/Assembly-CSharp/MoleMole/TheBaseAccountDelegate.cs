using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class TheBaseAccountDelegate
	{
		public delegate void Function(string param);

		protected AndroidJavaObject _activity;

		protected AndroidJavaObject _handler;

		protected AndroidJavaObject _delegate;

		public TheBaseAccountDelegate()
		{
			/*if (_activity == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				_activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}*/
		}

		public virtual void init(bool debugMode, string callbackClass, string callbackMethod, Function callback)
		{
		}

		public virtual IEnumerator login(string callbackClass, string callbackMethod, string arg1, string arg2, Function callback)
		{
			return null;
		}

		public virtual IEnumerator register(string callbackClass, string callbackMethod, string arg1, string arg2, string arg3, string arg4, Function callback)
		{
			return null;
		}

		public virtual string getUid()
		{
			return string.Empty;
		}

		public virtual string getUsername()
		{
			return string.Empty;
		}

		public virtual void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, Function callback)
		{
		}

		public virtual void showToolBar()
		{
		}

		public virtual void hideToolBar()
		{
		}

		public virtual void showPausePage()
		{
		}

		public virtual void showUserCenter()
		{
		}

		public virtual void exit()
		{
		}
	}
}
