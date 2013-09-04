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
	[ResponseCmd(Command = 0x1002)]
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
	/// Set System Time Response
	/// </summary>
	[ResponseCmd(Command = 0x1005)]
	public class SystemStartNetworkResponse : CidResponseItem
	{
		internal SystemStartNetworkResponse(byte[] payload) : base(payload) { }

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
	public class SystemModifyPermitJoinResponse : CidResponseItem
	{
		internal SystemModifyPermitJoinResponse(byte[] payload) : base(payload) { }

		public byte PermitTime { get { return Payload[0]; } }

		public override string ToString()
		{
			return string.Format("Permit time: {0}", PermitTime);
		}
	}
	/// <summary>
	/// End Device Announce Response
	/// </summary>
	[ResponseCmd(Command = 0x101B)]
	public class EndDeviceAnnounceResponse : CidResponseItem
	{
		internal EndDeviceAnnounceResponse(byte[] payload) : base(payload) { }

		public ushort DeviceAddress 
		{
			get { return BitHelpers.ToUInt16(Payload, 0); }
		}
		public ulong DeviceAddressIEEE
		{
			get { return BitHelpers.ToUInt64(Payload, 2); }
		}
		public bool CanBeCoordinator { get { return BitHelpers.GetBit(Payload[10], 0); } }
		public bool IsFullFunctioningDevice { get { return BitHelpers.GetBit(Payload[10], 1); } }
		public bool NodeIsMainsPowered { get { return BitHelpers.GetBit(Payload[10], 2); } }
		public bool RxEnabledDuringIdlePeriods { get { return BitHelpers.GetBit(Payload[10], 3); } }
		public bool HighSecurityEnabled { get { return BitHelpers.GetBit(Payload[10], 6); } }
		public bool NetworkAddressShouldBeAllocatedToNode { get { return BitHelpers.GetBit(Payload[10], 7); } }
	}

	/// <summary>
	/// Node Descriptor Response
	/// </summary>
	[ResponseCmd(Command = 0x1014)]
	public class NodeDescriptorResponse : SucceededFailedResponse
	{
		internal NodeDescriptorResponse(byte[] payload) : base(payload) { }

		public ushort SourceAddress
		{
			get
			{
				return BitHelpers.ToUInt16(Payload, 1);
			}
		}
		public byte MACCapabilityFlags { get { return Payload[5]; } }
	}

	/// <summary>
	/// Simple Descriptor Response
	/// </summary>
	[ResponseCmd(Command = 0x1015)]
	public class SimpleDescriptorResponse : SucceededFailedResponse
	{
		internal SimpleDescriptorResponse(byte[] payload) : base(payload) { }

		public ushort DestinationAddress
		{
			get { return BitHelpers.ToUInt16(Payload, 1); }
		}
		public byte AppEndpoint { get { return Payload[4]; } }
		public ushort EndpointProfileID
		{
			get { return BitHelpers.ToUInt16(Payload, 5); }
		}
		public ushort EndpointDeviceID
		{
			get { return BitHelpers.ToUInt16(Payload, 7); }
		}
		//TODO...
	}

	/// <summary>
	/// Device Joined Response
	/// </summary>
	[ResponseCmd(Command = 0x1011)]
	public class DeviceJoinedResponse : EndDeviceAnnounceResponse
	{
		internal DeviceJoinedResponse(byte[] payload) : base(payload) { }
	}

	/// <summary>
	/// Node Descriptor Response
	/// </summary>
	[ResponseCmd(Command = 0x1020)]
	public class BindResponse : CidResponseItem
	{
		internal BindResponse(byte[] payload) : base(payload) { }

		/// <summary>
		/// Status of Bind request
		/// </summary>
		public BindStatus Status { get { return (BindStatus)Payload[0]; } }

		public ushort SourceAddress
		{
			get
			{
				return BitHelpers.ToUInt16(Payload, 1);
			}
		}
	
		public enum BindStatus
		{
			Success = 0,
			NotSupported = 1,
			TableFull = 2
		}
	}

	[ResponseCmd(Command = 0x1031)]
	public abstract class AttributeBaseResponse : CidResponseItem
	{
		private class UnknownAttributeResponse : AttributeBaseResponse
		{
			public UnknownAttributeResponse(byte[] payload) : base(payload) { }
		}
		public static AttributeBaseResponse Create(byte[] payload)
		{
			bool isManufacturerSpecific = BitHelpers.GetBit(payload[0], 6);
			var commandID = isManufacturerSpecific ? payload[7] : payload[6];
			switch (commandID)
			{
				case 0x01: return new ReadAttributesResponse(payload);
				case 0x0A: return new ReportAttributesResponse(payload);
				case 0x0D: return new DiscoverAttributesResponse(payload);
				default: return new UnknownAttributeResponse(payload);
			}
		}
		internal AttributeBaseResponse(byte[] payload)
			: base(payload) 
		{
		}
		public bool IsManufacturerSpecific { get; private set; }

		public ushort ManufacturerCode
		{
			get
			{
				if (IsManufacturerSpecific)
					return BitHelpers.ToUInt16(Payload, 1);
				else
					return 0;
			}
		}
		public UInt16 SourceAddress
		{
			get { return BitHelpers.ToUInt16(Payload, IsManufacturerSpecific ? 2 : 1); }
		}
		public byte SourceEndpoint { get { return IsManufacturerSpecific ? Payload[4] : Payload[3]; } }
		public UInt16 ClusterID
		{
			get { return BitHelpers.ToUInt16(Payload, IsManufacturerSpecific ? 5 : 4); }
		}
		public byte CommandID { get { return IsManufacturerSpecific ? Payload[7] : Payload[6]; } }
		//CommandID:
		//0x0D = DiscoverAttributesResponse 
		//0x0B = Default
		//0x01 = ReadAttributesResponse
		//0x04 = WriteAttributesResponse
		//0x07 = ConfigureReportingResponse
		//0x09 = ReadReportingConfigureResponse
		//0x0A = ReadAttributesMessageResponse
	}
	//CMD == 0x0D
	public class DiscoverAttributesResponse : AttributeBaseResponse
	{
		public DiscoverAttributesResponse(byte[] payload) : base(payload) { }
		public byte AttributeCount { get { return IsManufacturerSpecific ? Payload[8] : Payload[7]; } }
		public bool IsListComplete { get { return (IsManufacturerSpecific ? Payload[9] : Payload[8]) == 0x01; } }
	}
	public class ReadAttributesResponse : AttributeBaseResponse
	{
		public ReadAttributesResponse(byte[] payload) : base(payload) { }
		public byte AttributeCount { get { return IsManufacturerSpecific ? Payload[8] : Payload[7]; } }
		public bool IsListComplete { get { return (IsManufacturerSpecific ? Payload[9] : Payload[8]) == 0x01; } }
	}
	public class ReportAttributesResponse : AttributeBaseResponse
	{
		public ReportAttributesResponse(byte[] payload)
			: base(payload)
		{
			int start = IsManufacturerSpecific ? 9 : 8;
			var attr = new List<AttributeRecord>(AttributeCount);
			for (int i = 0; i < AttributeCount; i++)
			{
				int length = 0;
				AttributeRecord rec = new AttributeRecord(payload, start, out length);
				attr.Add(rec);
				start += length;
			}
			Attributes = new AttributesCollection(attr);
		}
		public byte AttributeCount { get { return IsManufacturerSpecific ? Payload[8] : Payload[7]; } }
		public AttributesCollection Attributes { get; private set; }
	}
	public class AttributesCollection : IEnumerable<AttributeRecord>
	{
		private IEnumerable<AttributeRecord> m_records;
		public AttributesCollection(IEnumerable<AttributeRecord> records)
		{ m_records = records; }


		public IEnumerator<AttributeRecord> GetEnumerator()
		{
			foreach (var r in m_records)
				yield return r;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (var r in m_records)
				yield return m_records;
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var r in m_records)
				sb.AppendLine(r.ToString());
			return sb.ToString();
		}
	}

	/// <summary>
	/// An unknown response
	/// </summary>
	public class UnknownResponse : CidResponseItem
	{
		internal UnknownResponse(ushort cmd, byte[] payload) : base(payload)
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

	public class ResponseCmd : Attribute
	{
		public ushort Command { get; set; }
	}

}
