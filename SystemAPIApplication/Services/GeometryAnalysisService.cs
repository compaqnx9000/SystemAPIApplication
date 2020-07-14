using Microsoft.Extensions.Options;
using MongoDB.Bson;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemAPIApplication.bo;
using SystemAPIApplication.core;
using SystemAPIApplication.enums;
using SystemAPIApplication.Utils;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Services
{
    public class GeometryAnalysisService : IGeometryAnalysisService
    {
        private readonly IMongoService _mongoService;
        private ServiceUrls _config;

        public GeometryAnalysisService(IMongoService mongoService, IOptions<ServiceUrls> options)
        {
            _mongoService = mongoService ??
               throw new ArgumentNullException(nameof(mongoService));
            _config = options.Value;
        }

        public List<MultiVO> FireballMulti()
        {
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺

                double r = GetFireBallRadius(yield/1000, alt* Utils.Const.M2FT);

                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, 0, ""));
            }
            return multis;
        }
        public List<MultiVO> FireballMulti(string[] bo)
        {
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺

                double r = GetFireBallRadius(yield/1000, alt * Utils.Const.M2FT);

                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, 0, ""));
            }
            return multis;
        }
        public GeometryVO FireballMerge()
        {
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= 3.2808399;

                if (geom == null)
                    geom = GetFireBallGeometry(lon, lat, yield, alt);
                else
                    geom = geom.Union(GetFireBallGeometry(lon, lat, yield, alt));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), 0, "");

        }
        public GeometryVO FireballMerge(string[] bo)
        {
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= 3.2808399;

                if (geom == null)
                    geom = GetFireBallGeometry(lon, lat, yield, alt);
                else
                    geom = geom.Union(GetFireBallGeometry(lon, lat, yield, alt));
            }

            return new GeometryVO(Translate.Geometry2GeoJson(geom), 0, "");
        }

        #region 早期核辐射
        public List<MultiVO> NuclearradiationMulti()
        {
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺

                double limits = 100;
                string unit = "REM";
                var rule = _mongoService.QueryRule("早期核辐射");
                if (rule != null)
                {
                    unit = rule.unit;
                    limits = rule.limits;
                }

                MyAnalyse myAnalyse = new MyAnalyse();
                double r = myAnalyse.CalcNuclearRadiationRadius(yield / 1000, alt * Utils.Const.M2FT, limits);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, limits, unit));
            }
            return multis;
        }
        public List<MultiVO> NuclearradiationMulti(string[] bo)
        {
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺

                double limits = 100;
                string unit = "REM";
                var rule = _mongoService.QueryRule("早期核辐射");
                if (rule != null)
                {
                    unit = rule.unit;
                    limits = rule.limits;
                }

                MyAnalyse myAnalyse = new MyAnalyse();
                double r = myAnalyse.CalcNuclearRadiationRadius(yield / 1000, alt * Utils.Const.M2FT, limits);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, limits, unit));
            }
            return multis;
        }
        public GeometryVO NuclearradiationMerge()
        {
            double limits = 100;
            string unit = "REM"; 
            
            var rule = _mongoService.QueryRule("早期核辐射");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }

            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= 3.2808399;

                if (geom == null)
                    geom = GetNuclearRadiationGeometry(lon, lat, yield, alt,limits);
                else
                    geom = geom.Union(GetNuclearRadiationGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }
        public GeometryVO NuclearradiationMerge(string[] bo)
        {
            double limits = 100;
            string unit = "REM";

            var rule = _mongoService.QueryRule("早期核辐射");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }

            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= 3.2808399;

                if (geom == null)
                    geom = GetNuclearRadiationGeometry(lon, lat, yield, alt, limits);
                else
                    geom = geom.Union(GetNuclearRadiationGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }

        #endregion

        #region 冲击波
        public List<MultiVO> AirblastMulti()
        {
            double limits = 1;
            string unit = "PSI";

            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺
                double r = GetShockWaveRadius(yield / 1000, alt * 3.2808399, limits);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, limits, unit));
            }

            return multis;
        }
        public List<MultiVO> AirblastMulti(string[] bo)
        {
            double limits = 1;
            string unit = "PSI";

            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺
                double r = GetShockWaveRadius(yield / 1000, alt * 3.2808399, limits);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, limits, unit));
            }
            return multis;

        }
        public GeometryVO AirblastMerge()
        {
            double limits = 1;
            string unit = "PSI";

            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= 3.2808399;

                if (geom == null)
                    geom = GetShockWaveGeometry(lon, lat, yield, alt, limits);
                else
                    geom = geom.Union(GetShockWaveGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }
        public GeometryVO AirblastMerge(string[] bo)
        {
            double limits = 1;
            string unit = "PSI";

            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= 3.2808399;

                if (geom == null)
                    geom = GetShockWaveGeometry(lon, lat, yield, alt,limits);
                else
                    geom = geom.Union(GetShockWaveGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }
        #endregion

        #region 光辐射
        public List<MultiVO> ThermalradiationMulti()
        {
            double limits = 1.9;
            string unit = "CAL/CM²";

            var rule = _mongoService.QueryRule("光辐射");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺
                double r = GetThermalRadiationRadius(yield / 1000, alt * Const.M2FT, limits);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, limits, unit));
            }
            return multis;
        }
        public List<MultiVO> ThermalradiationMulti(string[] bo)
        {
            double limits = 1.9;
            string unit = "CAL/CM²";

            var rule = _mongoService.QueryRule("光辐射");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;



                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 传入的是吨，要变成千吨;输入的是米：要变成：英尺
                double r = GetThermalRadiationRadius(yield / 1000, alt * 3.2808399, limits);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, limits, unit));
            }
            return multis;

        }
        public GeometryVO ThermalradiationMerge()
        {
            double limits = 1.9;
            string unit = "CAL/CM²";

            var rule = _mongoService.QueryRule("光辐射");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= 3.2808399;

                if (geom == null)
                    geom = GetThermalRadiationGeometry(lon, lat, yield, alt, limits);
                else
                    geom = geom.Union(GetThermalRadiationGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }
        public GeometryVO ThermalradiationMerge(string[] bo)
        {
            double limits = 1.9;
            string unit = "CAL/CM²";

            var rule = _mongoService.QueryRule("光辐射");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 传入的是吨，要变成千吨
                yield /= 1000;

                // 输入的是米：要变成：英尺
                alt *= Const.M2FT;

                if (geom == null)
                    geom = GetThermalRadiationGeometry(lon, lat, yield, alt, limits);
                else
                    geom = geom.Union(GetThermalRadiationGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }
        #endregion

        #region 核电磁脉冲
        public List<MultiVO> NuclearpulseMulti()
        {
            double limits = 200;
            string unit = "V/M";

            var rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;



                // 核电磁脉冲的当量就是“吨”，这里就不需要变成“千吨”了,但是高度要变成km
                double r = GetNuclearPulseRadius(yield, alt / 1000.0, limits);

                // 返回值的alt单位是：米,所以要把 r 乘以 1000
                multis.Add(new MultiVO(id, Math.Round(r * 1000, 2), lon, lat, alt, limits, unit));
            }
            return multis;
        }
        public List<MultiVO> NuclearpulseMulti(string[] bo)
        {
            double limits = 200;
            string unit = "V/M";

            var rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                // 核电磁脉冲的当量就是“吨”，这里就不需要变成“千吨”了,但是高度要变成km
                double r = GetNuclearPulseRadius(yield, alt / 1000.0, limits);

                // 返回值的alt单位是：米,所以要把 r 乘以 1000
                multis.Add(new MultiVO(id, Math.Round(r * 1000, 2), lon, lat, alt, limits, unit));
            }
            return multis;

        }
        public GeometryVO NuclearpulseMerge()
        {
            double limits = 200;
            string unit = "V/M";

            var rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 核电磁脉冲的当量就是“吨”，这里就不需要变成“千吨”了

                // 爆高需要传入km，所以要除以1000.
                alt /= 1000;

                if (geom == null)
                    geom = GetNuclearPulseGeometry(lon, lat, yield, alt, limits);
                else
                    geom = geom.Union(GetNuclearPulseGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }
        public GeometryVO NuclearpulseMerge(string[] bo)
        {
            double limits = 200;
            string unit = "V/M";

            var rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null)
            {
                unit = rule.unit;
                limits = rule.limits;
            }
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 核电磁脉冲的当量就是“吨”，这里就不需要变成“千吨”了

                // 爆高需要传入km，所以要除以1000.
                alt /= 1000;

                if (geom == null)
                    geom = GetNuclearPulseGeometry(lon, lat, yield, alt, limits);
                else
                    geom = geom.Union(GetNuclearPulseGeometry(lon, lat, yield, alt, limits));
            }
            return new GeometryVO(Translate.Geometry2GeoJson(geom), limits, unit);

        }
        #endregion

        #region 核沉降
        public List<FalloutVO> FalloutMulti()
        {
            //TODO:  需要读取API获取风速和风向
            double wind_speed = 225;
            double wind_dir = 15;

            // 读取mongo数据库中HB库，用于仿真模拟
            List<FalloutVO> multis = new List<FalloutVO>();

            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                if (yield >= 1000)
                {
                    // 传入的是吨，要变成千吨
                    yield /= 1000;

                    // 输入的是米：要变成：英尺
                    alt *= 3.2808399;

                    //天气接口
                    string url = _config.Weather;//https://localhost:5001/weather

                    var timeUtc = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

                    WeatherBO weatherBO = new WeatherBO(lon, lat, alt, timeUtc);
                    string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(weatherBO);

                    try
                    {
                        Task<string> s = HttpCli.PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    Geometry geom = GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);

                    multis.Add(new FalloutVO(id, Translate.Geometry2GeoJson(geom), 1,1, "rads/h"));
                }

            }
            return multis;
        }
        public List<FalloutVO> FalloutMulti(string[] bo)
        {
            //TODO:  需要读取API获取风速和风向
            double wind_speed = 225;
            double wind_dir = 15;

            // 读取mongo数据库中HB库，用于仿真模拟
            List<FalloutVO> multis = new List<FalloutVO>();

            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;//米
                double yield = fireball.GetValue("Yield").AsDouble;//吨
                string id = fireball.GetValue("NuclearExplosionID").AsString;

                if (yield >= 1000)
                {
                    // 传入的是吨，要变成千吨
                    yield /= 1000;

                    // 输入的是米：要变成：英尺
                    alt *= 3.2808399;

                    //天气接口
                    string url = _config.Weather;//https://localhost:5001/weather

                    var timeUtc = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

                    WeatherBO weatherBO = new WeatherBO(lon, lat, alt, timeUtc);
                    string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(weatherBO);

                    try
                    {
                        Task<string> s = HttpCli.PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    Geometry geom = GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);

                    multis.Add(new FalloutVO(id, Translate.Geometry2GeoJson(geom), 1,1, "rads/h"));

                }
            }
            return multis;

        }
        public GeometryMergeVO FalloutMerge()
        {
            //TODO:  需要读取API获取风速和风向

            double wind_speed = 225;
            double wind_dir = 15;



            // 读取mongo数据库中HB库，用于仿真模拟
            List<FalloutVO> multis = new List<FalloutVO>();

            Geometry geom = null;
            List<BsonDocument> result = _mongoService.QueryAll();
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                if (yield >= 1000)
                {
                    // 传入的是吨，要变成千吨
                    yield /= 1000;

                    // 输入的是米：要变成：英尺
                    alt *= 3.2808399;

                    //天气接口
                    string url = _config.Weather;//https://localhost:5001/weather

                    var timeUtc = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

                    WeatherBO weatherBO = new WeatherBO(lon, lat, alt, timeUtc);
                    string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(weatherBO);

                    try
                    {
                        Task<string> s = HttpCli.PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    if (geom == null)
                        geom = GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);
                    else
                        geom = geom.Union(GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir));

                }
            }
            return new GeometryMergeVO(Translate.Geometry2GeoJson(geom), 1, 1, "rads/h");
        }
        public GeometryMergeVO FalloutMerge(string[] bo)
        {
            //TODO:  需要读取API获取风速和风向
            double wind_speed = 225;
            double wind_dir = 15;

            // 读取mongo数据库中HB库，用于仿真模拟
            List<FalloutVO> multis = new List<FalloutVO>();

            Geometry geom = null;
            List<BsonDocument> result = _mongoService.Query(bo);
            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                if (yield >= 1000)
                {
                    // 传入的是吨，要变成千吨
                    yield /= 1000;

                    // 输入的是米：要变成：英尺
                    alt *= 3.2808399;

                    //天气接口
                    string url = _config.Weather;//https://localhost:5001/weather

                    var timeUtc = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

                    WeatherBO weatherBO = new WeatherBO(lon, lat, alt, timeUtc);
                    string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(weatherBO);

                    try
                    {
                        Task<string> s = HttpCli.PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    if (geom == null)
                        geom = GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);
                    else
                        geom = geom.Union(GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir));

                }
            }
            return new GeometryMergeVO(Translate.Geometry2GeoJson(geom), 1, 1, "rads/h");

        }
        #endregion

        #region 多种融合
        public List<DamageMultiVO> Multi()
        {
            double psi = 1;
            double rem = 100;
            double calcm = 1.9;
            double vm = 200;
            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null) psi = rule.limits;

            rule = _mongoService.QueryRule("早期核辐射");
            if (rule != null) rem = rule.limits;

            rule = _mongoService.QueryRule("光辐射");
            if (rule != null) calcm = rule.limits;

            rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null) vm = rule.limits;

            // 读取mongo数据库中HB库，用于仿真模拟
            List<DamageMultiVO> damageMultiVOs = new List<DamageMultiVO>();

            List<BsonDocument> result = _mongoService.QueryAll();

            List<MultiVO> multiVOs_01 = new List<MultiVO>();
            List<MultiVO> multiVOs_02 = new List<MultiVO>();
            List<MultiVO> multiVOs_03 = new List<MultiVO>();
            List<MultiVO> multiVOs_04 = new List<MultiVO>();
            List<MultiVO> multiVOs_05 = new List<MultiVO>();

            foreach (BsonDocument nuke in result)
            {
                double lon = nuke.GetValue("Lon").AsDouble;
                double lat = nuke.GetValue("Lat").AsDouble;
                double alt = nuke.GetValue("Alt").AsDouble;
                double yield = nuke.GetValue("Yield").AsDouble;

                string id = nuke.GetValue("NuclearExplosionID").AsString;

                // 吨变千吨；米变英尺
                double r = GetFireBallRadius(yield / 1000, alt * Const.M2FT);
                multiVOs_01.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, 0, ""));

                r = GetNuclearRadiationRadius(yield / 1000, alt * Const.M2FT);
                multiVOs_02.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, rem, "rem"));

                r = GetShockWaveRadius(yield / 1000, alt * Const.M2FT, 99999);
                multiVOs_03.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, psi, "psi"));

                r = GetNuclearRadiationRadius(yield / 1000, alt * Const.M2FT);
                multiVOs_04.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, calcm, "cal/cm²"));

                // 吨不变；米变千米
                r = GetNuclearPulseRadius(yield, alt / 1000.0, 999);

                // 上一步r的返回值是千米，所以要变成米
                multiVOs_05.Add(new MultiVO(id, Math.Round(r * 1000, 2), lon, lat, alt, vm, "v/m"));

            }
            damageMultiVOs.Add(new DamageMultiVO("H火球", multiVOs_01));
            damageMultiVOs.Add(new DamageMultiVO("核辐射", multiVOs_02));
            damageMultiVOs.Add(new DamageMultiVO("冲击波", multiVOs_03));
            damageMultiVOs.Add(new DamageMultiVO("热/光辐射", multiVOs_04));
            damageMultiVOs.Add(new DamageMultiVO("电磁脉冲", multiVOs_05));

            return damageMultiVOs;
        }
        public List<DamageMultiVO> Multi(string[] bo)
        {
            double psi = 1;
            double rem = 100;
            double calcm = 1.9;
            double vm = 200;
            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null) psi = rule.limits;

            rule = _mongoService.QueryRule("早期核辐射");
            if (rule != null) rem = rule.limits;

            rule = _mongoService.QueryRule("光辐射");
            if (rule != null) calcm = rule.limits;

            rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null) vm = rule.limits;

            // 读取mongo数据库中HB库，用于仿真模拟
            List<DamageMultiVO> damageMultiVOs = new List<DamageMultiVO>();

            List<BsonDocument> result = _mongoService.Query(bo);

            List<MultiVO> multiVOs_01 = new List<MultiVO>();
            List<MultiVO> multiVOs_02 = new List<MultiVO>();
            List<MultiVO> multiVOs_03 = new List<MultiVO>();
            List<MultiVO> multiVOs_04 = new List<MultiVO>();
            List<MultiVO> multiVOs_05 = new List<MultiVO>();

            foreach (BsonDocument nuke in result)
            {
                double lon = nuke.GetValue("Lon").AsDouble;
                double lat = nuke.GetValue("Lat").AsDouble;
                double alt = nuke.GetValue("Alt").AsDouble;
                double yield = nuke.GetValue("Yield").AsDouble;

                string id = nuke.GetValue("NuclearExplosionID").AsString;

                // 吨变千吨；米变英尺
                double r = GetFireBallRadius(yield / 1000, alt * Const.M2FT);
                multiVOs_01.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, 0, ""));

                r = GetNuclearRadiationRadius(yield / 1000, alt * Const.M2FT);
                multiVOs_02.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, rem, "rem"));

                r = GetShockWaveRadius(yield / 1000, alt * Const.M2FT, 99999);
                multiVOs_03.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, psi, "psi"));

                r = GetNuclearRadiationRadius(yield / 1000, alt * Const.M2FT);
                multiVOs_04.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt, calcm, "cal/cm²"));

                // 吨不变；米变千米
                r = GetNuclearPulseRadius(yield, alt / 1000.0, 999);

                // 上一步r的返回值是千米，所以要变成米
                multiVOs_05.Add(new MultiVO(id, Math.Round(r * 1000, 2), lon, lat, alt, vm, "v/m"));

            }
            damageMultiVOs.Add(new DamageMultiVO("H火球", multiVOs_01));
            damageMultiVOs.Add(new DamageMultiVO("核辐射", multiVOs_02));
            damageMultiVOs.Add(new DamageMultiVO("冲击波", multiVOs_03));
            damageMultiVOs.Add(new DamageMultiVO("热/光辐射", multiVOs_04));
            damageMultiVOs.Add(new DamageMultiVO("电磁脉冲", multiVOs_05));

            return damageMultiVOs;
        }

        public List<DamageMergeVO> Merge()
        {
            double psi = 1;
            double rem = 100;
            double calcm = 1.9;
            double vm = 200;
            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null) psi = rule.limits;

            rule = _mongoService.QueryRule("早期核辐射");
            if (rule != null) rem = rule.limits;

            rule = _mongoService.QueryRule("光辐射");
            if (rule != null) calcm = rule.limits;

            rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null) vm = rule.limits;

            // 读取mongo数据库中HB库，用于仿真模拟
            List<DamageMergeVO> damageMergeVOs = new List<DamageMergeVO>();

            List<BsonDocument> result = _mongoService.QueryAll();

            Geometry geom_01 = null;
            Geometry geom_02 = null;
            Geometry geom_03 = null;
            Geometry geom_04 = null;
            Geometry geom_05 = null;


            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 吨变千吨；米变英尺
                if (geom_01 == null)
                    geom_01 = GetFireBallGeometry(lon, lat, yield / 1000, alt * Const.M2FT);
                else
                    geom_01 = geom_01.Union(GetFireBallGeometry(lon, lat, yield / 1000, alt * Const.M2FT));

                if (geom_02 == null)
                    geom_02 = GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, rem);
                else
                    geom_02 = geom_02.Union(GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, rem));

                if (geom_03 == null)
                    geom_03 = GetShockWaveGeometry(lon, lat, yield / 1000, alt * Const.M2FT, psi);
                else
                    geom_03 = geom_03.Union(GetShockWaveGeometry(lon, lat, yield / 1000, alt * Const.M2FT, psi));

                if (geom_04 == null)
                    geom_04 = GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, calcm);
                else
                    geom_04 = geom_04.Union(GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, calcm));

                //吨不变，米变千米
                if (geom_05 == null)
                    geom_05 = GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0, vm);
                else
                    geom_05 = geom_05.Union(GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0, vm));

            }

            damageMergeVOs.Add(new DamageMergeVO("H火球", Translate.Geometry2GeoJson(geom_01), 0, ""));
            damageMergeVOs.Add(new DamageMergeVO("核辐射", Translate.Geometry2GeoJson(geom_02), rem, "rem"));
            damageMergeVOs.Add(new DamageMergeVO("冲击波", Translate.Geometry2GeoJson(geom_03), psi, "psi"));
            damageMergeVOs.Add(new DamageMergeVO("热/光辐射", Translate.Geometry2GeoJson(geom_04), calcm, "cal/cm²"));
            damageMergeVOs.Add(new DamageMergeVO("电磁脉冲", Translate.Geometry2GeoJson(geom_05), vm, "v/m"));

            return damageMergeVOs;
        }
        public List<DamageMergeVO> Merge(string[] bo)
        {
            double psi = 1;
            double rem = 100;
            double calcm = 1.9;
            double vm = 200;
            var rule = _mongoService.QueryRule("冲击波");
            if (rule != null) psi = rule.limits;

            rule = _mongoService.QueryRule("早期核辐射");
            if (rule != null) rem = rule.limits;

            rule = _mongoService.QueryRule("光辐射");
            if (rule != null) calcm = rule.limits;

            rule = _mongoService.QueryRule("核电磁脉冲");
            if (rule != null) vm = rule.limits;

            // 读取mongo数据库中HB库，用于仿真模拟
            List<DamageMergeVO> damageMergeVOs = new List<DamageMergeVO>();

            List<BsonDocument> result = _mongoService.Query(bo);

            Geometry geom_01 = null;
            Geometry geom_02 = null;
            Geometry geom_03 = null;
            Geometry geom_04 = null;
            Geometry geom_05 = null;


            foreach (BsonDocument fireball in result)
            {
                double lon = fireball.GetValue("Lon").AsDouble;
                double lat = fireball.GetValue("Lat").AsDouble;
                double alt = fireball.GetValue("Alt").AsDouble;
                double yield = fireball.GetValue("Yield").AsDouble;

                // 吨变千吨；米变英尺
                if (geom_01 == null)
                    geom_01 = GetFireBallGeometry(lon, lat, yield / 1000, alt * Const.M2FT);
                else
                    geom_01 = geom_01.Union(GetFireBallGeometry(lon, lat, yield / 1000, alt * Const.M2FT));

                if (geom_02 == null)
                    geom_02 = GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, rem);
                else
                    geom_02 = geom_02.Union(GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, rem));

                if (geom_03 == null)
                    geom_03 = GetShockWaveGeometry(lon, lat, yield / 1000, alt * Const.M2FT, psi);
                else
                    geom_03 = geom_03.Union(GetShockWaveGeometry(lon, lat, yield / 1000, alt * Const.M2FT, psi));

                if (geom_04 == null)
                    geom_04 = GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, calcm);
                else
                    geom_04 = geom_04.Union(GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * Const.M2FT, calcm));

                //吨不变，米变千米
                if (geom_05 == null)
                    geom_05 = GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0, vm);
                else
                    geom_05 = geom_05.Union(GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0, vm));

            }

            damageMergeVOs.Add(new DamageMergeVO("H火球", Translate.Geometry2GeoJson(geom_01), 0, ""));
            damageMergeVOs.Add(new DamageMergeVO("核辐射", Translate.Geometry2GeoJson(geom_02), rem, "rem"));
            damageMergeVOs.Add(new DamageMergeVO("冲击波", Translate.Geometry2GeoJson(geom_03), psi, "psi"));
            damageMergeVOs.Add(new DamageMergeVO("热/光辐射", Translate.Geometry2GeoJson(geom_04), calcm, "cal/cm²"));
            damageMergeVOs.Add(new DamageMergeVO("电磁脉冲", Translate.Geometry2GeoJson(geom_05), vm, "v/m"));

            return damageMergeVOs;
        }

        #endregion


        /*  
         *  火球  
         */
        private Geometry GetFireBallGeometry(double lon, double lat, double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            double r =  myAnalyse.CalcfireBallRadius(yield, alt > 0);

            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);


            return geom;
        }
        private double GetFireBallRadius(double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcfireBallRadius(yield, alt > 0);
        }

        /* 
         * 核辐射 
         */


        private Geometry GetNuclearRadiationGeometry(double lon, double lat, double yield, double alt,double limits)
        {
            MyAnalyse myAnalyse = new MyAnalyse();

            double r = myAnalyse.CalcNuclearRadiationRadius(yield, alt, limits);

            //根据(lng,lat,R)生成Geometry
            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);

            return geom;
        }

        private double GetNuclearRadiationRadius(double yield, double alt)
        {
            // 没用了
            double limits = 100;
            var rule = _mongoService.QueryRule("早期核辐射");
            if (rule != null)
                limits = rule.limits;

            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcNuclearRadiationRadius(yield, alt, limits);
        }

        /* 
         * 冲击波 
         */
        private Geometry GetShockWaveGeometry(double lon, double lat, double yield, double alt,double limits)
        {

            MyAnalyse myAnalyse = new MyAnalyse();

            // 求核爆影响范围半径[54609,21249,10101,3734,1537]
            double r = myAnalyse.CalcShockWaveRadius(yield, alt, limits);

            //根据(lng,lat,R)生成Geometry
            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);
            return geom;
        }

        private double GetShockWaveRadius(double yield, double alt,double limits)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcShockWaveRadius(yield,alt, limits);
        }

        /* 
         * 光、热辐射 
         */
        private Geometry GetThermalRadiationGeometry(double lon, double lat, double yield, double alt,double limits)
        {

            MyAnalyse myAnalyse = new MyAnalyse();

            double r = myAnalyse.GetThermalRadiationR(yield, lat, limits);

            //根据(lng,lat,R)生成Geometry
            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);

            return geom;
        }
        private double GetThermalRadiationRadius(double yield, double alt,double limits)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.GetThermalRadiationR(yield, alt, limits);
        }

        /* 
        * 核电磁脉冲 
        */
        private Geometry GetNuclearPulseGeometry(double lon, double lat, double yield, double alt,double limits)
        {
            MyAnalyse myAnalyse = new MyAnalyse();

            double r = myAnalyse.CalcNuclearPulseRadius(yield, alt, limits);

            //根据(lng,lat,R)生成Geometry。
            // 注意，因为r返回的就是千米，而BuildCircle，需要传入的也是千米，所以就不用除以1000了
            Geometry geom = Translate.BuildCircle(lon, lat, r, 50);
            return geom;
        }
        private double GetNuclearPulseRadius(double yield, double alt,double limits)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcNuclearPulseRadius(yield, alt, limits);
        }

        /*
         * 核沉降
         */
        private Geometry GetFalloutGeometry(double lon, double lat, double yield, double alt, double wind_speed, double wind_dir)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            double maximumDownwindDistance = 0;
            double maximumWidth = 0;
            List<Coor> coors = myAnalyse.CalcRadioactiveFalloutRegion(
                lon, lat, alt, yield, wind_speed, wind_dir, DamageEnumeration.Light,
                ref maximumDownwindDistance, ref maximumWidth);

            List<Coordinate> coordinates = new List<Coordinate>();
            for (int i = 0; i < coors.Count; i++)
            {
                coordinates.Add(new Coordinate(coors[i].lng, coors[i].lat));
            }

            // 把coordinators 转换成geometry
            Coordinate[] coords = coordinates.ToArray();
            Polygon polygon = new NetTopologySuite.Geometries.Polygon(
                new LinearRing(coords));

            return polygon;
        }



    }
}
