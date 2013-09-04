using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZigbeeNet.Smartenit
{
	/// <summary>
	///	Zigbee Client for Smartenit CID API devices
	/// </summary>
    public class CidClient : ZigbeeClient, IDisposable
    {
		private System.IO.Stream m_baseStream;
		private List<byte> m_buffer = new List<byte>();

		/// <summary>
		/// Initializes a new instance of the <see cref="CidClient"/> class.
		/// </summary>
		/// <param name="outputStream">The RS-232 output stream to write to.</param>
		public CidClient(System.IO.Stream outputStream)
		{
			m_baseStream = outputStream;
		}

		/// <summary>
		/// Call this when your device receives data
		/// </summary>
		/// <param name="buffer">The data received from the RS232 device</param>
		public void OnDataRecieved(byte[] buffer)
		{
			m_buffer.AddRange(buffer);
			ProcessBuffer();
		}

		private void ProcessBuffer()
		{
			uint bytesRead = 0;
			var item = Parse(m_buffer.ToArray(), out bytesRead);
			if (bytesRead > 0)
			{
				m_buffer.RemoveRange(0, (int)bytesRead);
			}
			if (item != null)
			{
				if (ResponseReceived != null)
					ResponseReceived(this, item);
			}
			if (bytesRead > 0 && m_buffer.Count > 0)
				ProcessBuffer();
		}

		internal static ZigbeeCommand Parse(byte[] buffer, out uint bytesRead)
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
				return type.Invoke(new object[] { payload }) as ZigbeeCommand;
			}
			else if (CidResponseCreators.ContainsKey(cmd))
			{
				var type = CidResponseCreators[cmd];
				return type.Invoke(null, new object[] { payload }) as ZigbeeCommand;
			}
			else
				return new UnknownCidResponse(cmd, payload);
		}

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

		private static Dictionary<ushort, ConstructorInfo> CidResponseTypes;
		private static Dictionary<ushort, MethodInfo> CidResponseCreators;

		private static void LoadResponseTypes()
		{
			CidResponseTypes = new Dictionary<ushort, ConstructorInfo>();
			CidResponseCreators = new Dictionary<ushort, MethodInfo>();
			var typeinfo = typeof(ZigbeeCommand).GetTypeInfo();
			foreach (var subclass in typeinfo.Assembly.DefinedTypes.Where(t => t.IsSubclassOf(typeof(ZigbeeCommand))))
			{
				var attr = subclass.GetCustomAttribute<ResponseCmd>(false);
				if (attr != null)
				{
					if (subclass.IsAbstract)
					{
						var method = subclass.GetDeclaredMethod("Create");
						if (method != null && method.IsStatic)
						{
							var pinfo = method.GetParameters();
							if (pinfo.Length == 1 && pinfo[0].ParameterType == typeof(byte[]))
							{
								CidResponseCreators.Add(attr.Command, method);
							}
						}
					}
					else
					{
						foreach (var c in subclass.DeclaredConstructors)
						{
							var pinfo = c.GetParameters();
							if (pinfo.Length == 1 && pinfo[0].ParameterType == typeof(byte[]))
							{
								CidResponseTypes.Add(attr.Command, c);
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Raised when a message is received
		/// </summary>
		public event EventHandler<ZigbeeCommand> ResponseReceived;

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing,
		/// or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			m_baseStream.Dispose();
		}

		/// <summary>
		/// Sends a CID packet to the device.
		/// </summary>
		/// <param name="packet">The packet to send.</param>
		public void SendPacket(CidPacket packet)
		{
			m_baseStream.Write(new byte[] { 0x02 }, 0, 1);
			m_baseStream.Write(packet.Command, 0, packet.Command.Length);
			int fcs = 0;
			for (int i = 0; i < packet.Command.Length; i++)
				fcs = fcs ^ packet.Command[i];
			byte len = 0;
			if (packet.Body != null && packet.Body.Length > 0)
			{
				len = (byte)packet.Body.Length;
				fcs = fcs ^ len;
				m_baseStream.Write(new byte[] { len }, 0, 1);
				m_baseStream.Write(packet.Body, 0, packet.Body.Length);
				for (int i = 0; i < packet.Body.Length; i++)
					fcs = fcs ^ packet.Body[i];
			}
			else
				m_baseStream.Write(new byte[] { 0x00 }, 0, 1);
			m_baseStream.Write(new byte[] { (byte)fcs }, 0, 1);
		}
	}
}
