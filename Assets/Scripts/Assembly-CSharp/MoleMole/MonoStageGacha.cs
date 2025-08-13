using System;
using MoleMole.MainMenu;
using UnityEngine;
using proto;
using Material = UnityEngine.Material;

namespace MoleMole
{
	public class MonoStageGacha : MonoBehaviour
	{
		[SerializeField]
		private float _duration = 3.5f;

		[SerializeField]
		private GameObject _boxFPPrefab;

		[SerializeField]
		private GameObject _boxFPBigPrefab;

		[SerializeField]
		private GameObject _boxHCPrefab;

		[SerializeField]
		private GameObject _boxHCBigPrefab;

		[SerializeField]
		private GameObject _boxSPPrefab;

		[SerializeField]
		private GameObject _boxSPBigPrefab;

		[SerializeField]
		private float _boxScale = 1f;

		[SerializeField]
		private float _tenBoxScale = 1f;

		[SerializeField]
		private Material _matEquipment;

		[SerializeField]
		private Material _matAvatar;

		[SerializeField]
		private Material _matStageFP;

		[SerializeField]
		private Material _matStageHC;

		[SerializeField]
		private Material _matStageSP;

		[SerializeField]
		private GameObject _uiFadeEffect;

		[SerializeField]
		private float _FadeStart;

		[SerializeField]
		private float _FadeDuration;

		private bool _bUIFadeEffect;

		private float _time;

		private GameObject _box;

		private GameObject _uiCamera;

		private GameObject _spaceShip;

		private GameObject _warShip;

		private GameObject _gachaEffect;

		private Animator _gachaCameraAnimator;

		private MainMenuStage _mainMenuStage;

		private static readonly string ATMOSPHERE_PATH = "Rendering/MainMenuAtmosphereConfig/GachaSky";

		private GachaType _type;

		private GachaMainPageContext.GachaAmountType _amountType;

		private Action _endCallBack;

		public string audioEventName;

		public float audioEventDelay;

		private bool _audioEventPosted;

		private void Start()
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Invalid comparison between Unknown and I4
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Invalid comparison between Unknown and I4
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Invalid comparison between Unknown and I4
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Invalid comparison between Unknown and I4
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Invalid comparison between Unknown and I4
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Invalid comparison between Unknown and I4
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Invalid comparison between Unknown and I4
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Invalid comparison between Unknown and I4
			ChangeAtmosphere();
			_uiCamera.SetActive(false);
			if (_spaceShip != null)
			{
				_spaceShip.SetActive(true);
				_warShip = _spaceShip.transform.Find("Warship").gameObject;
				if (_warShip != null)
				{
					_warShip.SetActive(false);
				}
			}
			Transform transform = base.transform.Find("GachaStage/boxInitPositon");
			if ((int)_type == 2)
			{
				_box = UnityEngine.Object.Instantiate((_amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? _boxHCBigPrefab : _boxHCPrefab);
			}
			else if ((int)_type == 1)
			{
				_box = UnityEngine.Object.Instantiate((_amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? _boxFPBigPrefab : _boxFPPrefab);
			}
			else if ((int)_type == 3)
			{
				_box = UnityEngine.Object.Instantiate((_amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? _boxSPBigPrefab : _boxSPPrefab);
			}
			else
			{
				_box = null;
			}
			_box.transform.position = transform.position;
			_box.transform.rotation = transform.rotation;
			float num = ((_amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? _tenBoxScale : _boxScale);
			_box.transform.localScale = new Vector3(num, num, num);
			Transform transform2 = base.transform.Find("background");
			transform2.GetComponent<MeshRenderer>().material = (((int)_type != 1) ? _matAvatar : _matEquipment);
			MeshRenderer[] componentsInChildren = base.transform.Find("GachaStage").GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if ((int)_type == 2)
				{
					meshRenderer.material = _matStageHC;
				}
				else if ((int)_type == 1)
				{
					meshRenderer.material = _matStageFP;
				}
				else if ((int)_type == 3)
				{
					meshRenderer.material = _matStageSP;
				}
			}
			_gachaEffect = base.transform.Find("GachaEffect/GachaEffectUI_Colored").gameObject;
			_gachaEffect.SetActive((int)_type != 1);
			_gachaCameraAnimator = base.transform.Find("3DCamera").GetComponent<Animator>();
			if (_amountType == GachaMainPageContext.GachaAmountType.GachaOne)
			{
				_gachaCameraAnimator.SetTrigger("GachaOneTrigger");
			}
			else if (_amountType == GachaMainPageContext.GachaAmountType.GachaTen)
			{
				_gachaCameraAnimator.SetTrigger("GachaTenTrigger");
			}
			if (audioEventDelay == 0f)
			{
				Singleton<WwiseAudioManager>.Instance.Post(audioEventName);
				_audioEventPosted = true;
			}
			_bUIFadeEffect = false;
		}

		public void Init(GameObject spaceShip, GameObject uiCamera, GachaType type, GachaMainPageContext.GachaAmountType amountType, Action endCallBack = null)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			_spaceShip = spaceShip;
			_uiCamera = uiCamera;
			_type = type;
			_endCallBack = endCallBack;
			_amountType = amountType;
			_mainMenuStage = ((!(spaceShip == null)) ? spaceShip.GetComponent<MainMenuStage>() : null);
		}

		private void Update()
		{
			_time += Time.deltaTime;
			if (_time >= _duration)
			{
				_uiCamera.SetActive(true);
				if (_warShip != null)
				{
					_warShip.SetActive(true);
				}
				if (_spaceShip != null)
				{
					_spaceShip.SetActive(false);
				}
				if (_endCallBack != null)
				{
					_endCallBack();
				}
				UnityEngine.Object.Destroy(_box);
				UnityEngine.Object.Destroy(base.gameObject);
				RestoreAtmosphere();
			}
			if (_time > _FadeStart && !_bUIFadeEffect)
			{
				_bUIFadeEffect = true;
				GameObject gameObject = UnityEngine.Object.Instantiate(_uiFadeEffect);
				gameObject.GetComponent<MonoUIFadeEffect>().Init(_FadeDuration);
			}
			if (_time > audioEventDelay && !_audioEventPosted)
			{
				_audioEventPosted = true;
				Singleton<WwiseAudioManager>.Instance.Post(audioEventName);
			}
		}

		private void ChangeAtmosphere()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Invalid comparison between Unknown and I4
			float time = 0f;
			if ((int)_type == 2)
			{
				time = 18f;
			}
			else if ((int)_type == 1)
			{
				time = 12f;
			}
			else if ((int)_type == 3)
			{
				time = 18f;
			}
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigAtmosphereSeries.LoadFromFileAndDetach(ATMOSPHERE_PATH);
			int key = configAtmosphereSeries.KeyBeforeTime(time);
			ConfigAtmosphere config = configAtmosphereSeries.Value(key);
			if (_mainMenuStage != null)
			{
				_mainMenuStage.SetupAtmosphere(configAtmosphereSeries.Common, config);
				_mainMenuStage.IsUpdateAtmosphereAuto = false;
				_mainMenuStage.ForceUpdateAtmosphere = false;
			}
		}

		private void RestoreAtmosphere()
		{
			if (_mainMenuStage != null)
			{
				_mainMenuStage.IsUpdateAtmosphereAuto = true;
				_mainMenuStage.ForceUpdateAtmosphere = true;
			}
		}
	}
}
