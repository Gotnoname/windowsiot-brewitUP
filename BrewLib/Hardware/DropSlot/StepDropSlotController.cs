using BrewLib.Interfaces;
using BrewLib.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewLib.DropSlot
{
    public class StepDropSlotController
    {
        #region Singleton
        private static StepDropSlotController _instance;
        public static StepDropSlotController Instance => _instance ?? (_instance = new StepDropSlotController());
        #endregion

        #region Private variables
        private readonly StepDropSlot[] _dropSlots = new StepDropSlot[6];
        #endregion

        #region Ctor
        public StepDropSlotController()
        {
            _dropSlots[0] = new StepDropSlot(1, BrewProfileSettings.Instance.DropSlot1);
            _dropSlots[1] = new StepDropSlot(2, BrewProfileSettings.Instance.DropSlot2);
            _dropSlots[2] = new StepDropSlot(3, BrewProfileSettings.Instance.DropSlot3);
            _dropSlots[3] = new StepDropSlot(4, BrewProfileSettings.Instance.DropSlot4);
            _dropSlots[4] = new StepDropSlot(5, BrewProfileSettings.Instance.DropSlot5);
            _dropSlots[5] = new StepDropSlot(6, BrewProfileSettings.Instance.DropSlot6);
        } 
        #endregion

        public bool AddStepToSlot(IStep step, int slot)
        {
            foreach (var s in _dropSlots[slot].Steps)
            {
                //Cannot have several ingredients in the same slot
                //with different minute timings...
                //TODO: Alert user
                if (step.LengthMinutes != s.LengthMinutes)
                {
                    return false;
                }
            }

            _dropSlots[slot].Steps.Add(step);
            return true;
        }

        public void RemoveStepInSlot(IStep step, int slot)
        {
            foreach (var s in _dropSlots)
            {
                if (s.Steps.Contains(step))
                {
                    s.Steps.Remove(step);
                }
            }
        }

        public void DropStep(IStep step)
        {
            StepDropSlot slot = _dropSlots.FirstOrDefault(s => s.Steps.Contains(step));
            if (slot != null)
            {
                //Make sure that all appropriate hatches has been released before we 
                //open one that is higher up in the chain.
                switch (slot.SlotId)
                {
                    case 2:
                        if (!_dropSlots[0].HasDropped)
                        {
                            _dropSlots[0].Drop();
                        }
                        break;
                    case 3:
                        if (!_dropSlots[0].HasDropped)
                        {
                            _dropSlots[0].Drop();
                        }
                        if (!_dropSlots[1].HasDropped)
                        {
                            _dropSlots[1].Drop();
                        }
                        break;
                    case 5:
                        if (!_dropSlots[3].HasDropped)
                        {
                            _dropSlots[3].Drop();
                        }
                        break;
                    case 6:
                        if (!_dropSlots[3].HasDropped)
                        {
                            _dropSlots[3].Drop();
                        }
                        if (!_dropSlots[4].HasDropped)
                        {
                            _dropSlots[4].Drop();
                        }
                        break;
                }
                slot.Drop();
            }
        }
    }
}
