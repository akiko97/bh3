using System.Collections;
using MoleMole.MPProtocol;
using UnityEngine;

namespace MoleMole
{
	public class MPDevLevelActorPlugin : BaseActorPlugin
	{
		public MonoMPDevLevel _mpDevLevel;

		public MPDevLevelActorPlugin(MonoMPDevLevel devLevel)
		{
			_mpDevLevel = devLevel;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtStageReady)
			{
				return OnStageReady((EvtStageReady)evt);
			}
			return false;
		}

		private bool OnStageReady(EvtStageReady evt)
		{
			if (Singleton<MPManager>.Instance.isMaster && Singleton<MPLevelManager>.Instance.mpMode == MPMode.Normal)
			{
				_mpDevLevel.StartCoroutine(MasterWaitAndDo());
			}
			return true;
		}

		private IEnumerator MasterWaitAndDo()
		{
			yield return new WaitForSeconds(5f);
			while (true)
			{
				Singleton<MonsterManager>.Instance.CreateMonster("DeadWalker", "Default", 10, true, Vector3.zero, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), false, 0u);
				yield return new WaitForSeconds(2f);
				Singleton<MonsterManager>.Instance.CreateMonster("DeadArcher", "Default", 10, true, Vector3.zero, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), false, 0u);
				while (Singleton<MonsterManager>.Instance.LivingMonsterCount() > 0)
				{
					yield return new WaitForSeconds(2f);
				}
			}
		}
	}
}
