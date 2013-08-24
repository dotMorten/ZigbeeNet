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

		/// <summary>
		/// Starts the HA network as a coordinator
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static CidPacket SystemStartNetwork(ushort panID = 0, byte channelNo = 0)
		{
			byte[] pid = BitConverter.GetBytes(panID);
			return new CidPacket(new byte[] { 0x00, 0x05 }, new byte[] { pid[1], pid[0], channelNo });
		}

		/// <summary>
		/// Join network as a router
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static CidPacket SystemJoinNetwork(ushort panID = 0, byte channelNo = 0)
		{
			return SystemStartNetwork(panID, channelNo); //same command
		}


		/// <summary>
		/// Modify the Permit Join Time on a device
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		public static CidPacket ModifyPermitJoinRequest(ushort address, byte duration)
		{
			byte[] pid = BitConverter.GetBytes(address);
			return new CidPacket(new byte[] { 0x00, 0x10 }, new byte[] { 0x00, pid[1], pid[0], duration });
		}
		/// <summary>
		/// Modify the Permit Join Time on a device
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		public static CidPacket ModifyPermitJoinRequest(ulong address, byte duration)
		{
			byte[] pid = BitConverter.GetBytes(address);
			return new CidPacket(new byte[] { 0x00, 0x10 },
				new byte[] { 0x01, pid[7], pid[6], pid[5], pid[4], pid[3], pid[2], pid[1], pid[0], duration });
		}
	}
}
