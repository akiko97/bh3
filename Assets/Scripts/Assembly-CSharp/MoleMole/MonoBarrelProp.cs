using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoBarrelProp : MonoHitableProp
	{
		private enum State
		{
			Idle = 0,
			WaitingForDestroy = 1
		}

		[Header("Will be destroyed after this seconds")]
		public float DestroyDelay = 0.5f;

		private EntityTimer _destroyTimer;

		private bool _isActive;

		public bool _toExplode = true;

		private State _state;

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			_isActive = true;
			_destroyTimer = new EntityTimer(DestroyDelay, this);
			_destroyTimer.Reset(false);
			_state = State.Idle;
		}

		protected override void Update()
		{
			UpdatePlugins();
			if (_state == State.Idle)
			{
				_destroyTimer.Core(1f);
				if (_destroyTimer.isTimeUp)
				{
					if (config.PropArguments.OnDestroyEffectPattern != null && base.gameObject.activeSelf)
					{
						FireEffect(config.PropArguments.OnDestroyEffectPattern);
					}
					if (!string.IsNullOrEmpty(config.PropArguments.AnimEventIDForHit) && _toExplode)
					{
						string animEventIDForHit = config.PropArguments.AnimEventIDForHit;
						ConfigPropAnimEvent configPropAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, animEventIDForHit);
						configPropAnimEvent.AttackPattern.patternMethod(animEventIDForHit, configPropAnimEvent.AttackPattern, this, (1 << InLevelData.MONSTER_HITBOX_LAYER) | (1 << InLevelData.AVATAR_HITBOX_LAYER));
					}
					_destroyTimer.Reset(true);
					_destroyTimer.timespan = 0.1f;
					_state = State.WaitingForDestroy;
				}
			}
			else if (_state == State.WaitingForDestroy)
			{
				_destroyTimer.Core(1f);
				if (_destroyTimer.isTimeUp)
				{
					_destroyTimer.Reset(false);
					_isToBeRemove = true;
				}
			}
		}

		public override bool IsActive()
		{
			return _isActive;
		}

		public override void SetDied(KillEffect killEffect)
		{
			hitbox.enabled = false;
			_isActive = false;
			_destroyTimer.Reset(true);
			if (config.PropArguments.OnKillEffectPattern != null && _toExplode && base.gameObject.activeSelf)
			{
				FireEffect(config.PropArguments.OnKillEffectPattern);
			}
		}
	}
}
