using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemAPIApplication.vo
{
    public class GeometryVO
    {
        public GeometryVO(string geometry, double value, string unit)
        {
            this.geometry = geometry;
            this.value = value;
            this.unit = unit;
        }

        public string geometry { get; set; }
        public double value { get; set; }
        public string unit { get; set; }
    }
}
