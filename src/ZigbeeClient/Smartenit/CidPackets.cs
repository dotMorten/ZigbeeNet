using System;

namespace ZigbeeNet.Smartenit
{
	public static class CidPackets
	{
		/// <summary>
		/// PING device to verify if it is active and to check its capability
		/// </summary>
		public static CidPacket SystemPing
		{
			get { return new CidPacket(new byte[] { 0x00, 0x00 }, null); }
		}

		/// <summary>
		/// Gets current system time
		/// </summary>
		public static CidPacket SystemGetTime
		{
			get { return new CidPacket(new byte[] { 0x00, 0x02 }, null); }
		}

		private static DateTime Epoch = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Sets current system time
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static CidPacket SystemSetTime(DateTime time)
		{
			double seconds = (time.ToUniversalTime() - Epoch).TotalSeconds;
			byte[] secs = BitConverter.GetBytes((uint)seconds);
			return new CidPacket(new byte[] { 0x00, 0x03 }, new byte[] { secs[3], secs[2], secs[1], secs[0] });
		}
	}
}
