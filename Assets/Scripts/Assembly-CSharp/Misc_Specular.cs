using UnityEngine;

[ExecuteInEditMode]
public class Misc_Specular : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		GetComponent<Renderer>().sharedMaterial.SetVector("centerPos", base.transform.position);
	}
}
