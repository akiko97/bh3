using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class HintArrowManager
	{
		private const string RING_IN_ANIM = "RingAltIn";

		private const string RING_OUT_ANIM = "RingAltOut";

		private const float CHANGE_DIR_LERP_RATIO = 10f;

		private const string HINT_RING_PATH = "UI/HintArrowAlt/RingAlt";

		public const string MONSTER_HINT_PATH = "UI/HintArrowAlt/HintArrowMonsterAlt";

		public const string EXIT_HINT_PATH = "UI/HintArrowAlt/HintArrowExitAlt";

		public const string AVATAR_HINT_PATH = "UI/HintArrowAlt/HintArrowAvatarAlt";

		private List<MonoHintArrow> _hintArrowLs;

		private MonoSpawnPoint _pathSpawn;

		private MonoHintArrow _hintArrowForPath;

		private GameObject _hintRing;

		private Animation _hintRingAnim;

		private AnimationState _hintRingOutAnimState;

		private bool _hintRingVisible;

		public void InitAtStart()
		{
			_hintRing = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>("UI/HintArrowAlt/RingAlt"));
			_hintRing.name = "_HintRing";
			_hintRing.SetActive(false);
			_hintRingVisible = false;
			_hintRingAnim = _hintRing.GetComponent<Animation>();
			_hintRingOutAnimState = _hintRingAnim["RingAltOut"];
			_hintArrowLs = new List<MonoHintArrow>();
		}

		private void SetHintRingPosition()
		{
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			if (!(baseMonoAvatar == null))
			{
				Vector3 position = baseMonoAvatar.transform.position;
				position.y = _hintRing.transform.position.y;
				_hintRing.transform.position = position;
			}
		}

		public void SetHintRingVisible(bool visible)
		{
			if (visible)
			{
				if (!_hintRing.gameObject.activeSelf)
				{
					_hintRing.gameObject.SetActive(true);
				}
				_hintRingAnim.Play("RingAltIn");
			}
			else
			{
				_hintRingAnim.Play("RingAltOut");
			}
			_hintRingVisible = visible;
		}

		public void Core()
		{
			UpdateAllHintArrow();
		}

		public void AddHintArrow(uint listenRuntimeID)
		{
			BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(listenRuntimeID);
			string empty = string.Empty;
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(listenRuntimeID) == 4)
			{
				empty = "UI/HintArrowAlt/HintArrowMonsterAlt";
			}
			else if (Singleton<RuntimeIDManager>.Instance.ParseCategory(listenRuntimeID) == 3)
			{
				empty = "UI/HintArrowAlt/HintArrowAvatarAlt";
			}
			else
			{
				if (!(entity as BaseMonoDynamicObject != null) || Singleton<EventManager>.Instance.GetActor(entity.GetRuntimeID()) == null)
				{
					throw new Exception("Invalid Type or State!");
				}
				empty = "UI/HintArrowAlt/HintArrowExitAlt";
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(empty));
			MonoHintArrow component = gameObject.GetComponent<MonoHintArrow>();
			component.Init(listenRuntimeID, entity);
			component.transform.SetParent(_hintRing.transform, false);
			_hintArrowLs.Add(component);
		}

		public void AddHintArrowForPath(MonoSpawnPoint spawn)
		{
			if (_hintArrowForPath != null)
			{
				RemoveHintArrowForPath();
			}
			string path = "UI/HintArrowAlt/HintArrowExitAlt";
			GameObject gameObject = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(path));
			MonoHintArrow component = gameObject.GetComponent<MonoHintArrow>();
			component.Init(0u);
			component.transform.SetParent(_hintRing.transform, false);
			_pathSpawn = spawn;
			_hintArrowForPath = component;
			_hintArrowForPath.SetVisible(true);
			UpdateHintArrow(component.transform, spawn.XZPosition, false);
		}

		public void RemoveHintArrowForPath()
		{
			if (!(_hintArrowForPath == null))
			{
				_hintArrowForPath.SetVisible(false);
				_hintArrowForPath.SetDestroyUponFadeOut();
			}
		}

		private void UpdateAllHintArrow()
		{
			SetHintRingPosition();
			bool flag = false;
			for (int i = 0; i < _hintArrowLs.Count; i++)
			{
				if (!(_hintArrowLs[i] == null))
				{
					MonoHintArrow monoHintArrow = _hintArrowLs[i];
					SetHintArrowByScreenPos(monoHintArrow, monoHintArrow.listenEntity);
					if (monoHintArrow.state != MonoHintArrow.State.Hidden)
					{
						flag = true;
					}
				}
			}
			UpdateHintArrowForPath();
			flag |= _hintArrowForPath != null;
			if (flag != _hintRingVisible)
			{
				SetHintRingVisible(flag);
			}
			if (!_hintRingVisible && _hintRingOutAnimState.normalizedTime > 1f)
			{
				_hintRing.SetActive(false);
			}
		}

		private void SetHintArrowByScreenPos(MonoHintArrow arrow, BaseMonoEntity entity)
		{
			bool flag = entity != null && entity.IsActive() && !Singleton<AvatarManager>.Instance.IsLocalAvatar(entity.GetRuntimeID()) && !Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(entity);
			bool flag2 = arrow.state == MonoHintArrow.State.Visible || arrow.state == MonoHintArrow.State.FadingIn;
			bool flag3 = entity == null || entity.IsToBeRemove();
			if (arrow.state == MonoHintArrow.State.Hidden && flag3)
			{
				UnityEngine.Object.Destroy(arrow.gameObject);
			}
			else if (flag2 && !flag)
			{
				arrow.SetVisible(false);
				if (flag3)
				{
					arrow.SetDestroyUponFadeOut();
				}
			}
			else if (!flag2 && flag)
			{
				arrow.SetVisible(true);
			}
			if (flag)
			{
				UpdateHintArrow(arrow.transform, entity.XZPosition, flag2);
			}
		}

		private void UpdateHintArrowForPath()
		{
			if (!(_hintArrowForPath == null) && !(_pathSpawn == null))
			{
				UpdateHintArrow(_hintArrowForPath.transform, _pathSpawn.XZPosition, true);
			}
		}

		private void UpdateHintArrow(Transform hintArrowTrans, Vector3 targetXZPosition, bool isArrowVisibleBefore)
		{
			Vector3 forward = hintArrowTrans.forward;
			Vector3 vector = targetXZPosition - Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
			vector.y = 0f;
			vector.Normalize();
			if (isArrowVisibleBefore)
			{
				hintArrowTrans.forward = Vector3.Lerp(forward, vector, 10f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime);
			}
			else
			{
				hintArrowTrans.forward = vector;
			}
		}

		public void TriggerHintArrowEffect(uint runtimeID, MonoHintArrow.EffectType effectType)
		{
			for (int i = 0; i < _hintArrowLs.Count; i++)
			{
				if (!(_hintArrowLs[i] == null) && _hintArrowLs[i].listenRuntimID == runtimeID)
				{
					_hintArrowLs[i].TriggerEffect(effectType);
					break;
				}
			}
		}

		public MonoSpawnPoint GetSpawnPoint()
		{
			return _pathSpawn;
		}
	}
}
