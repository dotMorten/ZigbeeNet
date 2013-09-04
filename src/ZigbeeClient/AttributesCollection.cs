using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet
{
	public class AttributesCollection : Dictionary<ushort, AttributeRecord>
	{
		public AttributesCollection(IEnumerable<AttributeRecord> records)
		{
			foreach (var rec in records)
				this[rec.AttributeID] = rec;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var r in this)
				sb.AppendLine(r.Value.ToString());
			return sb.ToString();
		}
	}
}
