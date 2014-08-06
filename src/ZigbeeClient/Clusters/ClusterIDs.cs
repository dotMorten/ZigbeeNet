using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigbeeNet.Clusters
{
	public enum ClusterID : ushort
	{
		#region General Function
		Basic = 0x0000,
		PowerConfiguration = 0x0001,
		DeviceTemperatureConfiguration = 0x0002,
		Identify = 0x0003,
		Groups = 0x0004,
		Scenes = 0x0005,
		OnOff = 0x0006,
		OnOfSwitchConfiguration = 0x0007,
		LevelControl = 0x0008,
		Alarms = 0x0009,
 		Time = 0x000A,
		RSSILocation = 0x000B,
		AnalogInput = 0x000C,
		AnalogOutput = 0x000D,
		AnalogValue = 0x000E,
		BinaryInput = 0x000F,
		BinaryOutput = 0x0010,
		BinaryValue = 0x0011,
		MultiStateInput = 0x0012,
		MultiStateOutput = 0x0013,
		MultiStateValue = 0x0014,
		#endregion

		#region Closures
		ShadeConfiguration = 0x0100,
		DoorLock = 0x0101,
		#endregion

		#region HVAC
		PumpConfigurationAndControl = 0x0200,
		Thermostat = 0x0201,
		FanControl = 0x0202,
		DehumidificationControl = 0x0203,
		ThermostatUserInterfaceConfiguration = 0x0204,
		#endregion

		#region Lighting
		ColorControl = 0x0300,
		BallastConfiguration = 0x0301,
		#endregion

		#region Measurement and sensing
		IlluminanceMeasurement = 0x0400,
		IlluminanceLevelSensing = 0x0401,
		TemperatureMeasurement = 0x0402,
		PressureMeasurement = 0x0403,
		FlowMeasurement = 0x0404,
		RelativeHumidityMeasurement = 0x0405,
		OccupancySensing = 0x0406,
		#endregion

		#region Sec. & Safety (Intruder Alarm System)
		/// <summary>Intruder Alarm System - Window</summary>
		IAS_WD = 0x0500,
		/// <summary>Intruder Alarm System - Zone</summary>
		IAS_Zone = 0x0501,
		/// <summary>Intruder Alarm System - ???</summary>
		IAS_ACE = 0x0502,
		#endregion

		#region Smart Energy
		SE_Price = 0x0700,
		SE_DemandResponseAndLoadControl = 0x0701,
		SE_Metering = 0x0702,
		SE_Messaging = 0x0703,
		SE_SmartEnergyTunneling = 0x0704,
		SE_Prepayment = 0x0705,
		SE_KeyEstablishment = 0x0800,
		#endregion
	}
}
