using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.Events;

namespace MoleMole
{
	public class GalTouchSystem
	{
		private const int REACTION_RECORD_COUNT = 20;

		private const string CONFIG_FACE_ANIMATION_PATH = "FaceAnimation/";

		private const string PREFAB_FACE_EFFECT_PATH = "FaceEffect/";

		private Animator _animator;

		private FaceAnimation _faceAnimation;

		private FaceEffect _faceEffect;

		private int _heartLevel;

		private List<TouchPatternItem> _touchPatternList;

		private List<int> _itemIndexRecord;

		private bool _init;

		private bool _idle = true;

		public bool enable { get; set; }

		public bool idle
		{
			get
			{
				if (_idle)
				{
					return _idle;
				}
				if (_animator == null)
				{
					return true;
				}
				_idle = _animator.GetCurrentAnimatorStateInfo(0).IsName("StandBy") && !_faceAnimation.isPlaying && !_animator.IsInTransition(0);
				if (this.IdleChanged != null)
				{
					this.IdleChanged(_idle);
				}
				return _idle;
			}
		}

		public int heartLevel
		{
			get
			{
				return _heartLevel;
			}
			set
			{
				_heartLevel = value;
			}
		}

		public event UnityAction<bool> IdleChanged;

		public event UnityAction<int> TouchPatternTriggered;

		public void Init(Animator animator, int characterId, int heartLevel, Renderer leftEyeRenderer, Renderer rightEyeRenderer, Renderer mouthRenderer, IFaceMatInfoProvider leftEyeProvider, IFaceMatInfoProvider rightEyeProvider, IFaceMatInfoProvider mouthProvider, Transform headRoot = null)
		{
			if (_init)
			{
				return;
			}
			_animator = animator;
			_heartLevel = heartLevel;
			string text = CharacterName(characterId);
			_touchPatternList = TouchPatternData.GetTouchPatternList(text);
			_itemIndexRecord = new List<int>(20);
			_faceAnimation = new FaceAnimation();
			ConfigFaceAnimation faceAnimation = FaceAnimationData.GetFaceAnimation(text);
			FacePartControl facePartControl = new FacePartControl();
			facePartControl.Init(leftEyeProvider, leftEyeRenderer);
			FacePartControl facePartControl2 = new FacePartControl();
			facePartControl2.Init(rightEyeProvider, rightEyeRenderer);
			FacePartControl facePartControl3 = new FacePartControl();
			facePartControl3.Init(mouthProvider, mouthRenderer);
			_faceAnimation.Setup(faceAnimation, facePartControl, facePartControl2, facePartControl3);
			if (_faceEffect != null)
			{
				_faceEffect.Uninit();
				_faceEffect = null;
			}
			if (headRoot != null)
			{
				GameObject gameObject = Resources.Load<GameObject>("FaceEffect/FFX_" + text);
				if (gameObject != null)
				{
					GameObject gameObject2 = Object.Instantiate(gameObject);
					MonoFaceEffect component = gameObject2.GetComponent<MonoFaceEffect>();
					if (component != null)
					{
						_faceEffect = new FaceEffect();
						gameObject2.transform.SetParent(headRoot, false);
						_faceEffect.Init(component);
					}
					else
					{
						Object.Destroy(gameObject2);
					}
				}
			}
			enable = false;
			_init = true;
		}

		private string CharacterName(int id)
		{
			switch (id / 100)
			{
			case 1:
				return "Kiana";
			case 2:
				return "Mei";
			case 3:
				return "Bronya";
			default:
				return null;
			}
		}

