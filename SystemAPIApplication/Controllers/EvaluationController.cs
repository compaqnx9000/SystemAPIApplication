using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.Services;
using SystemAPIApplication.Utils;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Controllers
{
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;
        private readonly IMongoService _mongoService;

        public EvaluationController(IGeometryAnalysisService geometryAnalysisService,
            IMongoService mongoService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));

            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }
        [HttpGet("damage/merge")]
        public IActionResult Merge()
        {
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
                    geom_01 = _geometryAnalysisService.GetFireBallGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_01 = geom_01.Union(_geometryAnalysisService.GetFireBallGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                if (geom_02 == null)
                    geom_02 = _geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_02 = geom_02.Union(_geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                if (geom_03 == null)
                    geom_03 = _geometryAnalysisService.GetShockWaveGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_03 = geom_03.Union(_geometryAnalysisService.GetShockWaveGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                if (geom_04 == null)
                    geom_04 = _geometryAnalysisService.GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_04 = geom_04.Union(_geometryAnalysisService.GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                //吨不变，米变千米
                if (geom_05 == null)
                    geom_05 = _geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0);
                else
                    geom_05 = geom_05.Union(_geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0));


            }

            damageMergeVOs.Add(new DamageMergeVO("H火球", Translate.Geometry2GeoJson(geom_01)));
            damageMergeVOs.Add(new DamageMergeVO("核辐射", Translate.Geometry2GeoJson(geom_02)));
            damageMergeVOs.Add(new DamageMergeVO("冲击波", Translate.Geometry2GeoJson(geom_03)));
            damageMergeVOs.Add(new DamageMergeVO("热/光辐射", Translate.Geometry2GeoJson(geom_04)));
            damageMergeVOs.Add(new DamageMergeVO("电磁脉冲", Translate.Geometry2GeoJson(geom_05)));

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = damageMergeVOs
            });

        }
        [HttpPost("damage/merge")]
        public IActionResult Merge([FromBody] string[] bo)
        {
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
                    geom_01 = _geometryAnalysisService.GetFireBallGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_01 = geom_01.Union(_geometryAnalysisService.GetFireBallGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                if (geom_02 == null)
                    geom_02 = _geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_02 = geom_02.Union(_geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                if (geom_03 == null)
                    geom_03 = _geometryAnalysisService.GetShockWaveGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_03 = geom_03.Union(_geometryAnalysisService.GetShockWaveGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                if (geom_04 == null)
                    geom_04 = _geometryAnalysisService.GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399);
                else
                    geom_04 = geom_04.Union(_geometryAnalysisService.GetThermalRadiationGeometry(lon, lat, yield / 1000, alt * 3.2808399));

                //吨不变，米变千米
                if (geom_05 == null)
                    geom_05 = _geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0);
                else
                    geom_05 = geom_05.Union(_geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt / 1000.0));


            }

            damageMergeVOs.Add(new DamageMergeVO("H火球", Translate.Geometry2GeoJson(geom_01)));
            damageMergeVOs.Add(new DamageMergeVO("核辐射", Translate.Geometry2GeoJson(geom_02)));
            damageMergeVOs.Add(new DamageMergeVO("冲击波", Translate.Geometry2GeoJson(geom_03)));
            damageMergeVOs.Add(new DamageMergeVO("热/光辐射", Translate.Geometry2GeoJson(geom_04)));
            damageMergeVOs.Add(new DamageMergeVO("电磁脉冲", Translate.Geometry2GeoJson(geom_05)));

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = damageMergeVOs
            });

        }
        [HttpGet("damage/multi")]
        public IActionResult Multi()
        {
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
                double r = _geometryAnalysisService.GetFireBallRadius(yield / 1000, alt * 3.2808399);
                multiVOs_01.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                r = _geometryAnalysisService.GetNuclearRadiationRadius(yield / 1000, alt * 3.2808399);
                multiVOs_02.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                r = _geometryAnalysisService.GetShockWaveRadius(yield / 1000, alt * 3.2808399);
                multiVOs_03.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                r = _geometryAnalysisService.GetNuclearRadiationRadius(yield / 1000, alt * 3.2808399);
                multiVOs_04.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                // 吨不变；米变千米
                r = _geometryAnalysisService.GetNuclearPulseRadius(yield, alt / 1000.0);

                // 上一步r的返回值是千米，所以要变成米
                multiVOs_05.Add(new MultiVO(id, Math.Round(r * 1000, 2), lon, lat, alt));

            }
            damageMultiVOs.Add(new DamageMultiVO("H火球", multiVOs_01));
            damageMultiVOs.Add(new DamageMultiVO("核辐射", multiVOs_02));
            damageMultiVOs.Add(new DamageMultiVO("冲击波", multiVOs_03));
            damageMultiVOs.Add(new DamageMultiVO("热/光辐射", multiVOs_04));
            damageMultiVOs.Add(new DamageMultiVO("电磁脉冲", multiVOs_05));


            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = damageMultiVOs
            });
        }
        [HttpPost("damage/multi")]
        public IActionResult Multi([FromBody] string[] bo)
        {
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
                double r = _geometryAnalysisService.GetFireBallRadius(yield / 1000, alt * 3.2808399);
                multiVOs_01.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                r = _geometryAnalysisService.GetNuclearRadiationRadius(yield / 1000, alt * 3.2808399);
                multiVOs_02.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                r = _geometryAnalysisService.GetShockWaveRadius(yield / 1000, alt * 3.2808399);
                multiVOs_03.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                r = _geometryAnalysisService.GetThermalRadiationRadius(yield / 1000, alt * 3.2808399);
                multiVOs_04.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));

                // 吨不变；米变千米
                r = _geometryAnalysisService.GetNuclearPulseRadius(yield, alt / 1000.0);

                // 上一步r的返回值是千米，所以要变成米
                multiVOs_05.Add(new MultiVO(id, Math.Round(r * 1000, 2), lon, lat, alt));

            }
            damageMultiVOs.Add(new DamageMultiVO("H火球", multiVOs_01));
            damageMultiVOs.Add(new DamageMultiVO("核辐射", multiVOs_02));
            damageMultiVOs.Add(new DamageMultiVO("冲击波", multiVOs_03));
            damageMultiVOs.Add(new DamageMultiVO("热/光辐射", multiVOs_04));
            damageMultiVOs.Add(new DamageMultiVO("电磁脉冲", multiVOs_05));


            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = damageMultiVOs
            });
        }
    }
}
