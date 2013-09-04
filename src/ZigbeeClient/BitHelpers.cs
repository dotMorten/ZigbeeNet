using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet
{
	internal static class BitHelpers
	{
		public static bool GetBit(byte b, int pos)
		{
			return ((b & (byte)(1 << pos)) != 0);
		}
		public static short ToInt16(byte[] data, int pos)
		{
			return BitConverter.ToInt16(new byte[] { data[pos + 1], data[pos] }, 0);
		}

		public static ushort ToUInt16(byte[] data, int pos)
		{
			return BitConverter.ToUInt16(new byte[] { data[pos + 1], data[pos] }, 0);
		}

		public static uint ToUInt32(byte[] data, int pos)
		{
			return BitConverter.ToUInt32(new byte[] { data[pos + 3], data[pos + 2], data[pos + 1], data[pos] }, 0);
		}

		public static ulong ToUInt64(byte[] data, int pos)
		{
			return BitConverter.ToUInt64(
				new byte[] { data[pos + 7], data[pos + 6], data[pos + 5], data[pos+4],
					data[pos+3], data[pos+2], data[pos+1], data[pos] }, 0);
		}

		public static byte[] GetBytes(UInt16 value)
		{
			var bytes = BitConverter.GetBytes(value);
			return new byte[] { bytes[1], bytes[0] };
		}

		public static byte[] GetBytes(UInt64 value)
		{
			var bytes = BitConverter.GetBytes(value);
			return new byte[] {
				bytes[7],bytes[6],bytes[5],bytes[4],bytes[3],bytes[2],bytes[1],bytes[0]
			};
		}
	}
}
