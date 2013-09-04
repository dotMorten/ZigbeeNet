using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet
{
	public class AttributeRecord
	{
		internal AttributeRecord(byte[] data, int start, out int length)
		{
			AttributeID = BitHelpers.ToUInt16(data, start);
			DataType = (ZigbeeDataType)data[start + 2];
			length = 3;
			
			switch (DataType)
			{
				case ZigbeeDataType.UInt8:
				case ZigbeeDataType.GeneralData8Bit:
				case ZigbeeDataType.Enumeration8:
					Data = data[start + length++];
					break;
				case ZigbeeDataType.GeneralData16Bit:
					Data = data.Skip(start + length).Take(2).ToArray();
					length += 2;
					break;
				case ZigbeeDataType.GeneralData24Bit:
					Data = data.Skip(start + length).Take(3).ToArray();
					length += 3;
					break;
				case ZigbeeDataType.GeneralData32Bit:
					Data = data.Skip(start + length).Take(4).ToArray();
					length += 4;
					break;
				case ZigbeeDataType.GeneralData40Bit:
					Data = data.Skip(start + length).Take(5).ToArray();
					length += 5;
					break;
				case ZigbeeDataType.GeneralData48Bit:
					Data = data.Skip(start + length).Take(6).ToArray();
					length += 6;
					break;
				case ZigbeeDataType.GeneralData56Bit:
					Data = data.Skip(start + length).Take(7).ToArray();
					length += 7;
					break;
				case ZigbeeDataType.GeneralData64Bit:
					Data = data.Skip(start + length).Take(8).ToArray();
					length += 8;
					break;
				case ZigbeeDataType.Boolean:
					Data = data[start + length++] == 0x01; //TODO: 0xff means invalid
					break;

				case ZigbeeDataType.Array:
					ushort len = BitHelpers.ToUInt16(data, 0);
					length += 2;
					Data = data.Skip(start + length).Take(len).ToArray();
					break;
				case ZigbeeDataType.UInt16:
				case ZigbeeDataType.Enumeration16:
				case ZigbeeDataType.ClusterID:
				case ZigbeeDataType.AttributeID:
					Data = BitHelpers.ToUInt16(data, start + length);
					length += 2;
					break;
				case ZigbeeDataType.UInt32:
					Data = BitHelpers.ToUInt32(data, start + length);
					length += 4;
					break;
				case ZigbeeDataType.UInt64:
					Data = BitHelpers.ToUInt64(data, start + length);
					length += 4;
					break;
				case ZigbeeDataType.Int16:
					Data = BitHelpers.ToInt16(data, start + length);
					length += 2;
					break;
				case ZigbeeDataType.TimeOfDay:
					Data = new TimeSpan(data[start + length], data[start + length + 1], data[start + length + 2], data[start + length + 3] + 10);
					length += 4;
					break;
				case ZigbeeDataType.Date:
					Data = new DateTime(data[start + length] + 1900, data[start + length + 1], data[start + length + 2]);
					length += 4;
					break;
				case ZigbeeDataType.UtcTime:
					Data = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(BitHelpers.ToUInt32(data, start + length));
					length += 4;
					break;
				case ZigbeeDataType.Null:
					break;
				default:
					//TODO
					System.Diagnostics.Debug.WriteLine("Unknown data type: " + DataType.ToString());
					break;
			}
		}
		public override string ToString()
		{
			return string.Format("{0}: {1} ({2})", AttributeID, Data, DataType);
		}
		public ushort AttributeID { get; private set; }
		public bool Success { get; private set; }
		public ZigbeeDataType DataType { get; private set; }
		public object Data { get; private set; }
	}
}
