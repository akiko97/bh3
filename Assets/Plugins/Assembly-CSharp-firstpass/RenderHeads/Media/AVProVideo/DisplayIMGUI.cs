using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	[ExecuteInEditMode]
	[AddComponentMenu("AVPro Video/Display IMGUI")]
	public class DisplayIMGUI : MonoBehaviour
	{
		public MediaPlayer _mediaPlayer;

		public bool _displayInEditor = true;

		public ScaleMode _scaleMode = ScaleMode.ScaleToFit;

		public Color _color = Color.white;

		public bool _alphaBlend;

		public bool _fullScreen = true;

		public int _depth;

		[Range(0f, 1f)]
		public float _x;

		[Range(0f, 1f)]
		public float _y;

		[Range(0f, 1f)]
		public float _width = 1f;

		[Range(0f, 1f)]
		public float _height = 1f;

		private static int _propAlphaPack;

		private static int _propVertScale;

		private Shader _shaderAlphaPacking;

		private Material _material;

		private void Awake()
		{
			if (_propAlphaPack == 0)
			{
				_propAlphaPack = Shader.PropertyToID("AlphaPack");
				_propVertScale = Shader.PropertyToID("_VertScale");
			}
		}

		private void Start()
		{
			base.useGUILayout = false;
			if (_shaderAlphaPacking == null)
			{
				_shaderAlphaPacking = Shader.Find("AVProVideo/IMGUI/Texture Transparent");
			}
		}

		private void Update()
		{
			if (!(_mediaPlayer != null))
			{
				return;
			}
			Shader shader = null;
			if (_material != null)
			{
				shader = _material.shader;
			}
			Shader shader2 = null;
			switch (_mediaPlayer.m_AlphaPacking)
			{
			case AlphaPacking.TopBottom:
			case AlphaPacking.LeftRight:
				shader2 = _shaderAlphaPacking;
				break;
			}
			if (shader != shader2)
			{
				if (_material != null)
				{
					Object.Destroy(_material);
					_material = null;
				}
				if (shader2 != null)
				{
					_material = new Material(shader2);
				}
			}
			if (_material != null && _material.HasProperty(_propAlphaPack))
			{
				Helper.SetupAlphaPackedMaterial(_material, _mediaPlayer.m_AlphaPacking);
			}
		}

		private void OnGUI()
		{
			if (_mediaPlayer == null)
			{
				return;
			}
			bool flag = false;
			Texture texture = null;
			if (_displayInEditor)
			{
				texture = Texture2D.whiteTexture;
			}
			if (_mediaPlayer.Info != null && !_mediaPlayer.Info.HasVideo())
			{
				texture = null;
			}
			if (_mediaPlayer.TextureProducer != null && _mediaPlayer.TextureProducer.GetTexture() != null)
			{
				texture = _mediaPlayer.TextureProducer.GetTexture();
				flag = _mediaPlayer.TextureProducer.RequiresVerticalFlip();
			}
			if (!(texture != null) || (_alphaBlend && !(_color.a > 0f)))
			{
				return;
			}
			GUI.depth = _depth;
			GUI.color = _color;
			Rect rect = GetRect();
			switch (_mediaPlayer.m_AlphaPacking)
			{
			case AlphaPacking.None:
				if (flag)
				{
					GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0f, rect.y + rect.height / 2f));
				}
				GUI.DrawTexture(rect, texture, _scaleMode, _alphaBlend);
				break;
			case AlphaPacking.TopBottom:
			case AlphaPacking.LeftRight:
				if (flag)
				{
					_material.SetFloat(_propVertScale, -1f);
				}
				else
				{
					_material.SetFloat(_propVertScale, 1f);
				}
				Helper.DrawTexture(rect, texture, _scaleMode, _mediaPlayer.m_AlphaPacking, _material);
				break;
			}
		}

		public Rect GetRect()
		{
			return (!_fullScreen) ? new Rect(_x * (float)(Screen.width - 1), _y * (float)(Screen.height - 1), _width * (float)Screen.width, _height * (float)Screen.height) : new Rect(0f, 0f, Screen.width, Screen.height);
		}
	}
}
