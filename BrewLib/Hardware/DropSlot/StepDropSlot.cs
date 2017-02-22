using BrewLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewLib.DropSlot
{
    public class StepDropSlot
    {
        protected GpioPin Pin { get; private set; }
        public int SlotId { get; private set; }
        public List<IStep> Steps { get; private set; }
        public bool HasDropped { get; set; }

        public StepDropSlot(int slot, int pin)
        {
            SlotId = slot;
            HasDropped = false;
            Steps = new List<IStep>();

            if (!Utilities.IsDesktopComputer())
            {
                try
                {
                    var gpio = GpioController.GetDefault();
                    Pin = gpio.OpenPin(pin);
                    Pin.Write(GpioPinValue.Low);
                    Pin.SetDriveMode(GpioPinDriveMode.Output);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Could not open dropslot pin: " + pin + ". " + e);
                }
            }
        }

        public void Drop()
        {
            if (!Utilities.IsDesktopComputer())
            {
                Pin.Write(GpioPinValue.High);
            }
            HasDropped = true;
        }

        public void AddStep(IStep step)
        {
            Steps.Add(step);
        }

        public void RemoveStep(IStep step)
        {
            Steps.Remove(step);
        }
    }
}
