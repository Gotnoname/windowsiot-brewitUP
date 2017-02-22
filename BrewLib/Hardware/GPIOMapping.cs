using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib
{
    //https://ms-iot.github.io/content/en-US/win10/samples/PinMappingsRPi2.htm

    public class GPIOMapping
    {
        #region Singleton
        private static GPIOMapping _instance;
        public static GPIOMapping Instance => _instance ?? (_instance = new GPIOMapping());
        #endregion

        #region Private variables
        private readonly Dictionary<int, bool> _gpioMap = new Dictionary<int, bool>(); 
        #endregion

        public GPIOMapping()
        {
            _gpioMap.Add(4, false);
            _gpioMap.Add(5, false);
            _gpioMap.Add(6, false);
            _gpioMap.Add(12, false);
            _gpioMap.Add(13, false);
            _gpioMap.Add(16, false);
            _gpioMap.Add(17, false);
            _gpioMap.Add(18, false);
            _gpioMap.Add(19, false);
            _gpioMap.Add(20, false);
            _gpioMap.Add(21, false);
            _gpioMap.Add(22, false);
            _gpioMap.Add(23, false);
            _gpioMap.Add(24, false);
            _gpioMap.Add(25, false);
            _gpioMap.Add(26, false);
            _gpioMap.Add(27, false);
            _gpioMap.Add(35, false);
            _gpioMap.Add(47, false);
        }

        public void Allocate(int pin)
        {
            if (!_gpioMap.ContainsKey(pin))
            {
                throw new ArgumentOutOfRangeException("Invalid pin number: " + pin);
            }

            if (_gpioMap[pin])
            {
                throw new ArgumentException("Pin is already allocated.");
            }

            _gpioMap[pin] = true;
        }

        public bool Verify(int pin)
        {
            return _gpioMap.ContainsKey(pin);
        }

        public void DeAllocate(int pin)
        {
            if (_gpioMap.ContainsKey(pin))
            {
                _gpioMap[pin] = false;
            }
        }

        public void Reset()
        {
            foreach (var key in _gpioMap.Keys)
            {
                _gpioMap[key] = false;
            }
        }
    }
}
