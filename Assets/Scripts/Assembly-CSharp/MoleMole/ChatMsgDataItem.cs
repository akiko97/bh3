using System;
using proto;

namespace MoleMole
{
	public class ChatMsgDataItem
	{
		public enum Type
		{
			MSG = 0,
			EMPTY = 1,
			HISTORY_LINE = 2,
			SYSTEM = 3,
			LUCK_GECHA = 4
		}

		public const int SYSTEM_UID = -1;

		public const int DIVIDE_UID = -2;

		public const int EMPTY_UID = -3;

		public int uid;

		public string nickname;

		public string guildTitle;

		public DateTime time;

		public string msg;

		public Type type;

		public static ChatMsgDataItem EMPTY_MSG = CreateEmptyMsgDataItem();

		public static ChatMsgDataItem HISTORY_LINE_MSG = CreateHistoryLineMsgDataItem();

		public static ChatMsgDataItem TALK_TOO_FAST_MSG = CreateSystemFastTalkMsgDataItem();

		public ChatMsgDataItem()
		{
		}

		public ChatMsgDataItem(int uid, string nickname, DateTime time, string msg, Type type = Type.MSG)
		{
			Init(uid, nickname, time, msg);
		}

		public ChatMsgDataItem(ChatMsg chatMsg)
		{
			DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(chatMsg.time);
			string text = ((!string.IsNullOrEmpty(chatMsg.nickname)) ? chatMsg.nickname : ("ID. " + chatMsg.uid));
			Init((int)chatMsg.uid, text, dateTimeFromTimeStamp, chatMsg.msg);
		}

		public ChatMsgDataItem(SystemChatMsg sysChatMsg)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			if ((int)sysChatMsg.type == 1)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)sysChatMsg.item_id);
				Type type = Type.MSG;
				string text = LocalizationGeneralLogic.GetText("ChatMsg_GachaGetItem", string.Format("[{0}]", dummyStorageDataItem.GetDisplayTitle()));
				if (dummyStorageDataItem.rarity >= 4)
				{
					string text2 = string.Format("{0}[{1}]{2}", "<color=#9b59b6>", dummyStorageDataItem.GetDisplayTitle(), "</color>");
					string text3 = string.Format("{0}{1}{2}", "<color=#88c700ff>", LocalizationGeneralLogic.GetText("Chat_Content_Source_Egg"), "</color>");
					text = LocalizationGeneralLogic.GetText("ChatMsg_GachaGetItemFrom", text3, text2);
					type = Type.LUCK_GECHA;
				}
				DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(sysChatMsg.time);
				string text4 = ((!string.IsNullOrEmpty(sysChatMsg.nickname)) ? sysChatMsg.nickname : ("ID. " + sysChatMsg.uid));
				Init((int)sysChatMsg.uid, text4, dateTimeFromTimeStamp, text, type);
			}
		}

		public static ChatMsgDataItem CreateEmptyMsgDataItem()
		{
			ChatMsgDataItem chatMsgDataItem = new ChatMsgDataItem();
			chatMsgDataItem.uid = -3;
			chatMsgDataItem.type = Type.EMPTY;
			return chatMsgDataItem;
		}

		public static ChatMsgDataItem CreateHistoryLineMsgDataItem()
		{
			ChatMsgDataItem chatMsgDataItem = new ChatMsgDataItem();
			chatMsgDataItem.uid = -2;
			chatMsgDataItem.type = Type.HISTORY_LINE;
			return chatMsgDataItem;
		}

		public static ChatMsgDataItem CreateSystemFastTalkMsgDataItem()
		{
			ChatMsgDataItem chatMsgDataItem = new ChatMsgDataItem();
			chatMsgDataItem.uid = -1;
			chatMsgDataItem.type = Type.SYSTEM;
			chatMsgDataItem.msg = LocalizationGeneralLogic.GetText("Chat_Content_TalkTooFast");
			return chatMsgDataItem;
		}

		public bool isMsgDataItemBelongToType(Type dataType)
		{
			switch (dataType)
			{
			case Type.EMPTY:
				return type == dataType && uid == -3;
			case Type.HISTORY_LINE:
				return type == dataType && uid == -2;
			case Type.MSG:
				return type == dataType;
			case Type.SYSTEM:
				return type == dataType && uid == -1;
			case Type.LUCK_GECHA:
				return type == Type.LUCK_GECHA;
			default:
				return false;
			}
		}

		private void Init(int uid, string nickname, DateTime time, string msg, Type type = Type.MSG)
		{
			this.uid = uid;
			this.nickname = nickname;
			this.time = time;
			this.msg = msg;
			this.type = type;
		}
	}
}
