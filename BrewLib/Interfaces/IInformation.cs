using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib.Interfaces
{
    public interface IInformation
    {
        double Temperature { get; set; }
        int ElapsedMinutes { get; set; }
        string IpAddress { get; set; }
        double AverageTemperature { get; set; }
        double EnergyUsed { get; set; }
        int StepProgress { get; set; }
        string TimeElapsed { get; }

        void Reset();
    }
}
