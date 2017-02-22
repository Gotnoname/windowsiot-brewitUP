﻿using System;
using System.Threading.Tasks;

namespace Rinsen.IoT.OneWire
{
    public class DS18B20 : IOneWireDevice
    {
        public DS2482_100 DS2482_100 { get; set; }

        public byte[] OneWireAddress { get; set; }

        public string OneWireAddressString { get { return BitConverter.ToString(OneWireAddress); } }
        
        public void Initialize()
        {
        }

        public double GetTemperature()
        {
            byte[] scratchpad = GetTemperatureScratchpad();

            return GetTemp_Read(scratchpad[Scratchpad.TemperatureMSB], scratchpad[Scratchpad.TemperatureLSB]);
        }
        
        internal virtual double GetTemp_Read(byte msb, byte lsb)
        {
            double temp_read = 0;
            var negative = false;

            if (msb > 0xF8)
            {
                negative = true;
                msb = (byte)~msb;
                lsb = (byte)~lsb;
                var addOne = (ushort)lsb;
                addOne |= (ushort)(msb << 8);
                addOne++;
                lsb = (byte)(addOne & 0xFFu);
                msb = (byte)((addOne >> 8) & 0xFFu);
            }
            
            if (lsb.GetBit(0))
            {
                temp_read += Math.Pow(2, -4);
            }
            if (lsb.GetBit(1))
            {
                temp_read += Math.Pow(2, -3);
            }
            if (lsb.GetBit(2))
            {
                temp_read += Math.Pow(2, -2);
            }
            if (lsb.GetBit(3))
            {
                temp_read += Math.Pow(2, -1);
            }
            if (lsb.GetBit(4))
            {
                temp_read += Math.Pow(2, 0);
            }
            if (lsb.GetBit(5))
            {
                temp_read += Math.Pow(2, 1);
            }
            if (lsb.GetBit(6))
            {
                temp_read += Math.Pow(2, 2);
            }
            if (lsb.GetBit(7))
            {
                temp_read += Math.Pow(2, 3);
            }
            if (msb.GetBit(0))
            {
                temp_read += Math.Pow(2, 4);
            }
            if (msb.GetBit(1))
            {
                temp_read += Math.Pow(2, 5);
            }
            if (msb.GetBit(2))
            {
                temp_read += Math.Pow(2, 6);
            }

            if (negative)
            {
                temp_read = temp_read * -1;
            }

            return temp_read;
        }

        protected byte[] GetTemperatureScratchpad()
        {
            ResetOneWireAndMatchDeviceRomAddress();
            DS2482_100.EnableStrongPullup();
            StartTemperatureConversion();

            ResetOneWireAndMatchDeviceRomAddress();

            var scratchpad = ReadScratchpad();
            return scratchpad;
        }

        void StartTemperatureConversion()
        {
            DS2482_100.OneWireWriteByte(FunctionCommand.CONVERT_T);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        }

        byte[] ReadScratchpad()
        {
            DS2482_100.OneWireWriteByte(FunctionCommand.READ_SCRATCHPAD);

            var scratchpadData = new byte[9];

            for (int i = 0; i < scratchpadData.Length; i++)
            {
                scratchpadData[i] = DS2482_100.OneWireReadByte();
            }

            return scratchpadData;
        }

        void ResetOneWireAndMatchDeviceRomAddress()
        {
            DS2482_100.OneWireReset();

            DS2482_100.OneWireWriteByte(RomCommand.MATCH);

            foreach (var item in OneWireAddress)
            {
                DS2482_100.OneWireWriteByte(item);
            }
        }

        class Scratchpad
        {
            public const int TemperatureLSB = 0;

            public const int TemperatureMSB = 1;

            public const int ThRegisterOrUserByte1 = 2;

            public const int TlRegisterOrUserByte2 = 3;

            public const int ConfigurationRegister = 4;

            public const int Reserved = 5;

            public const int Reserved2 = 6;

            public const int Reserved3 = 7;

            public const int CRC = 8;

        }

        public class RomCommand
        {
            public const byte SEARCH = 0xF0;
            public const byte READ = 0x33;
            public const byte MATCH = 0x55;
            public const byte SKIP = 0xCC;
            public const byte ALARM_SEARCH = 0xEC;
        }

        public class FunctionCommand
        {
            public const byte CONVERT_T = 0x44;
            public const byte WRITE_SCRATCHPAD = 0x4E;
            public const byte READ_SCRATCHPAD = 0xBE;
            public const byte COPY_SCRATCHPAD = 0x48;
            public const byte RECALL_E = 0xB8;
            public const byte READ_POWER_SUPPLY = 0xB4;
        }
    }
}
