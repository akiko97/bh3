using UnityEngine;

namespace MoleMole
{
	public class AnimateTiledTexture : MonoBehaviour
	{
		[Header("UV animation tile X")]
		public int _uvAnimationTileX = 4;

		[Header("UV animation tile Y")]
		public int _uvAnimationTileY = 16;

		[Header("Frames per second")]
		public float _framesPerSecond = 30f;

		[Header("Phase")]
		public int _phase;

		private void Update()
		{
			int num = (int)(Time.time * _framesPerSecond);
			num = (num + _phase) % (_uvAnimationTileX * _uvAnimationTileY);
			Vector2 scale = new Vector2(1f / (float)_uvAnimationTileX, 1f / (float)_uvAnimationTileY);
			int num2 = num % _uvAnimationTileX;
			int num3 = num / _uvAnimationTileX;
			Vector2 offset = new Vector2((float)num2 * scale.x, 1f - scale.y - (float)num3 * scale.y);
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
			GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
		}
	}
}
