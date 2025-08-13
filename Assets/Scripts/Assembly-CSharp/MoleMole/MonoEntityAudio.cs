using UnityEngine;

namespace MoleMole
{
	public class MonoEntityAudio : MonoBehaviour
	{
		public string witchTimeEvent;

		public string moveEvent;

		public string pickupHPHigh;

		public string pickupHPLow;

		public string pickupEquipItem;

		public string pickupCoin;

		public string monsterBorn;

		public string bossBorn;

		public string ultraReady;

		public void PostWitchTime()
		{
			SavePost(witchTimeEvent);
		}

		public void PostMove()
		{
			SavePost(moveEvent);
		}

		public void PostPickupHPHigh()
		{
			SavePost(pickupHPHigh);
		}

		public void PostPickupHPLow()
		{
			SavePost(pickupHPLow);
		}

		public void PostPickupEquipItem()
		{
			SavePost(pickupEquipItem);
		}

		public void PostPickupCoin()
		{
			SavePost(pickupCoin);
		}

		public void PostMonsterBorn()
		{
			SavePost(monsterBorn);
		}

		public void PostBossBorn()
		{
			SavePost(bossBorn);
		}

		public void PostUltraReady()
		{
			SavePost(ultraReady);
		}

		private void SavePost(string eventName)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				Singleton<WwiseAudioManager>.Instance.Post(eventName, base.gameObject);
			}
		}
	}
}
