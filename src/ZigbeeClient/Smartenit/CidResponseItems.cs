using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZigbeeNet.Smartenit
{
	/// <summary>
	/// CID System Ping response
	/// </summary>
	[ResponseCmd(Command = 0x1000)]
	public class PingResponse : ZigbeeCommand
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
	[ResponseCmd(Command = 0x1002)]
	public class GetSystemTimeResponse : ZigbeeCommand
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
	/// Set System Time Response
	/// </summary>
	[ResponseCmd(Command = 0x1003)]
	public class SetSystemTimeResponse : SucceededFailedResponse
	{
		internal SetSystemTimeResponse(byte[] payload) : base(payload) { }
	}
	/// <summary>
	/// System Start Network Response
	/// </summary>
	[ResponseCmd(Command = 0x1005)]
	public class SystemStartNetworkResponse : ZigbeeCommand
	{
		internal SystemStartNetworkResponse(byte[] payload) : base(payload) {
			if (payload.Length != 11)
				throw new ArgumentOutOfRangeException();
		}

		public byte Channel { get { return Payload[0]; } }

		public ushort PanID
		{
			get
			{
				return BitConverter.ToUInt16(new byte[] { Payload[2], Payload[1] }, 0);
			}
		}
		public ulong ExtendedPanID
		{
			get
			{
				return BitConverter.ToUInt64(
					new byte[] { Payload[10], Payload[9], Payload[8], Payload[7], Payload[6], Payload[5], Payload[4], Payload[3] }, 0);
			}
		}

		public override string ToString()
		{
			return string.Format("Pan ID: {0}, Extended Pan ID: {1:x}, Channel: {2}", PanID, ExtendedPanID, Channel);
		}
	}

	/// <summary>
	/// Set System Time Response
	/// </summary>
	[ResponseCmd(Command = 0x1010)]
	public class SystemModifyPermitJoinResponse : ZigbeeCommand
	{
		internal SystemModifyPermitJoinResponse(byte[] payload) : base(payload) { }

		public byte PermitTime { get { return Payload[0]; } }

		public override string ToString()
		{
			return string.Format("Permit time: {0}", PermitTime);
		}
	}
	/// <summary>
	/// Set System Time Response
	/// </summary>
	[ResponseCmd(Command = 0x101E)]
	public class ActiveNetworkTableResponse : SucceededFailedResponse
	{
		internal ActiveNetworkTableResponse(byte[] payload)
			: base(payload)
		{
		}

		/// <summary>
		/// The message’s source network address
		/// </summary>
		public ushort SourceAddress
		{
			get
			{
				return BitConverter.ToUInt16(new byte[] { Payload[2], Payload[1] }, 0);
			}
		}
		public byte NetworkTableTotalSize
		{
			get { return Payload[3]; }
		}
		public byte StartIndex
		{
			get { return Payload[4]; }
		}
		public IEnumerable<NetworkTableEntry> NetworkTable
		{
			get
			{
				var tableLength = Payload[5];
				var tablePayload = Payload.Skip(6);
				for (int i = 0; i < tableLength; i++)
				{
					var data = tablePayload.Skip(i * 22).Take(22);
					yield return new NetworkTableEntry(data.ToArray());
				}
			}
		}
		public static string ToBinaryString(byte b)
		{
			char[] str = new char[8];
			str[7] = (b & 1)   > 0 ? '1' : '0';
			str[6] = (b & 2)   > 0 ? '1' : '0';
			str[5] = (b & 4)   > 0 ? '1' : '0';
			str[4] = (b & 8)   > 0 ? '1' : '0';
			str[3] = (b & 16)  > 0 ? '1' : '0';
			str[2] = (b & 32)  > 0 ? '1' : '0';
			str[1] = (b & 64)  > 0 ? '1' : '0';
			str[0] = (b & 128) > 0 ? '1' : '0';
			return new string(str);
		}
		public class NetworkTableEntry
		{
			internal NetworkTableEntry(byte[] Payload)
			{
				ExtendedPanID = BitConverter.ToUInt64(Payload.Take(8).Reverse().ToArray(), 0);
				IEEEAddress = BitConverter.ToUInt64(Payload.Skip(8).Take(8).Reverse().ToArray(), 0);
				ShortAddress = BitConverter.ToUInt16(Payload.Skip(16).Take(2).Reverse().ToArray(), 0);
				var flags2 = Payload.Skip(18).First();
				var flags1 = Payload.Skip(19).First();
				Flags = ToBinaryString(flags1) + " " + ToBinaryString(flags2);
				Type = (ZigBeeDeviceType)(flags1 & 3); //bits 0,1
				RxOnWhenIdle = (flags1 & 12) >> 3 == 1; //bits 2,3
				NeighborRelationShip = (RelationShip)((flags1 & 112) >> 5); //bits 4,5,6
				PermitJoining = (flags2 & 3) == 1; //bits 8,9
				Depth = Payload[20];
				LinkQuality = Payload[21];
			}

			public ulong ExtendedPanID { get; private set; }
			public ulong IEEEAddress { get; private set; }
			public ushort ShortAddress { get; private set; }
			public byte Depth { get; private set; }
			public byte LinkQuality { get; private set; }
			public ZigBeeDeviceType Type { get; private set; }
			public bool RxOnWhenIdle { get; private set; }
			public string Flags { get; private set; }
			public RelationShip NeighborRelationShip { get; private set; }
			public override string ToString()
			{
				return ZigbeeCommand.ObjectToString(this, new string[] { });
			}
			public bool PermitJoining { get; private set; }

			public enum ZigBeeDeviceType : byte
			{
				Coordinator = 0,
				Router = 1, 
				EndDevice = 2
			}
			public enum RelationShip : short 
			{
				NeighborIsParent = 0, 
				NeighborIsChild = 1, 
				NeighborIsSibling = 2, 
				NoneOfTheAbove = 3, 
				Unknown = 4
			}
		}
	}
	/// <summary>
	/// Set System Time Response
	/// </summary>
	[ResponseCmd(Command = 0x1016)]
	public class ActiveEndpointResponse : SucceededFailedResponse
	{
		internal ActiveEndpointResponse(byte[] payload)
			: base(payload)
		{
		}

		/// <summary>
		/// Network address of the destination queried
		/// </summary>
		public ushort Interest
		{
			get
			{
				return BitConverter.ToUInt16(new byte[] { Payload[2], Payload[1] }, 0);
			}
		}


		/// <summary>
		/// Endpoints IDs in the queried device 
		/// </summary>
		public byte[] Endpoints { get { return Payload.Skip(4).ToArray(); } }
	}
		/// <summary>
	/// Set System Time Response
	/// </summary>
	[ResponseCmd(Command = 0x1023)]
	public class BindTableResponse : SucceededFailedResponse
	{
		internal BindTableResponse(byte[] payload)
			: base(payload)
		{
		}

		/// <summary>
		/// The message’s source network address
		/// </summary>
		public ushort SourceAddress
		{
			get
			{
				return BitConverter.ToUInt16(new byte[] { Payload[2], Payload[1] }, 0);
			}
		}
		public ushort TotalCount
		{
			get
			{
				return BitConverter.ToUInt16(new byte[] { Payload[4], Payload[2] }, 0);
			}
		}

		public ushort StartIndex
		{
			get
			{
				return BitConverter.ToUInt16(new byte[] { Payload[6], Payload[5] }, 0);
			}
		}

		public byte[] BindList
		{
			get
			{
				var count = BitConverter.ToUInt16(new byte[] { Payload[6], Payload[5] }, 0);
				var payload = Payload.Skip(7).ToArray();
				return payload;
			}
		}
	}
	/// <summary>
	/// An unknown response
	/// </summary>
	public class UnknownCidResponse : ZigbeeCommand
	{
		internal UnknownCidResponse(ushort cmd, byte[] payload) : base(payload)
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
			return string.Format("CMD=0x{0:X4},PAYL=[{1}]", CMD, string.Join(",", Payload.Select(t=>t.ToString("X2"))));// string.Format("{0:xx}",(int)t))));
		}
	}

}
