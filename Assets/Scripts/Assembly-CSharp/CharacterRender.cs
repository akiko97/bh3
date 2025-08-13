using UnityEngine;

[ExecuteInEditMode]
public class CharacterRender : MonoBehaviour
{
	public Texture[] textures;

	public float mipMapBias = -1f;

	private void Start()
	{
		SetMipMapBias(mipMapBias);
	}

	private void OnEnable()
	{
		SetMipMapBias(mipMapBias);
	}

	private void OnDisable()
	{
		SetMipMapBias(0f);
	}

	private void SetMipMapBias(float bias)
	{
		Texture[] array = textures;
		foreach (Texture texture in array)
		{
			texture.mipMapBias = bias;
		}
	}
}
