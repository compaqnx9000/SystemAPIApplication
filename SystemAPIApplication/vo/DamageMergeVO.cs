using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemAPIApplication.vo
{
    /// <summary>
    ///  毁伤区域_融合
    /// </summary>
    public class DamageMergeVO
    {
        public DamageMergeVO(string damageType, string damageGeometry,  double value, string unit)
        {
            this.damageGeometry = damageGeometry;
            this.damageType = damageType;
            this.value = value;
            this.unit = unit;
        }

        public string damageGeometry { get; set; }
        public string damageType { get; set; }
        public double value { get; set; }
        public string unit { get; set; }
    }
}
