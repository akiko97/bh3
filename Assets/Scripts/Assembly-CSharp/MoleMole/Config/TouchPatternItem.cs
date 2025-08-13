using System;

namespace MoleMole.Config
{
	[Serializable]
	public class TouchPatternItem
	{
		public BodyPartType bodyPartType;

		public int heartLevel;

		public ReactionPattern reactionPattern;

		public int advanceTime;

		public ReactionPattern advanceReactionPattern;
	}
}
