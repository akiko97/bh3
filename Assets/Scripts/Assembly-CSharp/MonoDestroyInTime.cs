using UnityEngine;

public class MonoDestroyInTime : MonoBehaviour
{
	public float time;

	private float _timer;

	private void Start()
	{
		_timer = time;
	}

	private void Update()
	{
		_timer -= Time.deltaTime;
		if (_timer <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
