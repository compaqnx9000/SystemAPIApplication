using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
    public class NuclearPulseController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;
        private readonly IMongoService _mongoService;

        public NuclearPulseController(IGeometryAnalysisService geometryAnalysisService,
            IMongoService mongoService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));

            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }
        [HttpGet("nuclearpulse/merge")]
        public IActionResult NuclearpulseMerge()
        {
            // 读取mongo数据库中HB库，用于仿真模拟
            Geometry geom = null;
            List<BsonDocument> result = _mongoService.QueryAll ();
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
                    geom = _geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt);
                else
                    geom = geom.Union(_geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = Translate.Geometry2GeoJson(geom)
            });
        }
        [HttpPost("nuclearpulse/merge")]
        public IActionResult NuclearpulseMerge([FromBody] string[] bo)
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

                // 核电磁脉冲的当量就是“吨”，这里就不需要变成“千吨”了

                // 爆高需要传入km，所以要除以1000.
                alt /= 1000;

                if (geom == null)
                    geom = _geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt);
                else
                    geom = geom.Union(_geometryAnalysisService.GetNuclearPulseGeometry(lon, lat, yield, alt));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = Translate.Geometry2GeoJson(geom)
            });
        }
        [HttpGet("nuclearpulse/multi")]
        public IActionResult NuclearpulseMulti()
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

                

                // 核电磁脉冲的当量就是“吨”，这里就不需要变成“千吨”了,但是高度要变成km
                double r = _geometryAnalysisService.GetNuclearPulseRadius(yield, alt/1000.0);

                // 返回值的alt单位是：米,所以要把 r 乘以 1000
                multis.Add(new MultiVO(id, Math.Round(r*1000, 2), lon, lat, alt));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = multis
            });
        }
        [HttpPost("nuclearpulse/multi")]
        public IActionResult NuclearpulseMulti([FromBody] string[] bo)
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

                // 核电磁脉冲的当量就是“吨”，这里就不需要变成“千吨”了,但是高度要变成km
                double r = _geometryAnalysisService.GetNuclearPulseRadius(yield, alt/1000.0);

                // 返回值的alt单位是：米,所以要把 r 乘以 1000
                multis.Add(new MultiVO(id, Math.Round(r * 1000, 2), lon, lat, alt));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = multis
            });
        }

    }
}
