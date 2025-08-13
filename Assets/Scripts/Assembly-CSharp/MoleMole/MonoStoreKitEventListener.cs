using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoStoreKitEventListener : MonoBehaviour
	{
		public static Action<PayResult> IAP_PURCHASE_CALLBACK;
	}
}
