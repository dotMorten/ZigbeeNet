using System;
using System.IO;

namespace ZigbeeNet.Smartenit
{
	/// <summary>
	/// CID System Ping response
	/// </summary>
	public class PingResponse : CidResponseItem
	{
		internal PingResponse(byte[] payload)
			: base(payload)
		{
			if (payload.Length != 15)
				throw new ArgumentException();
			byte u8MacFlags = payload[0];
			CoordinatorCapability = GetBit(u8MacFlags, 0);
			FFD = GetBit(u8MacFlags, 1);
			NodeIsMainsPowered = GetBit(u8MacFlags, 2);
			ReceiverIsEnabledDuringIdlePeriods = GetBit(u8MacFlags, 3);
			CapableOfHighSecurity = GetBit(u8MacFlags, 6);
			NetworkAddressShouldBeAllocatedToNode = GetBit(u8MacFlags, 7);
			byte u8Services = payload[1];
			PrimaryTrustCenter = GetBit(u8Services, 0);
			BackupTrustCenter = GetBit(u8Services, 1);
			PrimaryBindingTableCache = GetBit(u8Services, 2);
			BackupBindingTableCache = GetBit(u8Services, 3);
			PrimaryDiscoveryCache = GetBit(u8Services, 4);
			BackupDiscoveryCache = GetBit(u8Services, 5);
			NetworkManager = GetBit(u8Services, 6);
			NodeIsInRunningState = GetBit(u8Services, 7);
			NodeFirmwareVersion = payload[2];
			ZigbeeProfile = BitConverter.ToUInt16(new byte[] { payload[4], payload[3]}, 0);
			NodeNetworkAddress = BitConverter.ToUInt16(new byte[] { payload[6], payload[5] }, 0);
			NodeIEEEAddress = BitConverter.ToUInt64(new byte[] { payload[14], payload[13], payload[12], payload[11], payload[10], payload[9], payload[8], payload[7] }, 0);
		}

		public bool CoordinatorCapability { get; private set; }
		public bool FFD { get; private set; }
		public bool NodeIsMainsPowered { get; private set; }
		public bool ReceiverIsEnabledDuringIdlePeriods { get; private set; }
		public bool CapableOfHighSecurity { get; private set; }
		public bool NetworkAddressShouldBeAllocatedToNode { get; private set; }
		public bool PrimaryTrustCenter { get; private set; }
		public bool BackupTrustCenter { get; private set; }
		public bool PrimaryBindingTableCache { get; private set; }
		public bool BackupBindingTableCache { get; private set; }
		public bool PrimaryDiscoveryCache { get; private set; }
		public bool BackupDiscoveryCache { get; private set; }
		public bool NetworkManager { get; private set; }
		public bool NodeIsInRunningState { get; private set; }
		public byte NodeFirmwareVersion { get; private set; }
		public UInt16 ZigbeeProfile { get; private set; }
		public UInt16 NodeNetworkAddress { get; private set; }
		public UInt64 NodeIEEEAddress { get; private set; }

		private static bool GetBit(byte b, int pos)
		{
			return ((b & (byte)(1 << pos)) != 0);
		}
	}

	/// <summary>
	/// Get System Time Response
	/// </summary>
	public class GetSystemTimeResponse : CidResponseItem
	{
		private static DateTime Epoch = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		internal GetSystemTimeResponse(byte[] payload)
			: base(payload)
		{
			Time = Epoch.AddSeconds(BitConverter.ToUInt32(new byte[] { payload[3], payload[2], payload[1], payload[0] }, 0));
		}

		public DateTime Time { get; private set; }

		public override string ToString()
		{
			return Time.ToLocalTime().ToString();
		}
	}

	/// <summary>
	/// Succeeded or Failed response
	/// </summary>
	public class SucceededFailedResponse : CidResponseItem
	{
		internal SucceededFailedResponse(byte[] payload)
			: base(payload)
		{
			Success = payload[0] == 0;
		}

		/// <summary>
		/// Gets a value indicating whether command was a success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		public bool Success { get; private set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return Success ? "SUCCESS" : "FAILED";
		}
	}

	/// <summary>
	/// Set System Time Response
	/// </summary>
	public class SetSystemTimeResponse : SucceededFailedResponse
	{
		internal SetSystemTimeResponse(byte[] payload) : base(payload) { }
	}

	/// <summary>
	/// An unknown response
	/// </summary>
	public class UnknownResponse : CidResponseItem
	{
		internal UnknownResponse(ushort cmd, byte[] payload)
			: base(payload)
		{
			CMD = cmd;
		}
		/// <summary>
		/// Response Command
		/// </summary>
		public ushort CMD { get; private set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("CMD={0},PAYL={1}", CMD, string.Join(",", Payload));
		}
	}
}
