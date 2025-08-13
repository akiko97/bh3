using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class DevLevelActorPlugin : BaseActorPlugin
	{
		public MonoDevLevel _devLevel;

		public DevLevelActorPlugin(MonoDevLevel devLevel)
		{
			_devLevel = devLevel;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtStageReady)
			{
				OnStageReady((EvtStageReady)evt);
			}
			return false;
		}

		private void OnStageReady(EvtStageReady evt)
		{
		}

		private IEnumerator WaitAndDo()
		{
			yield return new WaitForSeconds(3f);
			HPProfile.Begin();
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "InvisibleProp", 0f, 0f, Vector3.zero, Vector3.forward);
			HPProfile.End("1st invincible prop");
			yield return null;
			HPProfile.Begin();
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "InvisibleProp", 0f, 0f, Vector3.zero, Vector3.forward);
			HPProfile.End("2nd invincible prop");
			HPProfile.Begin();
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_No_Break_Hit", Vector3.zero, Vector3.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
			HPProfile.End("1st effect");
			HPProfile.Begin();
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_No_Break_Hit", Vector3.zero, Vector3.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
			HPProfile.End("2st effect");
		}

		private IEnumerator WaitAndSpawnGoodsAndProps()
		{
			yield return new WaitForSeconds(2f);
			LDDropDataItem.GetLDDropDataItemByName("HPMedic").CreateDropGoods(new Vector3(-6f, 0f, 6f), Vector3.forward);
			LDDropDataItem.GetLDDropDataItemByName("SPMedic").CreateDropGoods(new Vector3(-5f, 0f, 6f), Vector3.forward);
			LDDropDataItem.GetLDDropDataItemByName("Coin").CreateDropGoods(new Vector3(-4f, 0f, 6f), Vector3.forward);
			LDDropDataItem.GetLDDropDataItemByName("Boost").CreateDropGoods(new Vector3(-3f, 0f, 6f), Vector3.forward);
			LDDropDataItem.GetLDDropDataItemByName("Crit").CreateDropGoods(new Vector3(-2f, 0f, 6f), Vector3.forward);
			LDDropDataItem.GetLDDropDataItemByName("Shielded").CreateDropGoods(new Vector3(-1f, 0f, 6f), Vector3.forward);
			Singleton<DynamicObjectManager>.Instance.CreateStageExitField(562036737u, new Vector3(1f, 0f, 6f), Vector3.forward);
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "GeneralBox", 100000f, 0f, new Vector3(3f, 0f, 6f), Vector3.forward);
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "AdvancedBox", 10f, 0f, new Vector3(5f, 0f, 6f), Vector3.forward);
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "JokeBox", 10f, 0f, new Vector3(-6f, 0f, 4f), Vector3.forward);
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "Barrel", 10f, 100f, new Vector3(-4f, 0f, 4f), Vector3.forward);
			Singleton<DynamicObjectManager>.Instance.CreateBarrierField(562036737u, "Barrier_01", new Vector3(0f, 0f, -6f), Vector3.right, 3f);
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "Switch", 10f, 0f, new Vector3(-2f, 0f, 4f), Vector3.forward);
			uint id = Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "Trap_Fire", 10f, 0f, new Vector3(4f, 0f, 4f), Vector3.forward);
			Singleton<LevelDesignManager>.Instance.EnableFireProp(id, 5f, 3f);
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "Trap_Palsy_Bomb", 10f, 100f, new Vector3(7f, 0f, 4f), Vector3.forward);
			Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, "Trap_Palsy", 10f, 100f, new Vector3(0f, 0f, 2f), Vector3.forward);
			Singleton<DynamicObjectManager>.Instance.CreateEquipItem(562036737u, 20001, new Vector3(5f, 0f, -7f), Vector3.forward, true);
			Singleton<DynamicObjectManager>.Instance.CreateStigmataItem(562036737u, 30001, new Vector3(6f, 0f, -7f), Vector3.forward, true);
			Singleton<DynamicObjectManager>.Instance.CreateMaterialItem(562036737u, 1001, new Vector3(7f, 0f, -7f), Vector3.forward, true);
			Singleton<DynamicObjectManager>.Instance.CreateAvatarFragmentItem(562036737u, 10101, new Vector3(8f, 0f, -7f), Vector3.forward, true);
		}
	}
}
