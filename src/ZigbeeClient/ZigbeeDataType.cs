using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet
{
	public enum ZigbeeDataType : byte
	{
		/// <summary>No data</summary>
		Null = 0x00,
		/// <summary>8-bit data</summary>
		GeneralData8Bit = 0x08,
		/// <summary>16-bit data</summary>
		GeneralData16Bit = 0x09,
		/// <summary>24-bit data</summary>
		GeneralData24Bit = 0x0a,
		/// <summary>32-bit data</summary>
		GeneralData32Bit = 0x0b,
		/// <summary>40-bit data</summary>
		GeneralData40Bit = 0x0c,
		/// <summary>48-bit data</summary>
		GeneralData48Bit = 0x0d,
		/// <summary>56-bit data</summary>
		GeneralData56Bit = 0x0e,
		/// <summary>64-bit data</summary>
		GeneralData64Bit = 0x0f,

		/// <summary>Boolean</summary>
		Boolean = 0x10,

		/// <summary>8-bit bitmap data type</summary>
		Bitmap8 = 0x18,
		/// <summary>16-bit bitmap data type</summary>
		Bitmap16 = 0x19,
		/// <summary>24-bit bitmap data type</summary>
		Bitmap24 = 0x1a,
		/// <summary>32-bit bitmap data type</summary>
		Bitmap32 = 0x1b,

		/// <summary>Unsigned 8-bit integer</summary>
		UInt8 = 0x20,
		/// <summary>Unsigned 16-bit integer</summary>
		UInt16 = 0x21,
		/// <summary>Unsigned 24-bit integer</summary>
		UInt24 = 0x22,
		/// <summary>Unsigned 32-bit integer</summary>
		UInt32 = 0x23,
		/// <summary>Unsigned 40-bit integer</summary>
		UInt40 = 0x24,
		/// <summary>Unsigned 48-bit integer</summary>
		UInt48 = 0x25,
		/// <summary>Unsigned 56-bit integer</summary>
		UInt56 = 0x26,
		/// <summary>Unsigned 64-bit integer</summary>
		UInt64 = 0x27,

		/// <summary>Signed 8-bit integer</summary>
		Int8 = 0x28,
		/// <summary>Signed 16-bit integer</summary>
		Int16 = 0x29,
		/// <summary>Signed 24-bit integer</summary>
		Int24 = 0x2a,
		/// <summary>Signed 32-bit integer</summary>
		Int32 = 0x2b,
		/// <summary>Signed 40-bit integer</summary>
		Int40 = 0x2c,
		/// <summary>Signed 48-bit integer</summary>
		Int48 = 0x2d,
		/// <summary>Signed 56-bit integer</summary>
		Int56 = 0x2e,
		/// <summary>Signed 64-bit integer</summary>
		Int64 = 0x2f,

		/// <summary>8-bit enumeration</summary>
		Enumeration8 = 0x30,
		/// <summary>16-bit enumeration</summary>
		Enumeration16 = 0x31,

		/// <summary>16-bit floating point</summary>
		Float16 = 0x38,
		/// <summary>32-bit floating point</summary>
		Float32 = 0x39,
		/// <summary>64-bit floating point</summary>
		Float64 = 0x3a,

		/// <summary>Octet string</summary>
		StringOctet = 0x41,
		/// <summary>Character string</summary>
		StringCharacter = 0x42,
		/// <summary>Long octet string</summary>
		StringLongOctet = 0x43,
		/// <summary>Long character string</summary>
		StringLongCharacter = 0x44,

		/// <summary>Array</summary>
		Array = 0x48,
		/// <summary>Structure</summary>
		Structure = 0x4c,

		/// <summary>Set collection</summary>
		Set = 0x50,
		/// <summary>Bag collection</summary>
		Bag = 0x51,

		/// <summary>Time of day</summary>
		TimeOfDay = 0xe0,
		/// <summary>Date</summary>
		Date = 0xe1,
		/// <summary>UTC Time</summary>
		UtcTime = 0xe2,
		/// <summary>Cluster ID</summary>
		ClusterID = 0xe8,
		/// <summary>Attribute ID</summary>
		AttributeID = 0xe9,
		/// <summary>BACnet OID</summary>
		BacNetOid = 0xea,
		/// <summary>IEEE address (U64) type</summary>
		IEEEAddress = 0xf0,
		/// <summary>Unknown data type</summary>
		INVALID = 0xff      // Invalid data type
	}
}
