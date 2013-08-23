using System;

namespace ZigbeeNet.Smartenit
{
	public class CidPacket
	{
		public CidPacket(byte[] cmd, byte[] body)
		{
			if (cmd == null || cmd.Length != 2)
				throw new ArgumentException("cmd");
			Command = cmd;
			Body = body;
		}

		public byte[] Command { get; private set; }

		public byte[] Body { get; private set; }
	}
}
