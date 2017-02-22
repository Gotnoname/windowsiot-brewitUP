using BrewLib;
using BrewLib.Databse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBrewPi
{
    public class BrewState : IDisposable
    {
        #region Singleton
        private static BrewState _instance;
        public static BrewState Instance => _instance ?? (_instance = new BrewState());
        #endregion

        #region Private variables
        private string _path;
        private const string CURRENT_STATE_NAME = "currentBrew.state";
        #endregion

        #region Properties
        public bool HasStateFile
        {
            get
            {
                return File.Exists(_path);
            }
        }
        #endregion

        public BrewState()
        {
            _path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, CURRENT_STATE_NAME);
        }

        public BrewProfile GetState()
        {
            return HasStateFile ? ProfileWriter.Read(File.ReadAllBytes(_path)) : null;
        }

        public void WriteState(BrewProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            var profileArray = ProfileWriter.Write(profile);
            if (profileArray != null)
            {
                File.WriteAllBytes(_path, profileArray);
            }
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_path))
                {
                    File.Delete(_path);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not delete state file: " + e);
            }
        }
    }
}
