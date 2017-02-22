using BrewLib.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrewLib.Interfaces
{
    public interface IStep
    {
        int LengthMinutes { get; set; }
        int ElapsedMinutes { get; set; }
        int ElapsedSeconds { get; set; }
        int ProgressPercent { get; set; }

        double Temperature { get; set; }
        double Amount { get; set; }

        bool Finished { get; set; }
        bool HasStarted { get; }

        string Title { get; set; }

        StepType Type { get; set; }

        ObservableCollection<IStep> SubSteps { get; set; }

        Task RunTaskAsync(CancellationToken cancelToken);
        void StopTask();
        void WriteJson(JsonWriter writer);
    }
}
