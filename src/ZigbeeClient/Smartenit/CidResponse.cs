using System;
using System.IO;

namespace ZigbeeNet.Smartenit
{
	public abstract class ResponseItem
	{
		protected ResponseItem(byte[] payload) { Payload = payload; }

		public static ResponseItem Parse(byte[] buffer, out uint bytesRead)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			bytesRead = 0;
			System.IO.BinaryReader br = new System.IO.BinaryReader(new MemoryStream(buffer));
			var SOP = br.ReadByte();
			if (SOP != 0x02)
				throw new ArgumentException("Invalid SOP. Not a Zigbee response");
			if (buffer.Length < 4)
				return null;
			var cmd = SwapUInt16(br.ReadUInt16());
			var NACK = cmd & 32768;
			var ACK = cmd & 16384;
			var Response = cmd & 4096;
			var CommandNumber = 4095 & cmd;
			var LEN = br.ReadByte(); //pos=3
			if (buffer.Length < 5 + LEN)
				return null;
			var payload = br.ReadBytes(LEN);
			var fcs = br.ReadByte();
			bytesRead = 5u + LEN;

			var xor = XOR(buffer, 1u, bytesRead - 2u);
			if ((xor ^ fcs) != 0)
				throw new ArgumentException("Invalid message");
			switch (cmd)
			{
				case 4096: return new PingResponse(payload);
				case 4098: return new GetSystemTimeResponse(payload);
				case 4099: return new SetSystemTimeResponse(payload);
				default:
					return new UnknownResponse(cmd, payload);
			}
		}

		public byte[] Payload { get; set; }

		public static ushort SwapUInt16(ushort v)
		{
			return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
		}

		public static int XOR(byte[] data, uint start, uint count)
		{
			int b = data[start];
			for (int i = 1; i < count; i++)
			{
				b = b ^ data[start + i];
			}
			return b;
		}
		public override string ToString()
		{
			return string.Format("PAYL={0}", string.Join(",", Payload));
		}
	}

	public class PingResponse : ResponseItem
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

	public class GetSystemTimeResponse : ResponseItem
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

	public class SetSystemTimeResponse : ResponseItem
	{
		internal SetSystemTimeResponse(byte[] payload)
			: base(payload)
		{
			Success = payload[0] == 0;
		}
		public bool Success { get; private set; }
		public override string ToString()
		{
			return Success ? "SUCCESS" : "FAILED";
		}
	}

	public class UnknownResponse : ResponseItem
	{
		internal UnknownResponse(ushort cmd, byte[] payload)
			: base(payload)
		{
			CMD = cmd;
		}
		public ushort CMD { get; private set; }
		public override string ToString()
		{
			return string.Format("CMD={0},PAYL={1}", CMD, string.Join(",", Payload));
		}
	}
}
