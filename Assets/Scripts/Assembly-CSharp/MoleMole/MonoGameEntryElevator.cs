using UnityEngine;

namespace MoleMole
{
	public class MonoGameEntryElevator : MonoBehaviour
	{
		private const int _playBackgroundAnimLoopCount = 500;

		private int _playBackgroundAnimCount;

		public bool EnablePlayBackgroundAnim { get; set; }

		private void Awake()
		{
			EnablePlayBackgroundAnim = false;
		}

		private void Start()
		{
			Camera main = Camera.main;
			PostFX component = main.GetComponent<PostFX>();
			if (component != null)
			{
				component.WriteDepthTexture = true;
			}
		}

		[AnimationCallback]
		public void OnFloorAnimEvent(int phase)
		{
			MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
			monoGameEntry.OnElevatorFloorAnimEvent(phase);
		}

		[AnimationCallback]
		public void OnDoorAnimOver()
		{
			MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
			monoGameEntry.OnElevatorDoorAnimOver();
		}

		private void FixedUpdate()
		{
			if (EnablePlayBackgroundAnim)
			{
				if (_playBackgroundAnimCount == 0 || _playBackgroundAnimCount % 500 == 0)
				{
					PlayBackgroundAnimation();
					_playBackgroundAnimCount = 0;
				}
				_playBackgroundAnimCount++;
			}
		}

		private void PlayBackgroundAnimation()
		{
			Animation component = base.transform.GetComponent<Animation>();
			component.Blend("Background");
		}
	}
}
