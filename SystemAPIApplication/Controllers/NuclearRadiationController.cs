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
    public class NuclearRadiationController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;
        private readonly IMongoService _mongoService;

        public NuclearRadiationController(IGeometryAnalysisService geometryAnalysisService,
            IMongoService mongoService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));

            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }
        [HttpGet("nuclearradiation/merge")]
        public IActionResult NuclearradiationMerge()
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
                    geom = _geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield, alt);
                else
                    geom = geom.Union(_geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield, alt));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = Translate.Geometry2GeoJson(geom)
            });
        }
        [HttpPost("nuclearradiation/merge")]
        public IActionResult NuclearradiationMerge([FromBody] string[] bo)
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
                    geom = _geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield, alt);
                else
                    geom = geom.Union(_geometryAnalysisService.GetNuclearRadiationGeometry(lon, lat, yield, alt));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = Translate.Geometry2GeoJson(geom)
            });
        }
        [HttpGet("nuclearradiation/multi")]
        public IActionResult NuclearradiationMulti()
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
                double r = _geometryAnalysisService.GetNuclearRadiationRadius(yield/1000, alt* 3.2808399);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = multis
            });
        }
        [HttpPost("nuclearradiation/multi")]
        public IActionResult NuclearradiationMulti([FromBody] string[] bo)
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
                double r = _geometryAnalysisService.GetNuclearRadiationRadius(yield/1000, alt* 3.2808399);

                // 返回值的alt单位是：米
                multis.Add(new MultiVO(id, Math.Round(r, 2), lon, lat, alt));
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