		public bool BodyPartTouched(BodyPartType type)
		{
			if (_touchPatternList == null)
			{
				return false;
			}
			if (!enable)
			{
				return false;
			}
			int index = -1;
			bool advance = false;
			ReactionPattern patternByBodyPartTypeAndHeartLevel = GetPatternByBodyPartTypeAndHeartLevel(type, _heartLevel, out index, out advance);
			if (patternByBodyPartTypeAndHeartLevel == null)
			{
				return false;
			}
			bool flag = ActReactionPattern(patternByBodyPartTypeAndHeartLevel);
			if (flag)
			{
				if (advance)
				{
					_itemIndexRecord.Clear();
				}
				else
				{
					if (_itemIndexRecord.Count >= 20)
					{
						_itemIndexRecord.RemoveAt(0);
					}
					_itemIndexRecord.Add(index);
				}
				_idle = false;
				if (this.IdleChanged != null)
				{
					this.IdleChanged(_idle);
				}
				if (this.TouchPatternTriggered != null)
				{
					int num = 0;
					switch (type)
					{
					case BodyPartType.Face:
						num = 1;
						break;
					case BodyPartType.Head:
						num = 2;
						break;
					case BodyPartType.Chest:
						num = ((!advance) ? 3 : 4);
						break;
					case BodyPartType.Private:
						num = ((!advance) ? 5 : 6);
						break;
					case BodyPartType.Arm:
						num = 7;
						break;
					case BodyPartType.Stomach:
						num = 8;
						break;
					case BodyPartType.Leg:
						num = 9;
						break;
					}
					if (num != 0)
					{
						this.TouchPatternTriggered(num);
					}
				}
			}
			return flag;
		}

		public void StopFaceAnimation()
		{
			if (_faceAnimation != null)
			{
				_faceAnimation.Stop();
			}
			if (_faceEffect != null)
			{
				_faceEffect.HideAll();
			}
		}

		public void StopVoice()
		{
			if (_animator != null && Singleton<WwiseAudioManager>.Instance != null)
			{
				Singleton<WwiseAudioManager>.Instance.StopAll(_animator.gameObject);
			}
		}

		public void Process(float dt)
		{
			if (_faceAnimation == null)
			{
				return;
			}
			_faceAnimation.Process(dt);
			if (!_idle && !_faceAnimation.isPlaying && _animator.GetCurrentAnimatorStateInfo(0).IsName("StandBy") && !_animator.IsInTransition(0))
			{
				if (_faceEffect != null)
				{
					_faceEffect.HideAll();
				}
				_idle = true;
				if (this.IdleChanged != null)
				{
					this.IdleChanged(_idle);
				}
			}
		}

		private bool ActReactionPattern(ReactionPattern pattern)
		{
			if (!idle)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(pattern.bodyStateName) && _animator != null)
			{
				_animator.CrossFadeInFixedTime(pattern.bodyStateName, 0.3f, 0);
			}
			if (!string.IsNullOrEmpty(pattern.faceStateName) && _faceAnimation != null)
			{
				_faceAnimation.PlayFaceAnimation(pattern.faceStateName);
			}
			if (!string.IsNullOrEmpty(pattern.faceEffectName) && _faceEffect != null)
			{
				_faceEffect.ShowEffect(pattern.faceEffectName);
			}
			return true;
		}

		private ReactionPattern GetPatternByBodyPartTypeAndHeartLevel(BodyPartType bodyPartType, int heartLevel, out int index, out bool advance)
		{
			ReactionPattern reactionPattern = null;
			index = -1;
			advance = false;
			int i = 0;
			for (int count = _touchPatternList.Count; i < count; i++)
			{
				TouchPatternItem touchPatternItem = _touchPatternList[i];
				if (touchPatternItem.bodyPartType == bodyPartType && touchPatternItem.heartLevel == heartLevel)
				{
					reactionPattern = touchPatternItem.reactionPattern;
					index = i;
				}
				if (reactionPattern == null)
				{
					continue;
				}
				if (touchPatternItem.advanceTime <= 0)
				{
					break;
				}
				bool flag = true;
				int j = 0;
				for (int num = touchPatternItem.advanceTime - 1; j < num; j++)
				{
					if (j >= _itemIndexRecord.Count || _itemIndexRecord[_itemIndexRecord.Count - j - 1] != i)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					reactionPattern = touchPatternItem.advanceReactionPattern;
					advance = true;
				}
				break;
			}
			return reactionPattern;
		}
	}
}
