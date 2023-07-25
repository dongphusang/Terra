using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra
{
    // watering frequency; how often plkant 
    public enum WateringFreq
    {
        DAILY = 1,      // water everyday
        THREE_DAY = 3,  // water every three days
        FIVE_DAY = 5,   // water every five days
        WEEKLY = 7      // water every week
    }

    // mist modes; intensity of mist when watering plant
    public enum WateringMode
    {
        MIST_BRIEF = 0, // intended for brief watering
        MIST_HEAVY = 1, // intended for water to reach soil
    }
}
