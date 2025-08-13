using System.Collections.Generic;

namespace MoleMole
{
	public class DialogDataItem
	{
		public int dialogID;

		public int avatarID;

		public MonoStoryScreen.SelectScreenSide screenSide;

		public string source;

		public List<DialogMetaData.PlotChatNode> plotChatNodeList;

		public string audio;

		public DialogDataItem(int dialogID, int avatarID, int side, string source, List<DialogMetaData.PlotChatNode> plotChatNodeList, string audio)
		{
			this.dialogID = dialogID;
			this.avatarID = avatarID;
			switch (side)
			{
			case 0:
				screenSide = MonoStoryScreen.SelectScreenSide.Left;
				break;
			case 1:
				screenSide = MonoStoryScreen.SelectScreenSide.Right;
				break;
			default:
				screenSide = MonoStoryScreen.SelectScreenSide.None;
				break;
			}
			this.source = source;
			this.plotChatNodeList = plotChatNodeList;
			this.audio = audio;
		}

		public DialogDataItem(DialogMetaData dialogMetaData)
		{
			dialogID = dialogMetaData.dialogID;
			avatarID = dialogMetaData.avatarID;
			if (dialogMetaData.screenSide == 0)
			{
				screenSide = MonoStoryScreen.SelectScreenSide.Left;
			}
			else if (dialogMetaData.screenSide == 1)
			{
				screenSide = MonoStoryScreen.SelectScreenSide.Right;
			}
			else
			{
				screenSide = MonoStoryScreen.SelectScreenSide.None;
			}
			source = dialogMetaData.source;
			plotChatNodeList = dialogMetaData.content;
			audio = dialogMetaData.audio;
		}

		public override string ToString()
		{
			return string.Format("<DialogDataItem>\ndialogID: {0}\navatarID: {1}\nsource: {2}\n", dialogID, avatarID, source);
		}
	}
}
