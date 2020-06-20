using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemAPIApplication.vo
{
    public class MultiVO
    {
        public MultiVO(string nuclearExplosionID, double damageRadius, double lon, double lat, double alt)
        {
            this.nuclearExplosionID = nuclearExplosionID;
            this.damageRadius = damageRadius;
            this.lon = lon;
            this.lat = lat;
            this.alt = alt;
        }

        public string nuclearExplosionID { get; set; }
        public double damageRadius { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
        public double alt { get; set; }
        
    }
}
