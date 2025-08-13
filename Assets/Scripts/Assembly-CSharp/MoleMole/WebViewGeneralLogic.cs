using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public static class WebViewGeneralLogic
	{
		public static void LoadUrl(string url, bool transparent = false)
		{
			UniWebView uniWebView = CreateWebview();
			uniWebView.url = url;
			uniWebView.OnLoadComplete += OnLoadComplete;
			uniWebView.OnReceivedMessage += OnReceivedMessage;
			UniWebViewPlugin.TransparentBackground(uniWebView.gameObject.name, transparent);
			uniWebView.Load();
			uniWebView.Show();
		}

		private static UniWebView CreateWebview()
		{
			GameObject gameObject = GameObject.Find("WebView");
			if (gameObject == null)
			{
				gameObject = new GameObject("WebView");
			}
			UniWebView uniWebView = gameObject.AddComponent<UniWebView>();
			uniWebView.toolBarShow = false;
			gameObject.AddComponent<UniWebviewAndroidReloadHelper>();
			return uniWebView;
		}

		private static UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
		{
			if (orientation == UniWebViewOrientation.Portrait)
			{
				return new UniWebViewEdgeInsets(5, 5, 5, 5);
			}
			return new UniWebViewEdgeInsets(5, 5, 5, 5);
		}

		private static void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
		{
			if (!success)
			{
			}
		}

		private static void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
		{
			if (message.path == "close")
			{
				Object.Destroy(webView.gameObject);
			}
			else if (message.path == "register")
			{
				Object.Destroy(webView.gameObject);
				if (message.rawMessage != null)
				{
					Regex regex = new Regex("username=(.*)&password=(.*)");
					string value = regex.Match(message.rawMessage).Groups[1].Value;
					string value2 = regex.Match(message.rawMessage).Groups[2].Value;
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountRegisterSuccess, new Tuple<string, string>(value, value2)));
				}
			}
			else if (message.path == "login")
			{
				Object.Destroy(webView.gameObject);
				if (message.rawMessage != null)
				{
					Regex regex2 = new Regex("username=(.*)&password=(.*)");
					string value3 = regex2.Match(message.rawMessage).Groups[1].Value;
					string value4 = regex2.Match(message.rawMessage).Groups[2].Value;
					Singleton<AccountManager>.Instance.manager.LoginUIFinishedCallBack(value3, value4);
				}
			}
			else if (message.path == "bind_email")
			{
				Object.Destroy(webView.gameObject);
				if (message.rawMessage != null)
				{
					Regex regex3 = new Regex("email=(.*)");
					string value5 = regex3.Match(message.rawMessage).Groups[1].Value;
					Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.email = value5;
					Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged));
				}
			}
			else if (message.path == "bind_mobile")
			{
				Object.Destroy(webView.gameObject);
				if (message.rawMessage != null)
				{
					Regex regex4 = new Regex("mobile=(.*)");
					string value6 = regex4.Match(message.rawMessage).Groups[1].Value;
					Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.mobile = value6;
					Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged));
				}
			}
			else if (message.path == "bind_identity")
			{
				Object.Destroy(webView.gameObject);
				if (message.rawMessage != null)
				{
					Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.isRealNameVerify = true;
					Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged));
				}
			}
		}
	}
}
