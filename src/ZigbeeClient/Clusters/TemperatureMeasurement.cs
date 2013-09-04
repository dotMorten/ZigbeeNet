using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet.Clusters
{
	[ClusterID(ID = 0x0402)]
	public class TemperatureMeasurement
	{
		public TemperatureMeasurement(AttributesCollection coll)
		{
			MeasuredValue = float.NaN;
			MinMeasuredValue = float.NaN;
			MaxMeasuredValue = float.NaN;
			Tolerance = float.NaN;
			if (coll.ContainsKey(0x0000))
			{
				var attr = coll[0x0000];
				if (attr.DataType == ZigbeeDataType.Int16)
					MeasuredValue = ((short)attr.Data) / 100f;
			}
			if (coll.ContainsKey(0x0001))
			{
				var attr = coll[0x0001];
				if (attr.DataType == ZigbeeDataType.Int16)
					MinMeasuredValue = ((short)attr.Data) / 100f;
			}
			if (coll.ContainsKey(0x0002))
			{
				var attr = coll[0x0002];
				if (attr.DataType == ZigbeeDataType.Int16)
					MaxMeasuredValue = ((short)attr.Data) / 100f;
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
			return string.Format("Temperature: {0}C (+/- {1})", MeasuredValue, Tolerance);
		}
	}
}
