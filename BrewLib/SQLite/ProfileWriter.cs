using BrewLib.Interfaces;
using BrewLib.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib.Databse
{
    public class ProfileWriter
    {
        public const int CURRENT_VERSION = 0x01;

        public static byte[] Write(BrewProfile profile)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(ms))
                    {
                        string json = JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                        });
                        writer.Write(json);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static BrewProfile Read(byte[] p)
        {
            var profile = new BrewProfile();

            try
            {
                using (var ms = new MemoryStream(p))
                {
                    using (var reader = new BinaryReader(ms))
                    {
                        var json = reader.ReadString();
                        return JsonConvert.DeserializeObject<BrewProfile>(json, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects
                        });
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
