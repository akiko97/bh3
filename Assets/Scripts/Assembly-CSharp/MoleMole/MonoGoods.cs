using System;
using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoGoods : BaseMonoDynamicObject
	{
		public enum GoodsState
		{
			Appear = 0,
			Idle = 1,
			Attract = 2,
			Consumed = 3
		}

		public const float DEFAULT_HEIGHT = 0.3f;

		public const float GOODS_MIN_RADIUS = 0.25f;

		private const float MAX_ATTRACT_TIME = 2.5f;

		private const string GREEN_TINT_1 = "#00E48F7F";

		private const string GREEN_TINT_2 = "#95FF967F";

		private const string BLUE_TINT_1 = "#0041FF7F";

		private const string BLUE_TINT_2 = "#7081FF7F";

		private const string PURPLE_TINT_1 = "#3700FF7F";

		private const string PURPLE_TINT_2 = "#8E72FF7F";

		private const float ROTATE_SPEED_ON_GROUND = 100f;

		private EntityTimer attractTimer;

		public int DropItemMetaID = -1;

		public int DropItemLevel;

		public int DropItemNum;

		public List<MonoEffect> effects;

		public float attractRadius;

		public float speed;

		public float acceleration = 10f;

		[NonSerialized]
		public bool actDropAnim = true;

		[NonSerialized]
		public bool dropAnimFinished;

		[NonSerialized]
		public bool forceFlyToAvatar;

		[NonSerialized]
		public bool muteSound;

		public int reboundTimes;

		[ShowInInspector]
		private GoodsState _state;

		private Rigidbody _rigidbody;

		private bool _isToBeRemoved;

		private float _selfRotateSpeed = 1100f;

		private float _selfRotateAcceleration = 30f;

		private bool _collisionChecked;

		[Header("Attached Effect Pattern")]
		public string AttachEffectPattern;

		[Header("Inside Effect Pattern")]
		public string InsideEffectPattern;

		private bool _hasTriggerEntered;

		public List<MonoEffect> OutsideEffects
		{
			get
			{
				return effects;
			}
		}

		public GoodsState state
		{
			set
			{
				_state = value;
			}
		}

		protected void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.useGravity = false;
			_hasTriggerEntered = false;
			attractTimer = new EntityTimer(2.5f);
			attractTimer.SetActive(false);
		}

		protected override void Start()
		{
			base.Start();
			if (actDropAnim)
			{
				_rigidbody.useGravity = true;
				_rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f) * 2f, UnityEngine.Random.Range(1.5f, 2f) * 2f, UnityEngine.Random.Range(-1f, 1f) * 2f);
				_rigidbody.AddForce(Vector3.down * 3f * _rigidbody.mass);
				dropAnimFinished = false;
				reboundTimes = 2;
			}
			else
			{
				dropAnimFinished = true;
				reboundTimes = -1;
				UnityEngine.Object.Destroy(_rigidbody);
				_rigidbody = null;
				base.transform.SetLocalPositionY(0.3f);
			}
		}

		protected override void Update()
		{
			float angle;
			if (!actDropAnim)
			{
				angle = Time.deltaTime * 100f * TimeScale;
			}
			else if (actDropAnim && !dropAnimFinished)
			{
				angle = Time.deltaTime * _selfRotateSpeed * TimeScale;
				CheckBarrierCollider();
			}
			else
			{
				_selfRotateAcceleration = ((!(_selfRotateAcceleration < 10f)) ? (_selfRotateAcceleration - 3f) : 10f);
				_selfRotateSpeed = ((!(_selfRotateSpeed < 100f)) ? (_selfRotateSpeed - _selfRotateAcceleration) : 100f);
				angle = Time.deltaTime * _selfRotateSpeed * TimeScale;
			}
			base.transform.Rotate(base.transform.up, angle);
			if (_state == GoodsState.Appear)
			{
				_state = GoodsState.Idle;
			}
			else if (_state == GoodsState.Idle && base.transform.position.y <= 0.3f && _rigidbody != null && _rigidbody.velocity.y <= 0f)
			{
				if (reboundTimes > 0)
				{
					_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -0.8f * _rigidbody.velocity.y, _rigidbody.velocity.z);
					reboundTimes--;
				}
				else if (!dropAnimFinished)
				{
					dropAnimFinished = true;
					UnityEngine.Object.Destroy(_rigidbody);
					_rigidbody = null;
					base.transform.SetLocalPositionY(0.3f);
					if (!string.IsNullOrEmpty(AttachEffectPattern) && Singleton<EventManager>.Instance.GetActor<EquipItemActor>(GetRuntimeID()) != null)
					{
						bool flag = true;
						GraphicsRecommendGrade graphicsRecommendGrade = GraphicsSettingData.GetGraphicsRecommendGrade();
						if (graphicsRecommendGrade == GraphicsRecommendGrade.Off || graphicsRecommendGrade == GraphicsRecommendGrade.Low)
						{
							ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
							if (!personalGraphicsSetting.IsUserDefinedGrade)
							{
								flag = false;
							}
							else if (personalGraphicsSetting.RecommendGrade != GraphicsRecommendGrade.High)
							{
								flag = false;
							}
						}
						if (flag)
						{
							int rarity = Singleton<EventManager>.Instance.GetActor<EquipItemActor>(GetRuntimeID()).rarity;
							effects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(AttachEffectPattern, this);
							int i = 0;
							for (int count = OutsideEffects.Count; i < count; i++)
							{
								SetOutsideParticleColorByRarity(OutsideEffects[i].gameObject, rarity);
							}
						}
					}
				}
			}
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (_state == GoodsState.Idle)
			{
				if (dropAnimFinished || reboundTimes <= 0)
				{
					float property = localAvatar.GetProperty("Actor_GoodsAttrackRadius");
					property = Mathf.Clamp(property, 0f, property);
					attractRadius = localAvatar.config.CommonArguments.GoodsAttractRadius * property;
					float num = Vector3.Distance(XZPosition, localAvatar.XZPosition);
					if (num < attractRadius)
					{
						_state = GoodsState.Attract;
						attractTimer.SetActive(false);
						if (!string.IsNullOrEmpty(AttachEffectPattern) && Singleton<EventManager>.Instance.GetActor<EquipItemActor>(GetRuntimeID()) != null)
						{
							foreach (MonoEffect effect in effects)
							{
								if (effect != null)
								{
									effect.SetDestroyImmediately();
								}
							}
						}
					}
				}
			}
			else if (_state == GoodsState.Attract || forceFlyToAvatar)
			{
				speed += acceleration * TimeScale * Time.deltaTime;
				speed = ((!(speed < 20f)) ? 20f : speed);
				Vector3 vector = (localAvatar.RootNodePosition - base.transform.position).normalized * speed * TimeScale;
				base.transform.position += vector * Time.deltaTime;
				attractTimer.Core(1f);
				float num2 = Vector3.Distance(XZPosition, localAvatar.XZPosition);
				if (num2 < localAvatar.config.CommonArguments.CollisionRadius || attractTimer.isTimeUp)
				{
					OnTriggerEnter(localAvatar.hitbox);
				}
			}
			base.Update();
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!_hasTriggerEntered)
			{
				BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
				if (componentInParent != null && Singleton<AvatarManager>.Instance.IsLocalAvatar(componentInParent.GetRuntimeID()))
				{
					Singleton<EventManager>.Instance.FireEvent(new EvtFieldEnter(_runtimeID, componentInParent.GetRuntimeID()));
					_hasTriggerEntered = true;
				}
			}
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemoved;
		}

		public override bool IsActive()
		{
			return !_isToBeRemoved;
		}

		public override void SetDied()
		{
			base.SetDied();
			_isToBeRemoved = true;
			Singleton<EffectManager>.Instance.ClearEffectsByOwner(_runtimeID);
		}

		public void SetAttractTimerActive(bool isActive)
		{
			attractTimer.SetActive(isActive);
		}

		private void CheckBarrierCollider()
		{
			if (!actDropAnim || (actDropAnim && dropAnimFinished) || _collisionChecked)
			{
				return;
			}
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (!(localAvatar == null))
			{
				Vector3 start = new Vector3(localAvatar.transform.position.x, 0.01f, localAvatar.transform.position.z);
				Vector3 vector = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
				Vector3 vector2 = new Vector3(base.transform.position.x, 0.01f, base.transform.position.z);
				Vector3 end = vector2 + vector * Time.deltaTime * TimeScale;
				if (Physics.Linecast(start, end, (1 << InLevelData.OBSTACLE_COLLIDER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER)))
				{
					_rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
					_collisionChecked = true;
				}
			}
		}

		public void SetOutsideParticleColorByRarity(GameObject obj, int rarity)
		{
			ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				Renderer component = componentsInChildren[i].GetComponent<Renderer>();
				if (component != null && component.material.shader.name.IndexOf("Channel Mix") != -1)
				{
					Color color = Color.black;
					Color color2 = Color.black;
					switch (rarity)
					{
					case 1:
					case 2:
						color = Miscs.ParseColor("#00E48F7F");
						color2 = Miscs.ParseColor("#95FF967F");
						break;
					case 3:
						color = Miscs.ParseColor("#0041FF7F");
						color2 = Miscs.ParseColor("#7081FF7F");
						break;
					case 4:
					case 5:
					case 6:
						color = Miscs.ParseColor("#3700FF7F");
						color2 = Miscs.ParseColor("#8E72FF7F");
						break;
					}
					component.material.SetColor("_TintColor1", color);
					component.material.SetColor("_TintColor2", color2);
				}
			}
		}
	}
}
