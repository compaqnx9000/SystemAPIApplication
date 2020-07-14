using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemAPIApplication.vo
{
    public class GeometryMergeVO
    {
        public GeometryMergeVO(string geometry, double radValue,double value, string unit)
        {
            this.damageGeometry = geometry;
            this.radValue = radValue;
            this.value = value;
            this.unit = unit;
        }

        public string damageGeometry { get; set; }
        public double radValue { get; set; }
        public double value { get; set; }
        public string unit { get; set; }
    }
}
