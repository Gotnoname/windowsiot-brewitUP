using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrewLib.Objects;
using System.Diagnostics;
using BrewLib.PID;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BrewLib.Interfaces.Implementations
{
    public class MashingStep : BaseStep
    {
        public MashingStep()
        {
            Type = StepType.Mash;
        }

        public override async Task RunTaskAsync(CancellationToken cancelToken)
        {
            try
            {
                //First lets check if the temperature is what it should be
                //according to the mashing step
                IStep temperature = new TemperatureStep() {Temperature = Temperature};
                await temperature.RunTaskAsync(cancelToken);

                if(!HasStarted)
                {
                    await UIMessager.Instance.ShowMessageAndWaitForFeedback($"{Type}",
                        "Requires user input to continue. Press \"OK\" to continue.",
                        UIMessageButtons.OK,
                        UIMessageType.Information);
                }
                
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
                        ProgressPercent = ((ElapsedSeconds * 60) / LengthMinutes) * 100;
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
