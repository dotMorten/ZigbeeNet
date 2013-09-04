using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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
			StringBuilder sb = new StringBuilder(string.Format("PAYL={0}", string.Join(",", Payload)));
			var typeinfo = this.GetType().GetTypeInfo();
			var props = this.GetType().GetRuntimeProperties();
			foreach (var prop in props)
			{
				if (prop.Name == "Payload") continue;
				var val = prop.GetValue(this);
				if (val is UInt64) //display ulong as hex
					val = string.Format("{0:X8}", val);
				sb.AppendFormat("\n\t{0}: {1}", prop.Name, val);
			}
			return sb.ToString();
		}
	}
}
