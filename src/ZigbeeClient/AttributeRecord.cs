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
				case ZigbeeDataType.Array:
					ushort len = BitHelpers.ToUInt16(data, 0);
					length += 2;
					Data = data.Skip(start + length).Take(len).ToArray();
					break;
				case ZigbeeDataType.UInt16:
				case ZigbeeDataType.Enumeration16:
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
