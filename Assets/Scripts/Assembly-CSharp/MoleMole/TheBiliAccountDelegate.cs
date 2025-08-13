using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class TheBiliAccountDelegate : TheBaseAccountDelegate
	{
		private static string MERCHANT_ID = "18";

		private static string APP_ID = "180";

		private static string SERVER_ID = "378";

		private static string APP_KEY = "dbf8f1b4496f430b8a3c0f436a35b931";

		public TheBiliAccountDelegate()
		{
			if (_delegate == null)
			{
				_delegate = new AndroidJavaObject("com.miHoYo.bh3.bilibili.BiliAgent");
			}
		}

		public override void init(bool debugMode, string callbackClass, string callbackMethod, Function callback)
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("init", _activity, debugMode, MERCHANT_ID, APP_ID, SERVER_ID, APP_KEY, callbackClass, callbackMethod);
			});
		}

		public override IEnumerator login(string callbackClass, string callbackMethod, string arg1, string arg2, Function callback)
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("login", callbackClass, callbackMethod);
			});
			return null;
		}

		public override IEnumerator register(string callbackClass, string callbackMethod, string arg1, string arg2, string arg3, string arg4, Function callback)
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("register", callbackClass, callbackMethod);
			});
			return null;
		}

		public override string getUid()
		{
			return _delegate.Call<string>("getUid", new object[0]);
		}

		public override string getUsername()
		{
			return _delegate.Call<string>("getUsername", new object[0]);
		}

		public override void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, Function callback)
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("pay", productID, productName, productPrice, tradeNo, userID, notifyUrl, callbackClass, callbackMethod);
			});
		}

		public override void showToolBar()
		{
		}

		public override void hideToolBar()
		{
		}

		public override void showPausePage()
		{
		}

		public override void showUserCenter()
		{
		}

		public void createRole(string uid)
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("createRole", uid);
			});
		}

		public override void exit()
		{
		}
	}
}
