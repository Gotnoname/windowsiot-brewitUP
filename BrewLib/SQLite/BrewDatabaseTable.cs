using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace BrewLib.Databse
{
    public class BrewDatabaseTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ProfileId { get; set; }
        public byte[] Profile { get; set; }
    }
}
