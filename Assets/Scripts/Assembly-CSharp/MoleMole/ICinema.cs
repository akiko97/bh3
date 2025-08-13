using CinemaDirector;
using UnityEngine;

namespace MoleMole
{
	public interface ICinema
	{
		void Init(Transform target);

		void Play();

		bool IsShouldStop();

		Cutscene GetCutscene();

		Transform GetCameraTransform();

		float GetInitCameraFOV();

		float GetInitCameraClipZNear();
	}
}
