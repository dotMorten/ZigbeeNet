using System;
using System.Collections.Generic;
using System.Linq;

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
		/// <summary>
		/// Register IEEE Address and Link Key for a node
		/// </summary>
		/// <param name="address">The node IEEE address.</param>
		/// <param name="duration">The node link key (16 bytes).</param>
		/// <returns></returns>
		public static CidPacket RegisterNode(UInt64 nodeAddress, byte[] nodeLinkKey)
		{
			if (nodeLinkKey == null || nodeLinkKey.Length != 16)
				throw new ArgumentException("nodeLinkKey");
			throw new NotImplementedException();
			//byte[] pid = BitConverter.GetBytes(nodeAddress );
			//return new CidPacket(new byte[] { 0x00, 0x10 }, new byte[] { 0x00, pid[1], pid[0], duration });
		}
		/// <summary>
		/// Gets the destination's device node descriptor.
		/// </summary>
		/// <param name="address">Network address of the device generating the inquiry.</param>
		/// <param name="duration">Network address of the destination device being queries.</param>
		/// <returns></returns>
		public static CidPacket NodeDescriptorRequest(UInt16 destinationAddress, UInt16 deviceQueried)
		{
			byte[] d1 = BitConverter.GetBytes(destinationAddress );
			byte[] d2 = BitConverter.GetBytes(deviceQueried);
			return new CidPacket(new byte[] { 0x00, 0x14 }, new byte[] { d1[1], d1[0], d2[1], d2[0]});
		}
		/// <summary>
		/// Gets the destination's device simple descriptor information.
		/// </summary>
		/// <param name="address">Network address of the device generating the inquiry.</param>
		/// <param name="duration">Network address of the destination device being queries.</param>
		/// <returns></returns>
		public static CidPacket SimpleDescriptorRequest(UInt16 destinationAddress, UInt16 deviceQueried, byte endPoint)
		{
			byte[] d1 = BitConverter.GetBytes(destinationAddress);
			byte[] d2 = BitConverter.GetBytes(deviceQueried);
			return new CidPacket(new byte[] { 0x00, 0x15 }, new byte[] { d1[1], d1[0], d2[1], d2[0], endPoint });
		}

		/// <summary>
		/// Send Bind Request to a Node Hosting a Binding Table
		/// </summary>
		/// <param name="shortSrcAddress">Short address of destination node of request (client device to bind to). This
		/// may or may not be the node holding the binding table.</param>
		/// <param name="srcAddress">IEEE address of the source node for the binding (client device to bind to)</param>
		/// <param name="srcEndPoint">Binding source endpoint</param>
		/// <param name="clustID">Cluster ID to match</param>
		/// <param name="dstAdd">Destination Address</param>
		/// <param name="dstEndPoint">Destination endpoint</param>
		/// <returns>Bind Request packet</returns>
		public static CidPacket BindRequest(UInt16 shortSrcAddress, UInt64 srcAddress,
			byte srcEndPoint, UInt16 clustID, UInt64 dstAdd, byte dstEndPoint)
		{
			byte[] u8AddMode = new byte[] { 0x01 }; //16bit IEEE
			byte[] uAddress = BitHelpers.GetBytes(shortSrcAddress);
			byte[] u64SrcAddr = BitHelpers.GetBytes(srcAddress);
			byte[] u8SrcEPt = new byte[] { srcEndPoint };
			byte[] u16ClstrID = BitHelpers.GetBytes(clustID);
			byte[] u8DstMode = new byte[] { 0x03 }; //64bit IEEE
			byte[] u64DstAddr = BitHelpers.GetBytes(dstAdd);
			byte[] u8DstEPt = new byte[] { dstEndPoint };
			var data = Merge(new byte[][] { u8AddMode, uAddress, u64SrcAddr, 
				u8SrcEPt, u16ClstrID, u8DstMode, u64DstAddr, u8DstEPt });
			return new CidPacket(new byte[] { 0x00, 0x20 }, data.ToArray() );
		}

		public static CidPacket ReadAttributesRequest(
			DiscoverAttributesResponse resp)
		{
			byte[] u8Mode = new byte[] { resp.Payload[0] };
			byte[] manufacturerCode = BitHelpers.GetBytes(resp.ManufacturerCode);
			byte[] dstAdd = BitHelpers.GetBytes(resp.SourceAddress);
			byte[] dstEpt = new byte[] { resp.SourceEndpoint };
			byte[] clstrID = BitHelpers.GetBytes(resp.ClusterID);
			byte[] cmdId = new byte[] { 0 };
			byte[] attrbs = new byte[] { resp.AttributeCount };
			byte[] attrList = new byte[2 * resp.AttributeCount];
			for (int i = 0; i < resp.AttributeCount; i++)
			{
				byte[] id = new byte[] { 0x00, 0x00 }; // BitHelpers.GetBytes(resp.ClusterID);
				attrList[i * 2] = id[0];
				attrList[i * 2 + 1] = id[1];
			}
			IEnumerable<byte> data = null;
			if (resp.IsManufacturerSpecific)
			{
				data = Merge(new byte[][] { u8Mode, manufacturerCode, dstAdd, 
				dstEpt, clstrID, cmdId, attrbs, attrList });
			}
			else
			{
				data = Merge(new byte[][] { u8Mode, dstAdd, 
				dstEpt, clstrID, cmdId, attrbs, attrList });
			}
			return new CidPacket(new byte[] { 0x00, 0x30 }, data.ToArray());
		}

		private static IEnumerable<byte> Merge(IEnumerable<byte[]> arrays)
		{
			foreach (var array in arrays)
				foreach (var b in array)
					yield return b;
		}
	}
}
