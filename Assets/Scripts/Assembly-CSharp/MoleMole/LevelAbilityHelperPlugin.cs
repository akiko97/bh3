using System.Collections.Generic;

namespace MoleMole
{
	public class LevelAbilityHelperPlugin : BaseActorPlugin
	{
		private List<ActorModifier>[] _levelBuffAttachedModifiers;

		private LevelActor _levelActor;

		public string[] _attachedLevelBuffEffect;

		public LevelAbilityHelperPlugin(LevelActor levelActor)
		{
			_levelActor = levelActor;
			_levelBuffAttachedModifiers = new List<ActorModifier>[2];
			for (int i = 0; i < _levelBuffAttachedModifiers.Length; i++)
			{
				_levelBuffAttachedModifiers[i] = new List<ActorModifier>();
			}
			_attachedLevelBuffEffect = new string[2];
		}

		public void AddLevelBuffModifier(LevelBuffType levelBuffType, ActorModifier modifier)
		{
			_levelBuffAttachedModifiers[(int)levelBuffType].Add(modifier);
		}

		public void AttachLevelBuffEffect(LevelBuffType levelBuffType, string effectPattern)
		{
			RainController rainController = Singleton<StageManager>.Instance.GetPerpStage().rainController;
			if (rainController == null)
			{
				_attachedLevelBuffEffect[(int)levelBuffType] = effectPattern;
				Singleton<EffectManager>.Instance.CreateUniqueIndexedEffectPattern(effectPattern, levelBuffType.ToString(), Singleton<LevelManager>.Instance.levelEntity);
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtLevelBuffState)
			{
				return OnLevelBuffState((EvtLevelBuffState)evt);
			}
			return false;
		}

		private bool OnLevelBuffState(EvtLevelBuffState evt)
		{
			if (evt.state == LevelBuffState.Stop)
			{
				List<ActorModifier> list = _levelBuffAttachedModifiers[(int)evt.levelBuff];
				for (int i = 0; i < list.Count; i++)
				{
					ActorModifier actorModifier = list[i];
					if (actorModifier.owner != null)
					{
						actorModifier.owner.abilityPlugin.TryRemoveModifier(actorModifier);
					}
				}
				list.Clear();
				if (_attachedLevelBuffEffect[(int)evt.levelBuff] != null)
				{
					Singleton<EffectManager>.Instance.SetDestroyUniqueIndexedEffectPattern(evt.levelBuff.ToString());
					_attachedLevelBuffEffect[(int)evt.levelBuff] = null;
				}
			}
			return true;
		}
	}
}
