using BrewLib.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BrewLib.Interfaces.Implementations
{
    public class BoilStep : BaseStep
    { 
        public BoilStep()
        {
            Type = StepType.Boil;
        }

        public override async Task RunTaskAsync(CancellationToken cancelToken)
        {
            try
            {
                //First lets check if the temperature is what it should be
                //according to the boiling step
                IStep temperature = new TemperatureStep() { Temperature = Temperature };
                await temperature.RunTaskAsync(cancelToken);
                
                base.RunTaskAsync(cancelToken);

                while (LengthMinutes > ElapsedSeconds / 60)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        cancelToken.ThrowIfCancellationRequested();
                    }

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        ProgressPercent = (int)((ElapsedSeconds * 60) / LengthMinutes) * 100;
                    });
                    
                    await Task.Delay(1000, cancelToken);
                }
            }
            catch (OperationCanceledException)
            {

            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Finished = true;
            });
        }        
    }
}
