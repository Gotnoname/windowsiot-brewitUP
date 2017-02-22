using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Newtonsoft.Json;
using SQLite.Net;


namespace BrewLib.Databse
{
    public class BrewDatabase
    {
        private const string DATABSE_NAME = "brewBase.sqlite";
        SQLiteConnection _conn;
        private string _path;
        private List<BrewDatabaseTable> _entries = new List<BrewDatabaseTable>();
        private StorageFolder _profileFolder;
        private StorageFile _profileFile;

        #region Singleton
        private static BrewDatabase _instance;
        public static BrewDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BrewDatabase();
                return _instance;
            }
        } 
        #endregion

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
                        if ((_profileFolder = await vid.GetFolderAsync("BREWitUP")) == null)
                        {
                            _profileFolder = await vid.CreateFolderAsync("BREWitUP");
                        }

                        //Copy database file from backup folder!
                        var file = await _profileFolder.GetFileAsync(DATABSE_NAME);
                        await file.CopyAsync(ApplicationData.Current.LocalFolder,
                            DATABSE_NAME, 
                            NameCollisionOption.ReplaceExisting);
                    }
                    catch (Exception e)
                    {

                    }
                }

                //OBS!: SQLite only accepts writing files in the app folder!!!!
                _path = Path.Combine(ApplicationData.Current.LocalFolder.Path, DATABSE_NAME);
                _conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), _path);
                _conn.CreateTable<BrewDatabaseTable>();
                _profileFile = await StorageFile.GetFileFromPathAsync(_path);
            }
            catch (Exception e)
            {

            }
        }

        public void AddProfile(BrewProfile profile)
        {
            var profileBytes = ProfileWriter.Write(profile);
            if(profileBytes == null)
            {
                return;
            }

            if (!ProfileExists(profile))
            {
                _conn.Insert(new BrewDatabaseTable
                {
                    ProfileId = profile.Id.ToString(),
                    Profile = profileBytes
                });
            }
            else
            {
                UpdateProfile(profile);
            }

            //backup of file
            _profileFile?.CopyAsync(_profileFolder,
                DATABSE_NAME,
                NameCollisionOption.ReplaceExisting);

        }        

        public void UpdateProfile(BrewProfile profile)
        {
            BrewDatabaseTable entry = _entries.FirstOrDefault(t => t.ProfileId == profile.Id.ToString());
            if (entry != null)
            {
                entry.Profile = ProfileWriter.Write(profile);
                _conn.Update(entry);
            }
        }

        public void DeleteProfile(BrewProfile profile)
        {
            BrewDatabaseTable entry = _entries.FirstOrDefault(t => t.ProfileId == profile.Id.ToString());
            if(entry != null)
                _conn.Delete(entry);
        }

        public List<BrewProfile> GetProfiles()
        {
            TableQuery<BrewDatabaseTable> query = _conn.Table<BrewDatabaseTable>();
            _entries = query.ToList();
            var profiles = new List<BrewProfile>();
            foreach(var entry in query)
            {
                var profile = ProfileWriter.Read(entry.Profile);
                if (profile != null)
                {
                    profiles.Add(profile);
                }
            }
            return profiles;
        }

        public bool ProfileExists(BrewProfile profile)
        {
            BrewDatabaseTable entry = _entries.FirstOrDefault(t => t.ProfileId == profile.Id.ToString());
            return entry != null;
        }

        private byte[] SerialieToJson(BrewProfile profile)
        {
            string json = JsonConvert.SerializeObject(profile);
            return Encoding.UTF8.GetBytes(json);
        }

        private BrewProfile DeserializeFromJson(byte[] profile)
        {
            return JsonConvert.DeserializeObject<BrewProfile>(Encoding.UTF8.GetString(profile), new DTOJsonConverter());
        }
    }
}
