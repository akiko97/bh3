using System;
using MoleMole.MPProtocol;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseAnimatorEntityIdentity : BaseAbilityEntityIdentiy
	{
		protected BaseMonoAnimatorEntity _animatorEntity;

		private float _sendInterval;

		private Vector3 _lastSetForward;

		private Vector3 _lastSendXZ;

		private float _lastSentXZAngle;

		private float _lastSentTime;

		private int[] _muteSyncTagHashes;

		public BaseAnimatorEntityIdentity()
		{
		}

		public override void Core()
		{
			base.Core();
			if (base.isAuthority)
			{
				AuthorityCore();
			}
		}

		public override void OnAuthorityStart()
		{
			base.OnAuthorityStart();
			BaseMonoAnimatorEntity animatorEntity = _animatorEntity;
			animatorEntity.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Combine(animatorEntity.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(AuthorityOnAnimatorStateChanged));
			BaseMonoAnimatorEntity animatorEntity2 = _animatorEntity;
			animatorEntity2.onUserInputControllerChanged = (Action<AnimatorParameterEntry>)Delegate.Combine(animatorEntity2.onUserInputControllerChanged, new Action<AnimatorParameterEntry>(AuthorityUserInputControllerChanged));
			BaseMonoAnimatorEntity animatorEntity3 = _animatorEntity;
			animatorEntity3.onSteerFaceDirectionSet = (Action<Vector3>)Delegate.Combine(animatorEntity3.onSteerFaceDirectionSet, new Action<Vector3>(AuthorityOnFaceDirectionSet));
			_lastSentTime = Time.time;
			_lastSentXZAngle = float.MinValue;
			_lastSendXZ = MPMiscs.UNITINIALIZED;
			_sendInterval = _animatorEntity.animatorConfig.MPArguments.SyncSendInterval;
			string[] muteSyncAnimatorTags = _animatorEntity.animatorConfig.MPArguments.MuteSyncAnimatorTags;
			_muteSyncTagHashes = new int[muteSyncAnimatorTags.Length];
			for (int i = 0; i < muteSyncAnimatorTags.Length; i++)
			{
				_muteSyncTagHashes[i] = Animator.StringToHash(muteSyncAnimatorTags[i]);
			}
		}

		public void AuthorityOnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (!Miscs.ArrayContains(_muteSyncTagHashes, toState.tagHash))
			{
				MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_AnimatorStateChange>();
				Packet_Entity_AnimatorStateChange.StartPacket_Entity_AnimatorStateChange(pc.builder);
				Packet_Entity_AnimatorStateChange.AddNormalizedTimeTo(pc.builder, (fromState.shortNameHash != toState.shortNameHash) ? toState.normalizedTime : 0f);
				Packet_Entity_AnimatorStateChange.AddToStateHash(pc.builder, toState.shortNameHash);
				Packet_Entity_AnimatorStateChange.AddStateSync(pc.builder, EntityStateSync.CreateEntityStateSync(pc.builder, _animatorEntity.XZPosition.x, _animatorEntity.XZPosition.z, MPMiscs.ForwardToXZAngles(_animatorEntity.FaceDirection)));
				pc.Finish(Packet_Entity_AnimatorStateChange.EndPacket_Entity_AnimatorStateChange(pc.builder));
				Singleton<MPManager>.Instance.SendStateUpdateToOthers(runtimeID, pc);
			}
		}

		public void AuthorityUserInputControllerChanged(AnimatorParameterEntry entry)
		{
			MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_AnimatorParameterChange>();
			Packet_Entity_AnimatorParameterChange.StartPacket_Entity_AnimatorParameterChange(pc.builder);
			Packet_Entity_AnimatorParameterChange.AddStateHash(pc.builder, entry.stateHash);
			Packet_Entity_AnimatorParameterChange.AddParameter(pc.builder, (byte)entry.type);
			switch (entry.type)
			{
			case AnimatorControllerParameterType.Bool:
				Packet_Entity_AnimatorParameterChange.AddBoolValue(pc.builder, entry.boolValue);
				break;
			case AnimatorControllerParameterType.Int:
				Packet_Entity_AnimatorParameterChange.AddIntValue(pc.builder, entry.intValue);
				break;
			case AnimatorControllerParameterType.Float:
				Packet_Entity_AnimatorParameterChange.AddFloatValue(pc.builder, entry.floatValue);
				break;
			}
			pc.Finish(Packet_Entity_AnimatorParameterChange.EndPacket_Entity_AnimatorParameterChange(pc.builder));
			Singleton<MPManager>.Instance.SendReliableToOthers(runtimeID, pc);
		}

		public void AuthorityOnFaceDirectionSet(Vector3 forward)
		{
			_lastSetForward = forward;
		}

		private void AuthorityCore()
		{
			AuthoritySendTransformSyncCore();
		}

		protected virtual void AuthoritySendTransformSyncCore()
		{
			if (Time.time - _lastSentTime > _sendInterval)
			{
				float num = MPMiscs.ForwardToXZAngles(_lastSetForward);
				Vector3 xZPosition = _animatorEntity.XZPosition;
				if (MPMiscs.NeedSyncTransform((xZPosition - _lastSendXZ).sqrMagnitude, num - _lastSentXZAngle))
				{
					MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_TransformSync>();
					Packet_Entity_TransformSync.StartPacket_Entity_TransformSync(pc.builder);
					Packet_Entity_TransformSync.AddStateSync(pc.builder, EntityStateSync.CreateEntityStateSync(pc.builder, _animatorEntity.XZPosition.x, _animatorEntity.XZPosition.z, num));
					pc.Finish(Packet_Entity_TransformSync.EndPacket_Entity_TransformSync(pc.builder));
					Singleton<MPManager>.Instance.SendStateUpdateToOthers(runtimeID, pc);
					_lastSendXZ = xZPosition;
					_lastSentXZAngle = num;
					_lastSentTime = Time.time;
				}
			}
		}

		public override void OnRemoteStart()
		{
			base.OnRemoteStart();
			_animatorEntity.SetUseLocalController(false);
		}

		protected override void OnRemoteReliablePacket(MPRecvPacketContainer pc)
		{
			base.OnRemoteReliablePacket(pc);
			if (pc.packet is Packet_Entity_AnimatorParameterChange)
			{
				OnRemoteEntityAnimatorParameterChanged(pc.As<Packet_Entity_AnimatorParameterChange>());
			}
		}

		protected override void OnRemoteStateUpdate(MPRecvPacketContainer pc)
		{
			base.OnRemoteReliablePacket(pc);
			if (pc.packet is Packet_Entity_AnimatorStateChange)
			{
				OnRemoteEntityAnimatorStateChanged(pc.As<Packet_Entity_AnimatorStateChange>());
			}
			else if (pc.packet is Packet_Entity_TransformSync)
			{
				OnRemoteTransformSync(pc.As<Packet_Entity_TransformSync>());
			}
		}

		protected virtual void OnRemoteEntityAnimatorStateChanged(Packet_Entity_AnimatorStateChange packet)
		{
			_animatorEntity.SyncAnimatorState(packet.ToStateHash, packet.NormalizedTimeTo);
			_animatorEntity.SteerFaceDirectionTo(MPMiscs.XZAnglesToForward(packet.StateSync.XzAngle));
		}

		protected virtual void OnRemoteEntityAnimatorParameterChanged(Packet_Entity_AnimatorParameterChange packet)
		{
			switch ((AnimatorControllerParameterType)packet.Parameter)
			{
			case AnimatorControllerParameterType.Bool:
				_animatorEntity.SetLocomotionBool(packet.StateHash, packet.BoolValue);
				break;
			case AnimatorControllerParameterType.Int:
				_animatorEntity.SetLocomotionInteger(packet.StateHash, packet.IntValue);
				break;
			case AnimatorControllerParameterType.Float:
				_animatorEntity.SetLocomotionFloat(packet.StateHash, packet.FloatValue);
				break;
			case (AnimatorControllerParameterType)2:
				break;
			}
		}

		protected virtual void OnRemoteTransformSync(Packet_Entity_TransformSync packet)
		{
			_animatorEntity.SteerFaceDirectionTo(MPMiscs.XZAnglesToForward(packet.StateSync.XzAngle));
			Vector3 vector = MPMiscs.Convert(packet.StateSync.XzPosition);
			if ((vector - _animatorEntity.XZPosition).sqrMagnitude > 8f)
			{
				_animatorEntity.SyncPosition(vector);
			}
		}
	}
}
