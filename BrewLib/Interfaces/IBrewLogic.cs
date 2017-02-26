using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib.Interfaces
{
    public interface IBrewLogic : IDisposable
    {
        bool IsRunning { get; }
        IStep CurrentStep { get; set; }
        IStep NextStep { get; set; }

        void Start();
        void Pause();
        void Stop();
        void Pump();
    }
}
