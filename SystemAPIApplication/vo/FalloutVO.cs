using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemAPIApplication.vo
{
    public class FalloutVO
    {
        public FalloutVO(string nuclearExplosionID, string damageGeometry, double radValue)
        {
            this.nuclearExplosionID = nuclearExplosionID;
            this.damageGeometry = damageGeometry;
            this.radValue = radValue;
        }

        public string nuclearExplosionID { get; set; }
        public string damageGeometry { get; set; }
        public double radValue { get; set; }

    }
}
