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
        public DamageMergeVO(string damageGeometry, string damageType)
        {
            this.damageGeometry = damageGeometry;
            this.damageType = damageType;
        }

        public string damageGeometry { get; set; }
        public string damageType { get; set; }
    }
}
