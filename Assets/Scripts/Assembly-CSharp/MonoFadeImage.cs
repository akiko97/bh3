using UnityEngine;
using UnityEngine.UI;

public class MonoFadeImage : MonoBehaviour
{
	private Image blackFade;

	private float _fadeAlpha;

	private float _fadeSpeed;

	private bool _fading;

	private void Awake()
	{
		InitFade();
	}

	private void Update()
	{
		UpdateFade();
	}

	private void InitFade()
	{
		blackFade = GetComponent<Image>();
	}

	private void UpdateFade()
	{
		if (blackFade != null && _fading)
		{
			_fadeAlpha += _fadeSpeed;
			if (_fadeAlpha < 0f)
			{
				_fadeAlpha = 0f;
				_fading = false;
			}
			else if (_fadeAlpha > 1f)
			{
				_fadeAlpha = 1f;
				_fading = false;
			}
			blackFade.color = new Color(0f, 0f, 0f, _fadeAlpha);
		}
	}

	public void FadeOut(float speed = 1f)
	{
		_fading = true;
		_fadeSpeed = speed * Time.deltaTime;
	}

	public void FadeIn(float speed = 1f)
	{
		_fading = true;
		_fadeSpeed = (0f - speed) * Time.deltaTime;
	}
}
