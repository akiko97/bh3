using System.Collections;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class ElevatorModelContext : BaseWidgetContext
	{
		public enum DescImageType
		{
			Loading = 0,
			Identifying = 1,
			Confirmed = 2
		}

		private const string LOADING_IMG_PATH = "GameEntry/Texture/Elevator_Display01";

		private const string IDENTIFY_IMG_PATH = "GameEntry/Texture/Elevator_Display02";

		private const string CONFIRMED_IMG_PATH = "GameEntry/Texture/Elevator_Display03";

		private Coroutine _checkFloorPhase1AnimOverCoroutine;

		private Coroutine _checkFloorPhase2AnimOverCoroutine;

		private Renderer _renderer;

		private MaterialPropertyBlock _block;

		private int _shaderMaintexID;

		public ElevatorModelContext(GameObject view)
		{
			config = new ContextPattern
			{
				contextName = "ElevatorModelContext",
				viewPrefabPath = "GameEntry/Elevator"
			};
			base.view = view;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.PlayAnimAfterLoad)
			{
				return OnPlayAnimAfterLoad();
			}
			return false;
		}

		protected override bool SetupView()
		{
			_renderer = base.view.transform.Find("Elevator/Elevator 1/StartLoading_Screen02").GetComponent<MeshRenderer>();
			_block = new MaterialPropertyBlock();
			_shaderMaintexID = Shader.PropertyToID("_MainTex");
			base.view.transform.Find("Elevator/OutsideDoor").localPosition = new Vector3(0f, 10f, 0f);
			PlayLoadingAnimation();
			return false;
		}

		public override void Destroy()
		{
			StopCheckFloorPhase1AnimOver();
			StopCheckFloorPhase2AnimOver();
			base.Destroy();
		}

		private bool OnPlayAnimAfterLoad()
		{
			PlayLoadingAnimation();
			return false;
		}

		public void PlayLoadingAnimation()
		{
			PlayElevatorAnimation();
			PlayPillarAnimation();
			MonoElevatorProjectiveLight[] componentsInChildren = base.view.GetComponentsInChildren<MonoElevatorProjectiveLight>();
			MonoElevatorProjectiveLight[] array = componentsInChildren;
			foreach (MonoElevatorProjectiveLight monoElevatorProjectiveLight in array)
			{
				monoElevatorProjectiveLight.enabled = true;
			}
			EnableBackgroundAnim();
		}

		private void EnableBackgroundAnim()
		{
			GameObject gameObject = base.view;
			MonoGameEntryElevator component = gameObject.GetComponent<MonoGameEntryElevator>();
			if (component != null)
			{
				component.EnablePlayBackgroundAnim = true;
			}
		}

		public void PlayElevatorAnimation()
		{
			GameObject gameObject = base.view;
			Animation component = gameObject.GetComponent<Animation>();
			component.Play("Elevator", PlayMode.StopAll);
		}

		public void PlayFloorPhase1Animation()
		{
			GameObject gameObject = base.view;
			Animation component = gameObject.GetComponent<Animation>();
			component.Blend("FloorPhase1");
			AnimationState floorState = null;
			foreach (AnimationState item in component)
			{
				if (item.name == "FloorPhase1")
				{
					floorState = item;
					break;
				}
			}
			_checkFloorPhase1AnimOverCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(CheckFloorPhase1AnimOver(floorState));
		}

		public void PlayFloorPhase2Animation()
		{
			GameObject gameObject = base.view;
			Animation component = gameObject.GetComponent<Animation>();
			component.Play("FloorPhase2", PlayMode.StopAll);
			AnimationState floorState = null;
			foreach (AnimationState item in component)
			{
				if (item.name == "FloorPhase2")
				{
					floorState = item;
					break;
				}
			}
			_checkFloorPhase2AnimOverCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(CheckFloorPhase2AnimOver(floorState));
		}

		public void PlayPillarAnimation()
		{
			GameObject gameObject = base.view;
			Animation component = gameObject.GetComponent<Animation>();
			component.Blend("Pillar");
		}

		public void PlayDoorAnimation()
		{
			GameObject gameObject = base.view;
			Animation component = gameObject.GetComponent<Animation>();
			component.Play("Door", PlayMode.StopAll);
		}

		public void PlayBackAnimation()
		{
			GameObject gameObject = base.view;
			Animation component = gameObject.GetComponent<Animation>();
			component.Play("ElevatorBack", PlayMode.StopAll);
		}

		public void HideSomeParts()
		{
			GameObject gameObject = base.view;
			gameObject.transform.Find("Elevator/Pillar").gameObject.SetActive(false);
			gameObject.transform.Find("Elevator/InnerDoor/Shadow02").gameObject.SetActive(false);
			gameObject.transform.Find("Elevator/Shadow_Left").gameObject.SetActive(false);
			gameObject.transform.Find("Elevator/Shadow_Right").gameObject.SetActive(false);
			gameObject.transform.Find("StartLoading_BG01").gameObject.SetActive(false);
			gameObject.transform.Find("StartLoading_BG02_B").gameObject.SetActive(false);
			gameObject.transform.Find("StartLoading_BG02_F").gameObject.SetActive(false);
		}

		public void SetDescImage(DescImageType imageType)
		{
			if (!(_renderer == null) && _block != null)
			{
				switch (imageType)
				{
				case DescImageType.Loading:
					_block.SetTexture(_shaderMaintexID, Miscs.LoadResource<Texture>("GameEntry/Texture/Elevator_Display01"));
					_renderer.SetPropertyBlock(_block);
					break;
				case DescImageType.Identifying:
					_block.SetTexture(_shaderMaintexID, Miscs.LoadResource<Texture>("GameEntry/Texture/Elevator_Display02"));
					_renderer.SetPropertyBlock(_block);
					break;
				case DescImageType.Confirmed:
					_block.SetTexture(_shaderMaintexID, Miscs.LoadResource<Texture>("GameEntry/Texture/Elevator_Display03"));
					_renderer.SetPropertyBlock(_block);
					break;
				}
			}
		}

		private IEnumerator CheckFloorPhase1AnimOver(AnimationState floorState)
		{
			while (!(floorState == null) && !(floorState.normalizedTime >= 1f))
			{
				yield return null;
			}
			if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
			{
				MonoGameEntry gameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
				if (gameEntry != null)
				{
					gameEntry.OnElevatorFloorPhase1AnimOver();
				}
			}
		}

		private IEnumerator CheckFloorPhase2AnimOver(AnimationState floorState)
		{
			while (!(floorState == null) && !(floorState.normalizedTime >= 1f))
			{
				yield return null;
			}
			if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
			{
				MonoGameEntry gameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
				if (gameEntry != null)
				{
					gameEntry.OnElevatorFloorPhase2AnimOver();
				}
			}
		}

		private void StopCheckFloorPhase1AnimOver()
		{
			if (_checkFloorPhase1AnimOverCoroutine != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_checkFloorPhase1AnimOverCoroutine);
				_checkFloorPhase1AnimOverCoroutine = null;
			}
		}

		private void StopCheckFloorPhase2AnimOver()
		{
			if (_checkFloorPhase2AnimOverCoroutine != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_checkFloorPhase2AnimOverCoroutine);
				_checkFloorPhase2AnimOverCoroutine = null;
			}
		}
	}
}
