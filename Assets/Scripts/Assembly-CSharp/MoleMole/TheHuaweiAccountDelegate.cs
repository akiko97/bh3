using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class TheHuaweiAccountDelegate : TheBaseAccountDelegate
	{
		public TheHuaweiAccountDelegate()
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				if (_delegate == null)
				{
					_delegate = new AndroidJavaObject("com.miHoYo.bh3.huawei.HuaweiSdk");
				}
			});
		}

		public override void init(bool debugMode, string callbackClass, string callbackMethod, Function callback)
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("init", debugMode, _activity, callbackClass, callbackMethod);
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
			return null;
		}

		public override string getUid()
		{
			return _delegate.Call<string>("getUid", new object[0]);
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
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("showToolbar");
			});
		}

		public override void hideToolBar()
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("hideToolbar");
			});
		}

		public override void exit()
		{
			_activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				_delegate.Call("exit");
			});
		}
	}
}
