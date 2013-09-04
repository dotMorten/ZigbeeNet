using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZigbeeNet
{
	/// <summary>
	/// Succeeded or Failed response
	/// </summary>
	public class SucceededFailedResponse : ZigbeeCommand
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
	/// End Device Announce Response
	/// </summary>
	[ResponseCmd(Command = 0x101B)]
	public class EndDeviceAnnounceResponse : ZigbeeCommand
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
	public class BindResponse : ZigbeeCommand
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
	public abstract class AttributeBaseResponse : ZigbeeCommand
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

	public class ResponseCmd : Attribute
	{
		public ushort Command { get; set; }
	}

}
