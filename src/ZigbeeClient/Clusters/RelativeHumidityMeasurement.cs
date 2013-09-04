using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet.Clusters
{
	[ClusterID(ID = 0x0405)]
	public class RelativeHumidityMeasurement
	{
		public RelativeHumidityMeasurement(AttributesCollection coll)
		{
			MeasuredValue = float.NaN;
			MinMeasuredValue = float.NaN;
			MaxMeasuredValue = float.NaN;
			Tolerance = float.NaN;
			if (coll.ContainsKey(0x0000))
			{
				var attr = coll[0x0000];
				if (attr.DataType == ZigbeeDataType.UInt16)
					MeasuredValue = ((ushort)attr.Data) / 100f;
			}
			if (coll.ContainsKey(0x0001))
			{
				var attr = coll[0x0001];
				if (attr.DataType == ZigbeeDataType.UInt16)
					MinMeasuredValue = ((ushort)attr.Data) / 100f;
			}
			if (coll.ContainsKey(0x0002))
			{
				var attr = coll[0x0002];
				if (attr.DataType == ZigbeeDataType.UInt16)
					MaxMeasuredValue = ((ushort)attr.Data) / 100f;
			}
			if (coll.ContainsKey(0x0003))
			{
				var attr = coll[0x0003];
				if (attr.DataType == ZigbeeDataType.UInt16)
					Tolerance = ((ushort)attr.Data) / 100f;
			}
		}
		public float MeasuredValue { get; private set; }
		public float MinMeasuredValue { get; private set; }
		public float MaxMeasuredValue { get; private set; }
		public float Tolerance { get; private set; }
		public override string ToString()
		{
			return string.Format("Humidity: {0}% (+/- {1})", MeasuredValue, Tolerance);
		}
	}
}
