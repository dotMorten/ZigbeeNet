using System;
using System.Collections.Generic;
using System.Linq;
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
			var item = CidResponseItem.Parse(m_buffer.ToArray(), out bytesRead);
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

		/// <summary>
		/// Raised when a message is received
		/// </summary>
		public event EventHandler<CidResponseItem> ResponseReceived;

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
