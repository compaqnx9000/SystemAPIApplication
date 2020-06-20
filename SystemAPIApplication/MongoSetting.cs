using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemAPIApplication
{
    public class MongoSetting
    {
        /// <summary>
        　　/// 数据库连接字符串
        　　/// </summary>
        public string IP { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Pwd { get; set; }
        public string Document { get; set; }
        public string Collection { get; set; }
    }
}
