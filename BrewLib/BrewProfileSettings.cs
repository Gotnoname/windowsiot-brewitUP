using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BrewLib
{
    public class BrewProfileSettings : INotifyPropertyChanged
    {
        #region Singleton
        private static BrewProfileSettings _instance = null;
        public static BrewProfileSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BrewProfileSettings();
                return _instance;
            }
        }
        #endregion

        #region Properties
        public int SSRHeaterGPIO
        {
            get
            {
                return sSRHeaterGPIO;
            }

            set
            {
                sSRHeaterGPIO = value;
                OnPropertyChanged("SSRHeaterGPIO");
            }
        }

        public int SSRPumpGPIO
        {
            get
            {
                return sSRPumpGPIO;
            }

            set
            {
                sSRPumpGPIO = value;
                OnPropertyChanged("SSRPumpGPIO");
            }
        }

        public double PIDKp
        {
            get
            {
                return pIDKp;
            }

            set
            {
                pIDKp = value;
                OnPropertyChanged("PIDKp");
            }
        }

        public double PIDKi
        {
            get
            {
                return pIDKi;
            }

            set
            {
                pIDKi = value;
                OnPropertyChanged("PIDKi");
            }
        }

        public double PIDKd
        {
            get
            {
                return pIDKd;
            }

            set
            {
                pIDKd = value;
                OnPropertyChanged("PIDKd");
            }
        }

        public int PIDSetPoint
        {
            get
            {
                return pIDSetPoint;
            }

            set
            {
                pIDSetPoint = value;
                OnPropertyChanged("PIDSetPoint");
            }
        }

        public int PIDOutputLimitMin
        {
            get
            {
                return pIDOutputLimitMin;
            }

            set
            {
                pIDOutputLimitMin = value;
                OnPropertyChanged("PIDOutputLimitMin");
            }
        }

        public int PIDOutputLimitMax
        {
            get
            {
                return pIDOutputLimitMax;
            }

            set
            {
                pIDOutputLimitMax = value;
                OnPropertyChanged("PIDOutputLimitMax");
            }
        }

        public int MaximumBoilingTemperature
        {
            get
            {
                return maximumBoilingTemperature;
            }

            set
            {
                maximumBoilingTemperature = value;
                OnPropertyChanged("MaximumBoilingTemperature");
            }
        }

        public int MinimumBoilingTemperature
        {
            get
            {
                return minimumBoilingTemperature;
            }

            set
            {
                minimumBoilingTemperature = value;
                OnPropertyChanged("MinimumBoilingTemperature");
            }
        }

        public int BuzzerGPIO
        {
            get
            {
                return buzzerGPIO;
            }

            set
            {
                buzzerGPIO = value;
                OnPropertyChanged("BuzzerGPIO");
            }
        }

        public int DropSlot1
        {
            get { return dropSlot1; }
            set
            {
                dropSlot1 = value;
                OnPropertyChanged("DropSlot1");
            }
        }

        public int DropSlot2
        {
            get { return dropSlot2; }
            set
            {
                dropSlot2 = value;
                OnPropertyChanged("DropSlot2");
            }
        }

        public int DropSlot3
        {
            get { return dropSlot3; }
            set
            {
                dropSlot3 = value;
                OnPropertyChanged("DropSlot3");
            }
        }

        public int DropSlot4
        {
            get { return dropSlot4; }
            set
            {
                dropSlot4 = value;
                OnPropertyChanged("DropSlot4");
            }
        }

        public int DropSlot5
        {
            get { return dropSlot5; }
            set
            {
                dropSlot5 = value;
                OnPropertyChanged("DropSlot5");
            }
        }

        public int DropSlot6
        {
            get { return dropSlot6; }
            set
            {
                dropSlot6 = value;
                OnPropertyChanged("DropSlot6");
            }
        }

        public int HeaterElementWatt
        {
            get { return heaterElementWatt; }
            set { heaterElementWatt = value; OnPropertyChanged("HeaterElementWatt");}
        }

        public double TemperatureSanityValue
        {
            get { return temperatureSanityValue; }
            set { temperatureSanityValue = value; OnPropertyChanged("TemperatureSanityValue");}
        }

        public double TemperatureOvershoot
        {
            get { return temperatureOvershoot; }
            set { temperatureOvershoot = value; OnPropertyChanged("TemperatureOvershoot"); }
        }

        #endregion

        public const int CURRENT_VERSION = 0x02;

        //GPIO
        private int sSRHeaterGPIO = 18;
        private int sSRPumpGPIO = 23;
        private int buzzerGPIO = 24;

        //GPIO drop slots
        private int dropSlot1 = 5;
        private int dropSlot2 = 6;
        private int dropSlot3 = 13;
        private int dropSlot4 = 26;
        private int dropSlot5 = 12;
        private int dropSlot6 = 16;

        //PID
        private double pIDKp = 0.05;
        private double pIDKi = 0.008;
        private double pIDKd = 0.1;
        private int pIDSetPoint = 100;
        private int pIDOutputLimitMin = 0;
        private int pIDOutputLimitMax = 100;

        //Misc
        private int maximumBoilingTemperature = 100;
        private int minimumBoilingTemperature = 98;
        private int heaterElementWatt = 2500;
        private double temperatureSanityValue = 2.0;
        private double temperatureOvershoot = 2.0;

        private bool _hasLoaded = false;
        private string settingsFilePath;
        private const string SETTINGS_FILE_NAME = "brewSettings.dat";

        private StorageFolder settingsFolder;
        private StorageFile settingsFile;


        public BrewProfileSettings()
        {
            //settingsFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, SETTINGS_FILE_NAME);

            
        }

        public async Task Init()
        {
            try
            {
                StorageFolder vid = KnownFolders.VideosLibrary;

                // Get the first child folder, which represents the SD card.

                if (vid != null)
                {
                    // An SD card is present and the sdCard variable now contains a reference to it.
                    try
                    {

                        if ((settingsFolder = await vid.GetFolderAsync("BREWitUP")) == null)
                        {
                            settingsFolder = await vid.CreateFolderAsync("BREWitUP");
                        }

                        try
                        {
                            settingsFile = await settingsFolder.GetFileAsync(SETTINGS_FILE_NAME);
                        }
                        catch (FileNotFoundException)
                        {
                            settingsFile = await settingsFolder.CreateFileAsync(SETTINGS_FILE_NAME);
                        }
                       
                        settingsFilePath = settingsFile.Path;
                    }
                    catch (Exception e)
                    {
                        //UIMessager.Instance.ShowMessage("Could not create settings folder/file: " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public async Task<bool> Save()
        {
            bool result = true;
            await Task.Run(async () =>
            {
                try
                {
                    using (var writer = new BinaryWriter(new FileStream(settingsFilePath, FileMode.Create)))
                    {
                        writer.Write(CURRENT_VERSION);
                        writer.Write(SSRHeaterGPIO);
                        writer.Write(sSRPumpGPIO);
                        writer.Write(BuzzerGPIO);
                        writer.Write(DropSlot1);
                        writer.Write(DropSlot2);
                        writer.Write(DropSlot3);
                        writer.Write(DropSlot4);
                        writer.Write(DropSlot5);
                        writer.Write(DropSlot6);

                        writer.Write(PIDKp);
                        writer.Write(PIDKi);
                        writer.Write(PIDKd);
                        writer.Write(PIDSetPoint);
                        writer.Write(PIDOutputLimitMin);
                        writer.Write(PIDOutputLimitMax);

                        writer.Write(MaximumBoilingTemperature);
                        writer.Write(MinimumBoilingTemperature);
                        writer.Write(HeaterElementWatt);
                        writer.Write(TemperatureSanityValue);
                    }
                }
                catch (Exception e)
                {
                    //Could not save
                    result = false;
                    //await UIMessager.Instance.ShowMessageAndWaitForFeedback("Settings",
                    //    "Could not save settings: " + e.Message, UIMessageButtons.OK, UIMessageType.Error);
                }
            });
            return result;
        }

        public async Task Load()
        {
            await Task.Run(async () =>
            {
                if (!_hasLoaded)
                {
                    _hasLoaded = true;
                    if (settingsFile == null)
                        return;
                    bool result = true;
                    try
                    {
                        using (var reader = new BinaryReader(new FileStream(settingsFilePath, FileMode.Open)))
                        {
                            int version = reader.ReadInt32();
                            SSRHeaterGPIO = reader.ReadInt32();
                            SSRPumpGPIO = reader.ReadInt32();
                            BuzzerGPIO = reader.ReadInt32();
                            DropSlot1 = reader.ReadInt32();
                            DropSlot2 = reader.ReadInt32();
                            DropSlot3 = reader.ReadInt32();
                            DropSlot4 = reader.ReadInt32();
                            DropSlot5 = reader.ReadInt32();
                            DropSlot6 = reader.ReadInt32();

                            PIDKp = reader.ReadDouble();
                            PIDKi = reader.ReadDouble();
                            PIDKd = reader.ReadDouble();
                            PIDSetPoint = reader.ReadInt32();
                            PIDOutputLimitMin = reader.ReadInt32();
                            PIDOutputLimitMax = reader.ReadInt32();

                            MaximumBoilingTemperature = reader.ReadInt32();
                            MinimumBoilingTemperature = reader.ReadInt32();
                            HeaterElementWatt = reader.ReadInt32();

                            if (version >= 0x02)
                            {
                                TemperatureSanityValue = reader.ReadDouble();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //Could not load
                        //await UIMessager.Instance.ShowMessageAndWaitForFeedback("Settings",
                        //    "Could not load settings: " + e.Message, UIMessageButtons.OK, UIMessageType.Error);
                    }
                }
            });
        }

        public bool Verify()
        {
            var t = new List<int>
            {
                SSRHeaterGPIO,
                SSRPumpGPIO,
                BuzzerGPIO,
                DropSlot1,
                DropSlot2,
                DropSlot3,
                DropSlot4,
                DropSlot5,
                DropSlot6
            };

            var sb = new StringBuilder();
            foreach (var i in t)
            {
                if (!GPIOMapping.Instance.Verify(i))
                {
                    sb.AppendLine(string.Format("Pin {0} cannot be used.", i));
                }
            }

            //UIMessager.Instance.ShowMessage("Could not save settings!", sb.ToString(), UIMessageButtons.OK, UIMessageType.Error);
            return sb.Length == 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
