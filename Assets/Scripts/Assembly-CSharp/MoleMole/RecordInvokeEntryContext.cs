using System.Collections.Generic;
using FlatBuffers;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public struct RecordInvokeEntryContext
	{
		public FlatBufferBuilder builder;

		public List<Offset<AbilityInvokeEntry>> outOffsetLs;

		public bool shouldRecord;

		public int offset;

		public AbilityInvokeArgument argType;

		public byte instancedAbilityID;

		public byte instanceModifierID;

		public uint targetID;

		public byte localID;

		public void Finish(bool record)
		{
			shouldRecord = record;
			if (shouldRecord)
			{
				WriteBuilder();
			}
		}

		public void Finish<T>(Offset<T> offset, AbilityInvokeArgument argType) where T : Table
		{
			this.offset = offset.Value;
			this.argType = argType;
			Finish(true);
		}

		public void Finish(AbilityInvokeArgument argType)
		{
			offset = -1;
			this.argType = argType;
			Finish(true);
		}

		private void WriteBuilder()
		{
			AbilityInvokeEntry.StartAbilityInvokeEntry(builder);
			AbilityInvokeEntry.AddInstancedAbilityID(builder, instancedAbilityID);
			AbilityInvokeEntry.AddInstancedModifierID(builder, instanceModifierID);
			AbilityInvokeEntry.AddTarget(builder, targetID);
			AbilityInvokeEntry.AddLocalID(builder, localID);
			if (argType != AbilityInvokeArgument.NONE)
			{
				AbilityInvokeEntry.AddArgumentType(builder, argType);
			}
			if (offset >= 0)
			{
				AbilityInvokeEntry.AddArgument(builder, offset);
			}
			Offset<AbilityInvokeEntry> item = AbilityInvokeEntry.EndAbilityInvokeEntry(builder);
			outOffsetLs.Add(item);
		}
	}
}
