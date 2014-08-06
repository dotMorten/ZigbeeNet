using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace ZigbeeNet
{
	public abstract class ZigbeeCommand
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ZigbeeCommand"/> class.
		/// </summary>
		/// <param name="payload">The payload.</param>
		protected ZigbeeCommand(byte[] payload) { Payload = payload; }

		/// <summary>
		/// Gets the payload data.
		/// </summary>
		/// <value>
		/// The payload.
		/// </value>
		public byte[] Payload { get; private set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(string.Format("PAYL={0}", string.Join(",", Payload.Select(b=>b.ToString("X2")))));
			sb.Append("\n\t");
			sb.Append(ObjectToString(this, new string[] { "Payload" }).Replace("\n","\n\t"));
			return sb.ToString();
		}
		public static string ObjectToString(object o, string[] filter)
		{
			StringBuilder sb = new StringBuilder();
			var typeinfo = o.GetType().GetTypeInfo();
			var props = o.GetType().GetRuntimeProperties();
			foreach (var prop in props)
			{
				if(filter.Contains(prop.Name))
					continue;
				try
				{
					if (sb.Length > 0)
						sb.AppendFormat("\n");
					sb.AppendFormat("{0}: ", prop.Name);
					var val = prop.GetValue(o);
					if (val is UInt64) //display ulong as hex
						sb.AppendFormat("{0:X8}", val);
					else if(val is byte[])
					{
						sb.AppendFormat("[{0}]", string.Join(",", (byte[])val));
					}
					else if (val is Clusters.ClusterID[])
					{
						sb.AppendFormat("[{0}]", string.Join(",", (Clusters.ClusterID[])val));
					}
					else if (val is ushort[])
					{
						sb.AppendFormat("[{0}]", string.Join(",", (ushort[])val));
					}
					else if (val is IEnumerable && !(val is string)) // && !val.GetType().IsArray)
					{
						int i = 0;
						foreach (var item in (IEnumerable)val)
						{
							sb.AppendFormat("\n  [{0}]\t{1}",i++, item.ToString().Replace("\n", "\n\t"));
						}
					}
					else
						sb.AppendFormat("{0}", val);

				}
				catch (System.Exception ex)
				{
					sb.AppendFormat("{0}: ERROR: {1}", prop.Name, ex.Message);
				}
			}
			return sb.ToString();
		}
	}
}
