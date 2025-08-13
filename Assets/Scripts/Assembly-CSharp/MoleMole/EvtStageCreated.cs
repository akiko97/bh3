using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class EvtStageCreated : BaseLevelEvent
	{
		public bool isBorn;

		public Vector3 offset;

		public List<string> avatarSpawnNameList;

		public EvtStageCreated(List<string> avatarSpawnNameList, bool isBorn, Vector3 offset)
		{
			this.avatarSpawnNameList = avatarSpawnNameList;
			this.isBorn = isBorn;
			this.offset = offset;
		}

		public override string ToString()
		{
			return "stage created";
		}
	}
}
