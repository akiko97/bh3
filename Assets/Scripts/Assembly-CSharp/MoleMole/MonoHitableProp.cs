using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoHitableProp : BaseMonoPropObject, IFrameHaltable
	{
		public Collider hitbox;

		protected FrameHaltPlugin _frameHaltPlugin;

		public FixedStack<float> timeScaleStack
		{
			get
			{
				return _timeScaleStack;
			}
		}

		protected override void Awake()
		{
			base.Awake();
		}

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			onIsGhostChanged = (Action<bool>)Delegate.Combine(onIsGhostChanged, new Action<bool>(OnIsGhostChanged));
			InitPlugins();
		}

		private void OnIsGhostChanged(bool isGhost)
		{
			hitbox.enabled = !isGhost;
		}

		public override void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity, uint sourceID)
		{
			ResetTrigger("LightHitTrigger");
			ResetTrigger("HitTrigger");
			if (hitEffect > AttackResult.AnimatorHitEffect.Normal)
			{
				SetTrigger("HitTrigger");
			}
			else
			{
				SetTrigger("LightHitTrigger");
			}
			FrameHalt(frameHalt);
		}

		public override void SetDied(KillEffect killEffect)
		{
			if (config.PropArguments.OnKillEffectPattern != null && base.gameObject.activeSelf)
			{
				FireEffect(config.PropArguments.OnKillEffectPattern);
			}
			base.SetDied(killEffect);
		}

		public override void FrameHalt(int frameNum)
		{
			if (frameNum > 0)
			{
				_frameHaltPlugin.FrameHalt(frameNum);
			}
		}

		protected override void Update()
		{
			UpdatePlugins();
			base.Update();
		}

		protected virtual void InitPlugins()
		{
			_frameHaltPlugin = new FrameHaltPlugin(this);
		}

		protected void UpdatePlugins()
		{
			_frameHaltPlugin.Core();
		}

		protected override void OnTimeScaleChanged(float newTimeScale)
		{
			if (animator != null)
			{
				animator.speed = newTimeScale;
			}
		}

		uint IFrameHaltable.GetRuntimeID()
		{
			return GetRuntimeID();
		}
	}
}
