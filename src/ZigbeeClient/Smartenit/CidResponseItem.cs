using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ZigbeeNet.Smartenit
{
	/// <summary>
	/// CID Response base class
	/// </summary>
	public abstract class CidResponseItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CidResponseItem"/> class.
		/// </summary>
		/// <param name="payload">The payload.</param>
		protected CidResponseItem(byte[] payload) { Payload = payload; }

		internal static CidResponseItem Parse(byte[] buffer, out uint bytesRead)
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
			if (CidResponseTypes == null)
				LoadResponseTypes();
			if (CidResponseTypes.ContainsKey(cmd))
			{
				var type = CidResponseTypes[cmd];
				return type.Invoke(new object[] { payload }) as CidResponseItem;
			}
			else
				return new UnknownResponse(cmd, payload);
		}
		private static Dictionary<ushort, ConstructorInfo> CidResponseTypes;

		private static void LoadResponseTypes()
		{
			CidResponseTypes = new Dictionary<ushort, ConstructorInfo>();
			var typeinfo = typeof(CidResponseItem).GetTypeInfo();
			foreach (var subclass in typeinfo.Assembly.DefinedTypes.Where(t => t.IsSubclassOf(typeof(CidResponseItem))))
			{
				var attr = subclass.GetCustomAttribute<ResponseCmd>();
				if (attr != null)
				{
					foreach (var c in subclass.DeclaredConstructors)
					{
						var pinfo = c.GetParameters();
						if(pinfo.Length == 1 && pinfo[0].ParameterType == typeof(byte[]))
						{
							CidResponseTypes.Add(attr.Command, c);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the payload data.
		/// </summary>
		/// <value>
		/// The payload.
		/// </value>
		public byte[] Payload { get; private set; }

		private static ushort SwapUInt16(ushort v)
		{
			return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
		}

		private static int XOR(byte[] data, uint start, uint count)
		{
			int b = data[start];
			for (int i = 1; i < count; i++)
			{
				b = b ^ data[start + i];
			}
			return b;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("PAYL={0}", string.Join(",", Payload));
		}
	}
}
