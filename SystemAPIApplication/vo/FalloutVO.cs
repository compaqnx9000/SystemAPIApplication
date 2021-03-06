﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemAPIApplication.vo
{
    public class FalloutVO
    {
        public FalloutVO(string nuclearExplosionID, string damageGeometry, double radValue,double value,string unit)
        {
            this.nuclearExplosionID = nuclearExplosionID;
            this.damageGeometry = damageGeometry;
            this.radValue = radValue;
            this.value = value;
            this.unit = unit;
        }

        public string nuclearExplosionID { get; set; }
        public string damageGeometry { get; set; }
        public double radValue { get; set; }
        public double value { get; set; }
        public string unit { get; set; }
    }
}
