using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace rpi.server.Controllers
{
    [Route("api/[controller]")]
    public class LedController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get(int id, bool isOn)
        {
            var pin = Pi.Gpio.Pins[id];

            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(isOn);

            return new string[] { $"a kapott pin: {id}", isOn ? "bekapcsol" : "kikapcsol" };
        }
    }
}
