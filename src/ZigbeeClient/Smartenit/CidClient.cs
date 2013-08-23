using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZigbeeNet.Smartenit
{
    public class CidClient : ZigbeeClient, IDisposable
    {
		private System.IO.Stream m_baseStream;
		private List<byte> m_buffer = new List<byte>();

		public CidClient(System.IO.Stream baseStream)
		{
			m_baseStream = baseStream;
		}
		public void OnDataRecieved(byte[] buffer)
		{
			m_buffer.AddRange(buffer);
			ProcessBuffer();
		}

		private void ProcessBuffer()
		{
			uint bytesRead = 0;
			var item = ResponseItem.Parse(m_buffer.ToArray(), out bytesRead);
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

		public event EventHandler<ResponseItem> ResponseReceived;

		public void Dispose()
		{
			m_baseStream.Dispose();
		}

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
