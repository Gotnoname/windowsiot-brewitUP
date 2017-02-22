using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrewLib.Objects;
using BrewLib.Profile;
using BrewLib.PID;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BrewLib.Interfaces.Implementations
{
    public class TemperatureStep : BaseStep 
    {
        public TemperatureStep()
        {
            Type = StepType.Temperature;
        }

        public override async Task RunTaskAsync(CancellationToken cancelToken)
        {
            base.RunTaskAsync(cancelToken);          

            //Loop until we land on the desired temperature
            while (TemperatureController.Instance.Controller.Temperature < Temperature)
            {
                try
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        cancelToken.ThrowIfCancellationRequested();
                    }
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        ProgressPercent = (int)(TemperatureController.Instance.Controller.Temperature / Temperature) * 100;
                    });
                    await Task.Delay(1000, cancelToken);                    
                }
                catch (OperationCanceledException)
                {

                }
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Finished = true;
            });
        }        
    }
}

