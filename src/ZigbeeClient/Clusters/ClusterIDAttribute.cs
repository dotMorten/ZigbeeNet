using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet.Clusters
{
	internal class ClusterIDAttribute : Attribute
	{
		public ushort ID { get; set; }
	}
}
