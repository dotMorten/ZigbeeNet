ZigbeeNet
=========

Portable Class Library for Zigbee. Supports .NET, Windows Store and Windows Phone.

This project is in its very early stages. Only supports Smartenit CID API, and very limited set of commands.


.NET Example code:

		private void Start()
		{
			SerialPort port = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
			port.DataReceived += port_DataReceived;
			port.Open();

			client = new ZigbeeNet.Smartenit.CidClient(port.BaseStream);
			client.ResponseReceived += client_ResponseReceived;
			client.SendPacket(CidPackets.SystemPing);
			client.SendPacket(CidPackets.SystemGetTime);
			client.SendPacket(CidPackets.SystemSetTime(DateTime.Now));
			client.SendPacket(CidPackets.SystemGetTime);
		}

		private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort port = sender as SerialPort;
			int count = port.BytesToRead;
			while (count > 0)
			{
				byte[] bytes = new byte[count];
				int readBytes = port.Read(bytes, 0, count);
				client.OnDataRecieved(bytes);
				count = port.BytesToRead;
			}
		}

		private void client_ResponseReceived(object sender, ZigbeeNet.Smartenit.CidResponseItem e)
		{
		  Console.WriteLine(
					string.Format("{0}: {1}\n", e.GetType().Name, e.ToString());
			);
		}
