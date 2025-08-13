using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DialogMetaData : IHashable
	{
		public class PlotChatNode
		{
			public readonly string chatContent;

			public readonly float chatDuration;

			public PlotChatNode(string chatContent, float chatDuration)
			{
				this.chatContent = chatContent;
				this.chatDuration = chatDuration;
			}

			public PlotChatNode(string nodeString)
			{
				if (nodeString.Contains(":"))
				{
					char[] seperator = new char[1] { ':' };
					List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
					chatContent = stringListFromString[0].Trim();
					chatDuration = float.Parse(stringListFromString[1]);
				}
				else
				{
					chatContent = nodeString;
					chatDuration = MiscData.Config.BasicConfig.DefaultChatDuration;
				}
			}
		}

		public readonly int dialogID;

		public readonly int avatarID;

		public readonly int screenSide;

		public readonly string source;

		public readonly List<PlotChatNode> content;

		public readonly string audio;

		public DialogMetaData(int dialogID, int avatarID, int screenSide, string source, List<PlotChatNode> content, string audio)
		{
			this.dialogID = dialogID;
			this.avatarID = avatarID;
			this.screenSide = screenSide;
			this.source = source;
			this.content = content;
			this.audio = audio;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(dialogID, ref lastHash);
			HashUtils.ContentHashOnto(avatarID, ref lastHash);
			HashUtils.ContentHashOnto(screenSide, ref lastHash);
			HashUtils.ContentHashOnto(source, ref lastHash);
			if (content != null)
			{
				foreach (PlotChatNode item in content)
				{
					HashUtils.ContentHashOnto(item.chatContent, ref lastHash);
					HashUtils.ContentHashOnto(item.chatDuration, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(audio, ref lastHash);
		}
	}
}
