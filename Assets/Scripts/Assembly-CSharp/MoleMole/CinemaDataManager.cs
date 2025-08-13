using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class CinemaDataManager
	{
		private Dictionary<string, ICinema> _loadedCinemaData = new Dictionary<string, ICinema>();

		private CinemaDataManager()
		{
		}

		public void InitAtAwake()
		{
			_loadedCinemaData.Clear();
		}

		public void Preload(BaseMonoAvatar aMonoAvatar)
		{
			ConfigAvatar config = aMonoAvatar.config;
			if (config.CinemaPaths.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<AvatarCinemaType, string> cinemaPath in config.CinemaPaths)
			{
				string value = cinemaPath.Value;
				if (!_loadedCinemaData.ContainsKey(value))
				{
					GameObject original = Miscs.LoadResource<GameObject>(value);
					GameObject gameObject = Object.Instantiate(original);
					ICinema cinema = gameObject.GetComponent<MonoBehaviour>() as ICinema;
					cinema.GetCutscene().Optimize();
					_loadedCinemaData.Add(value, cinema);
				}
			}
		}

		public ICinema GetCinemaDataByAvatar(string avatar, AvatarCinemaType type)
		{
			ConfigAvatar avatarConfig = AvatarData.GetAvatarConfig(avatar);
			string key = avatarConfig.CinemaPaths[type];
			return _loadedCinemaData[key];
		}
	}
}
