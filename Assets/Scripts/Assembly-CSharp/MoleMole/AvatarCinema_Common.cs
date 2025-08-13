using CinemaDirector;
using UnityEngine;

namespace MoleMole
{
	public class AvatarCinema_Common : MonoBehaviour, ICinema
	{
		[SerializeField]
		private Transform _anchor;

		[SerializeField]
		private Cutscene _cutScene;

		[SerializeField]
		private CharacterTrackGroup _characterTrackGroup;

		[SerializeField]
		private Transform _camera;

		[SerializeField]
		[Header("Init FOV, only positive value works")]
		private float _initFov = -1f;

		[SerializeField]
		[Header("Init Near Z Plane, only positive value works")]
		private float _initClipZNear = -1f;

		private MonoMainCamera _mainCamera;

		private bool _shouldStop;

		public bool IsShouldStop()
		{
			return _shouldStop;
		}

		private void Awake()
		{
			_camera.GetComponent<Camera>().enabled = false;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init(Transform actor)
		{
			_anchor.parent = actor;
			_anchor.localPosition = Vector3.zero;
			_anchor.localRotation = Quaternion.identity;
			_characterTrackGroup.Actor = actor;
			_cutScene.CutsceneFinished += CutsceneFinished;
			_mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
		}

		public void Play()
		{
			_mainCamera.TransitToCinema(this);
			_cutScene.Play();
		}

		public Cutscene GetCutscene()
		{
			return _cutScene;
		}

		public Transform GetCameraTransform()
		{
			if (_cutScene.State == Cutscene.CutsceneState.Playing)
			{
				return _camera;
			}
			return null;
		}

		private void CutsceneFinished(object sender, CutsceneEventArgs e)
		{
			_shouldStop = true;
			Cutscene cutscene = sender as Cutscene;
			cutscene.CutsceneFinished -= CutsceneFinished;
			uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
			Singleton<EventManager>.Instance.FireEvent(new EvtCinemaFinish(runtimeID, cutscene));
			_mainCamera.TransitToFollow();
		}

		private void ReceiveMessage(string messageID)
		{
			uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
			Singleton<EventManager>.Instance.FireEvent(new EvtCinemaReceiveMessage(runtimeID, _cutScene, messageID));
			if (messageID == "CloseToEnd")
			{
				_shouldStop = true;
				_cutScene.Pause();
			}
		}

		public float GetInitCameraFOV()
		{
			return _initFov;
		}

		public float GetInitCameraClipZNear()
		{
			return _initClipZNear;
		}
	}
}
