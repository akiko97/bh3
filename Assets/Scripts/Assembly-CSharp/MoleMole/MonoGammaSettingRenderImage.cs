using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(RawImage))]
	public class MonoGammaSettingRenderImage : MonoBehaviour
	{
		private RenderTextureWrapper _renderTexture;

		public void SetupView()
		{
			DoSetupRenderTexture();
		}

		public void OnDestroy()
		{
			ReleaseRenderTexture();
		}

		public void ReleaseRenderTexture()
		{
			if (_renderTexture != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_renderTexture);
				_renderTexture = null;
				GameObject gameObject = GameObject.Find("MainCamera");
				if (gameObject != null)
				{
					gameObject.GetComponent<Camera>().targetTexture = null;
				}
			}
		}

		private void DoSetupRenderTexture()
		{
			if (_renderTexture != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_renderTexture);
				_renderTexture = null;
			}
			base.transform.GetComponent<RawImage>().enabled = true;
			float num = 1f;
			Canvas component = Singleton<MainUIManager>.Instance.SceneCanvas.GetComponent<Canvas>();
			if (component != null && component.renderMode != RenderMode.WorldSpace)
			{
				num = component.scaleFactor;
			}
			float width = ((RectTransform)base.transform).rect.width;
			float height = ((RectTransform)base.transform).rect.height;
			_renderTexture = GraphicsUtils.GetRenderTexture((int)(width * num), (int)(height * num), 24, RenderTextureFormat.ARGB32);
			_renderTexture.content.filterMode = FilterMode.Point;
			base.transform.GetComponent<RawImage>().texture = (RenderTexture)_renderTexture;
			GameObject gameObject = GameObject.Find("MainCamera");
			if (gameObject != null)
			{
				_renderTexture.BindToCamera(gameObject.GetComponent<Camera>());
			}
		}
	}
}
