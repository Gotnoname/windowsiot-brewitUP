using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GarageBrewLib.SSR;
using BrewLib.SSR;

namespace BrewLib.Profile
{
    public class BuzzerController : IDisposable
    {
        #region Private variables
        readonly SolidStateRelayHandler _buzzerHandler;
        #endregion

        #region Properties
        public bool IsBuzzing { get; set; }
        #endregion

        public BuzzerController()
        {
            if (!Utilities.IsDesktopComputer())
            {
                _buzzerHandler = new SolidStateRelayHandler(BrewProfileSettings.Instance.BuzzerGPIO);
            }
        }

        public void ActivateBuzzer(CancellationToken cancelToken)
        {
            if (IsBuzzing || _buzzerHandler == null)
            {
                return;
            }

            new Task(async () =>
            {
                IsBuzzing = true;
                _buzzerHandler.Switch(SSRState.On);
                await Task.Delay(5000, cancelToken);
                _buzzerHandler.Switch(SSRState.Off);
                IsBuzzing = false;
            }).Start();
        }

        public void Dispose()
        {            
            _buzzerHandler?.Dispose();
        }
    }
}
