using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib.Hardware
{
    public interface ITemperature : IDisposable
    {
        bool IsRunning { get; set; }
        double Temperature { get; set; }
        void Init();
    }
}
