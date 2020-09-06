using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemAPIApplication.bo;
using SystemAPIApplication.vo;
namespace SystemAPIApplication.Services
{
    public class GeometryAnalysisService : IGeometryAnalysisService
    {
        public IConfiguration Configuration { get; }

        private readonly IMongoService _mongoService;

        public GeometryAnalysisService(IConfiguration configuration,IMongoService mongoService)
        {
            Configuration = configuration;

            _mongoService = mongoService ??
               throw new ArgumentNullException(nameof(mongoService));
        }

        public List<MultiVO> FireballMulti()
        {
            // 读取mongo数据库中HB库中的hbmock表，用于仿真模拟
            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (MockBO bo in bos)
            {
                double r = MyCore.NuclearAlgorithm.GetFireBallRadius(bo.Yield, bo.Alt);
                multis.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, 0, ""));
            }
            return multis;
        }
        public List<MultiVO> FireballMulti(string[] bo)
        {
            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> bos = _mongoService.QueryMock(bo);
            foreach (var b in bos)
            {
                double r = MyCore.NuclearAlgorithm.GetFireBallRadius(b.Yield, b.Alt);
                multis.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, 0, ""));
            }
            return multis;
        }
        public GeometryVO FireballMerge()
        {
            Geometry geom = null;
            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (MockBO bo in bos)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetFireBallGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetFireBallGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), 0, "");

        }
        public GeometryVO FireballMerge(string[] bo)
        {
            Geometry geom = null;
            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetFireBallGeometry(b.Lon, b.Lat, b.Yield, b.Alt);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetFireBallGeometry(b.Lon, b.Lat, b.Yield, b.Alt));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), 0, "");
        }

        #region 早期核辐射
        public List<MultiVO> NuclearradiationMulti()
        {
            var rule = QueryLimits("早期核辐射");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                double r = MyCore.NuclearAlgorithm.GetNuclearRadiationRadius(bo.Yield, bo.Alt, rule.limit);
                multis.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, rule.limit, rule.unit));
            }
            return multis;
        }
        public List<MultiVO> NuclearradiationMulti(string[] bo)
        {
            var rule = QueryLimits("早期核辐射");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                double r = MyCore.NuclearAlgorithm.GetNuclearRadiationRadius(b.Yield, b.Alt, rule.limit);
                multis.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, rule.limit, rule.unit));
            }
            return multis;
        }
        public GeometryVO NuclearradiationMerge()
        {
            var rule = QueryLimits("早期核辐射");

            Geometry geom = null;
            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt,rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);

        }
        public GeometryVO NuclearradiationMerge(string[] bo)
        {
            var rule = QueryLimits("早期核辐射");

            Geometry geom = null;
            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);

        }

        #endregion

        #region 冲击波
        public List<MultiVO> AirblastMulti()
        {
            var rule = QueryLimits("冲击波");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                double r = MyCore.NuclearAlgorithm.GetShockWaveRadius(bo.Yield, bo.Alt, rule.limit);
                multis.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, rule.limit, rule.unit));
            }

            return multis;
        }
        public List<MultiVO> AirblastMulti(string[] bo)
        {
            var rule = QueryLimits("冲击波");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                double r = MyCore.NuclearAlgorithm.GetShockWaveRadius(b.Yield, b.Alt, rule.limit);
                multis.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, rule.limit, rule.unit));
            }
            return multis;

        }
        public GeometryVO AirblastMerge()
        {
            var rule = QueryLimits("冲击波");

            Geometry geom = null;
            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetShockWaveGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetShockWaveGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);

        }
        public GeometryVO AirblastMerge(string[] bo)
        {
            var rule = QueryLimits("冲击波");

            Geometry geom = null;
            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetShockWaveGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetShockWaveGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);

        }
        #endregion

        #region 光辐射
        public List<MultiVO> ThermalradiationMulti()
        {
            var rule = QueryLimits("光辐射");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                double r = MyCore.NuclearAlgorithm.GetThermalRadiationRadius(bo.Yield, bo.Alt, rule.limit);
                multis.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, rule.limit, rule.unit));
            }
            return multis;
        }
        public List<MultiVO> ThermalradiationMulti(string[] bo)
        {
            var rule = QueryLimits("光辐射");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                double r = MyCore.NuclearAlgorithm.GetThermalRadiationRadius(b.Yield, b.Alt, rule.limit);
                multis.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, rule.limit, rule.unit));
            }
            return multis;
        }
        public GeometryVO ThermalradiationMerge()
        {
            var rule = QueryLimits("光辐射");

            Geometry geom = null;
            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);
        }
        public GeometryVO ThermalradiationMerge(string[] bo)
        {
            var rule = QueryLimits("光辐射");

            Geometry geom = null;
            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);
        }
        #endregion

        #region 核电磁脉冲
        public List<MultiVO> NuclearpulseMulti()
        {
            var rule = QueryLimits("核电磁脉冲");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                double r = MyCore.NuclearAlgorithm.GetNuclearPulseRadius(bo.Yield, bo.Alt, rule.limit);
                multis.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, rule.limit, rule.unit));
            }
            return multis;
        }
        public List<MultiVO> NuclearpulseMulti(string[] bo)
        {
            var rule = QueryLimits("核电磁脉冲");

            List<MultiVO> multis = new List<MultiVO>();

            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                double r = MyCore.NuclearAlgorithm.GetNuclearPulseRadius(b.Yield, b.Alt, rule.limit);
                multis.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r , 2), b.Lon, b.Lat, b.Alt, rule.limit, rule.unit));
            }
            return multis;
        }
        public GeometryVO NuclearpulseMerge()
        {
            var rule = QueryLimits("核电磁脉冲");

            Geometry geom = null;
            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);
        }
        public GeometryVO NuclearpulseMerge(string[] bo)
        {
            var rule = QueryLimits("核电磁脉冲");

            Geometry geom = null;
            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                if (geom == null)
                    geom = MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit);
                else
                    geom = geom.Union(MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rule.limit));
            }
            return new GeometryVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), rule.limit, rule.unit);
        }
        #endregion

        #region 核沉降
        public List<FalloutVO> FalloutMulti()
        {
            double wind_speed = 225;
            double wind_dir = 15;

            List<FalloutVO> multis = new List<FalloutVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                if (bo.Yield >= 1000)
                {
                    QueryWeather(bo.Lon, bo.Lat, bo.Alt, ref wind_speed, ref wind_dir);

                    Geometry geom = MyCore.NuclearAlgorithm.GetFalloutGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, wind_speed, wind_dir);

                    multis.Add(new FalloutVO(bo.NuclearExplosionID, MyCore.Utils.Translate.Geometry2GeoJson(geom), 1,1, "rads/h"));
                }

            }
            return multis;
        }
        public List<FalloutVO> FalloutMulti(string[] bo)
        {
            double wind_speed = 225;
            double wind_dir = 15;

            List<FalloutVO> multis = new List<FalloutVO>();

            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                if (b.Yield >= 1000)
                {
                    QueryWeather(b.Lon, b.Lat, b.Alt, ref wind_speed, ref wind_dir);

                    Geometry geom = MyCore.NuclearAlgorithm.GetFalloutGeometry(b.Lon, b.Lat, b.Yield, b.Alt, wind_speed, wind_dir);

                    multis.Add(new FalloutVO(b.NuclearExplosionID, MyCore.Utils.Translate.Geometry2GeoJson(geom), 1,1, "rads/h"));

                }
            }
            return multis;
        }
        public GeometryMergeVO FalloutMerge()
        {
            double wind_speed = 225;
            double wind_dir = 15;

            List<FalloutVO> multis = new List<FalloutVO>();

            Geometry geom = null;
            List<MockBO> bos = _mongoService.QueryMockAll();
            foreach (var bo in bos)
            {
                if (bo.Yield >= 1000)
                {
                    QueryWeather(bo.Lon, bo.Lat, bo.Alt, ref wind_speed, ref wind_dir);

                    if (geom == null)
                        geom = MyCore.NuclearAlgorithm.GetFalloutGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, wind_speed, wind_dir);
                    else
                        geom = geom.Union(MyCore.NuclearAlgorithm.GetFalloutGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, wind_speed, wind_dir));

                }
            }
            if (geom == null) 
                return null;

            return new GeometryMergeVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), 1, 1, "rads/h");
        }
        public GeometryMergeVO FalloutMerge(string[] bo)
        {
            double wind_speed = 225;
            double wind_dir = 15;

            List<FalloutVO> multis = new List<FalloutVO>();

            Geometry geom = null;
            List<MockBO> result = _mongoService.QueryMock(bo);
            foreach (var b in result)
            {
                if (b.Yield >= 1000)
                {
                    QueryWeather(b.Lon, b.Lat, b.Alt, ref wind_speed, ref wind_dir);

                    if (geom == null)
                        geom = MyCore.NuclearAlgorithm.GetFalloutGeometry(b.Lon, b.Lat, b.Yield, b.Alt, wind_speed, wind_dir);
                    else
                        geom = geom.Union(MyCore.NuclearAlgorithm.GetFalloutGeometry(b.Lon, b.Lat, b.Yield, b.Alt, wind_speed, wind_dir));

                }
            }
            if (geom == null)
                return null;

            return new GeometryMergeVO(MyCore.Utils.Translate.Geometry2GeoJson(geom), 1, 1, "rads/h");
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

            List<DamageMultiVO> damageMultiVOs = new List<DamageMultiVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();

            List<MultiVO> multiVOs_01 = new List<MultiVO>();
            List<MultiVO> multiVOs_02 = new List<MultiVO>();
            List<MultiVO> multiVOs_03 = new List<MultiVO>();
            List<MultiVO> multiVOs_04 = new List<MultiVO>();
            List<MultiVO> multiVOs_05 = new List<MultiVO>();

            foreach (var bo in bos)
            {
                double r = MyCore.NuclearAlgorithm.GetFireBallRadius(bo.Yield, bo.Alt);
                multiVOs_01.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, 0, ""));

                r = MyCore.NuclearAlgorithm.GetNuclearRadiationRadius(bo.Yield, bo.Alt, rem);
                multiVOs_02.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, rem, "rem"));

                r = MyCore.NuclearAlgorithm.GetShockWaveRadius(bo.Yield, bo.Alt, psi);
                multiVOs_03.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, psi, "psi"));

                r = MyCore.NuclearAlgorithm.GetThermalRadiationRadius(bo.Yield, bo.Alt, calcm);
                multiVOs_04.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r, 2), bo.Lon, bo.Lat, bo.Alt, calcm, "cal/cm²"));

                r = MyCore.NuclearAlgorithm.GetNuclearPulseRadius(bo.Yield, bo.Alt, vm);
                multiVOs_05.Add(new MultiVO(bo.NuclearExplosionID, Math.Round(r , 2), bo.Lon, bo.Lat, bo.Alt, vm, "v/m"));

            }
            damageMultiVOs.Add(new DamageMultiVO("核火球", multiVOs_01));
            damageMultiVOs.Add(new DamageMultiVO("早期核辐射", multiVOs_02));
            damageMultiVOs.Add(new DamageMultiVO("冲击波", multiVOs_03));
            damageMultiVOs.Add(new DamageMultiVO("光辐射", multiVOs_04));
            damageMultiVOs.Add(new DamageMultiVO("核电磁脉冲", multiVOs_05));

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

            List<DamageMultiVO> damageMultiVOs = new List<DamageMultiVO>();

            List<MockBO> result = _mongoService.QueryMock(bo);

            List<MultiVO> multiVOs_01 = new List<MultiVO>();
            List<MultiVO> multiVOs_02 = new List<MultiVO>();
            List<MultiVO> multiVOs_03 = new List<MultiVO>();
            List<MultiVO> multiVOs_04 = new List<MultiVO>();
            List<MultiVO> multiVOs_05 = new List<MultiVO>();

            foreach (var b in result)
            {
                double r = MyCore.NuclearAlgorithm.GetFireBallRadius(b.Yield, b.Alt);
                multiVOs_01.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, 0, ""));

                r = MyCore.NuclearAlgorithm.GetNuclearRadiationRadius(b.Yield, b.Alt, rem);
                multiVOs_02.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, rem, "rem"));

                r = MyCore.NuclearAlgorithm.GetShockWaveRadius(b.Yield, b.Alt, psi);
                multiVOs_03.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, psi, "psi"));

                r = MyCore.NuclearAlgorithm.GetThermalRadiationRadius(b.Yield, b.Alt, calcm);
                multiVOs_04.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, calcm, "cal/cm²"));

                r = MyCore.NuclearAlgorithm.GetNuclearPulseRadius(b.Yield, b.Alt, vm);
                multiVOs_05.Add(new MultiVO(b.NuclearExplosionID, Math.Round(r, 2), b.Lon, b.Lat, b.Alt, vm, "v/m"));
            }

            damageMultiVOs.Add(new DamageMultiVO("核火球", multiVOs_01));
            damageMultiVOs.Add(new DamageMultiVO("早期核辐射", multiVOs_02));
            damageMultiVOs.Add(new DamageMultiVO("冲击波", multiVOs_03));
            damageMultiVOs.Add(new DamageMultiVO("光辐射", multiVOs_04));
            damageMultiVOs.Add(new DamageMultiVO("核电磁脉冲", multiVOs_05));

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

            List<DamageMergeVO> damageMergeVOs = new List<DamageMergeVO>();

            List<MockBO> bos = _mongoService.QueryMockAll();

            Geometry geom_01 = null;
            Geometry geom_02 = null;
            Geometry geom_03 = null;
            Geometry geom_04 = null;
            Geometry geom_05 = null;


            foreach (var bo in bos)
            {
                if (geom_01 == null)
                    geom_01 = MyCore.NuclearAlgorithm.GetFireBallGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt);
                else
                    geom_01 = geom_01.Union(MyCore.NuclearAlgorithm.GetFireBallGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt));

                if (geom_02 == null)
                    geom_02 = MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rem);
                else
                    geom_02 = geom_02.Union(MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, rem));

                if (geom_03 == null)
                    geom_03 = MyCore.NuclearAlgorithm.GetShockWaveGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, psi);
                else
                    geom_03 = geom_03.Union(MyCore.NuclearAlgorithm.GetShockWaveGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, psi));

                if (geom_04 == null)
                    geom_04 = MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, calcm);
                else
                    geom_04 = geom_04.Union(MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, calcm));

                if (geom_05 == null)
                    geom_05 = MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, vm);
                else
                    geom_05 = geom_05.Union(MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(bo.Lon, bo.Lat, bo.Yield, bo.Alt, vm));

            }

            damageMergeVOs.Add(new DamageMergeVO("核火球", MyCore.Utils.Translate.Geometry2GeoJson(geom_01), 0, ""));
            damageMergeVOs.Add(new DamageMergeVO("早期核辐射", MyCore.Utils.Translate.Geometry2GeoJson(geom_02), rem, "rem"));
            damageMergeVOs.Add(new DamageMergeVO("冲击波", MyCore.Utils.Translate.Geometry2GeoJson(geom_03), psi, "psi"));
            damageMergeVOs.Add(new DamageMergeVO("光辐射", MyCore.Utils.Translate.Geometry2GeoJson(geom_04), calcm, "cal/cm²"));
            damageMergeVOs.Add(new DamageMergeVO("核电磁脉冲", MyCore.Utils.Translate.Geometry2GeoJson(geom_05), vm, "v/m"));

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

            List<DamageMergeVO> damageMergeVOs = new List<DamageMergeVO>();

            List<MockBO> result = _mongoService.QueryMock(bo);

            Geometry geom_01 = null;
            Geometry geom_02 = null;
            Geometry geom_03 = null;
            Geometry geom_04 = null;
            Geometry geom_05 = null;

            foreach (var b in result)
            {
                if (geom_01 == null)
                    geom_01 = MyCore.NuclearAlgorithm.GetFireBallGeometry(b.Lon, b.Lat, b.Yield, b.Alt);
                else
                    geom_01 = geom_01.Union(MyCore.NuclearAlgorithm.GetFireBallGeometry(b.Lon, b.Lat, b.Yield, b.Alt));

                if (geom_02 == null)
                    geom_02 = MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rem);
                else
                    geom_02 = geom_02.Union(MyCore.NuclearAlgorithm.GetNuclearRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, rem));

                if (geom_03 == null)
                    geom_03 = MyCore.NuclearAlgorithm.GetShockWaveGeometry(b.Lon, b.Lat, b.Yield, b.Alt, psi);
                else
                    geom_03 = geom_03.Union(MyCore.NuclearAlgorithm.GetShockWaveGeometry(b.Lon, b.Lat, b.Yield, b.Alt, psi));

                if (geom_04 == null)
                    geom_04 = MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, calcm);
                else
                    geom_04 = geom_04.Union(MyCore.NuclearAlgorithm.GetThermalRadiationGeometry(b.Lon, b.Lat, b.Yield, b.Alt, calcm));

                if (geom_05 == null)
                    geom_05 = MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(b.Lon, b.Lat, b.Yield, b.Alt, vm);
                else
                    geom_05 = geom_05.Union(MyCore.NuclearAlgorithm.GetNuclearPulseGeometry(b.Lon, b.Lat, b.Yield, b.Alt, vm));

            }

            damageMergeVOs.Add(new DamageMergeVO("核火球", MyCore.Utils.Translate.Geometry2GeoJson(geom_01), 0, ""));
            damageMergeVOs.Add(new DamageMergeVO("早期核辐射", MyCore.Utils.Translate.Geometry2GeoJson(geom_02), rem, "rem"));
            damageMergeVOs.Add(new DamageMergeVO("冲击波", MyCore.Utils.Translate.Geometry2GeoJson(geom_03), psi, "psi"));
            damageMergeVOs.Add(new DamageMergeVO("光辐射", MyCore.Utils.Translate.Geometry2GeoJson(geom_04), calcm, "cal/cm²"));
            damageMergeVOs.Add(new DamageMergeVO("核电磁脉冲", MyCore.Utils.Translate.Geometry2GeoJson(geom_05), vm, "v/m"));

            return damageMergeVOs;
        }

        #endregion

        private void QueryWeather(double lng,double lat,double alt,ref double wind_speed ,ref double wind_dir )
        {
            //天气接口
            string url = Configuration["ServiceUrls:Weather"];//https://localhost:5001/weather

            var timeUtc = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

            WeatherBO weatherBO = new WeatherBO(lng, lat, alt, timeUtc);
            string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(weatherBO);

            try
            {
                Task<string> s = MyCore.Utils.HttpCli.PostAsyncJson(url, postBody);
                s.Wait();
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
            }
            catch (Exception)
            {

            }
        }


        internal class LimitUnit
        {
            public LimitUnit(string unit, double limit)
            {
                this.unit = unit;
                this.limit = limit;
            }

            public string unit { get; set; }
            public double limit { get; set; }
        }
        private LimitUnit QueryLimits(string clsName)
        {
            var rule = _mongoService.QueryRule(clsName);
            if (rule != null)
            {
                return new LimitUnit(rule.unit, rule.limits);
            }
            return null;
        }
    }
}
