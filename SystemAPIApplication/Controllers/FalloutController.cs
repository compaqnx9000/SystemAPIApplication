using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SystemAPIApplication.bo;
using SystemAPIApplication.Services;
using SystemAPIApplication.Utils;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Controllers
{
    [ApiController]
    public class FalloutController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;
        private readonly IMongoService _mongoService;
        private ServiceUrls _config;

        private const double m2ft = 3.28084;

        public FalloutController(IGeometryAnalysisService geometryAnalysisService,
            IMongoService mongoService, IOptions<ServiceUrls> options)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));

            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));

            _config = options.Value;

        }
        [HttpGet("fallout/merge")]
        public IActionResult FalloutMerge()
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
                        Task<string> s = PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    if (geom == null)
                        geom = _geometryAnalysisService.GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);
                    else
                        geom = geom.Union(_geometryAnalysisService.GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir));

                }
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = new
                {
                    damageGeometry = Translate.Geometry2GeoJson(geom),
                    radValue = 1
                }
            });
        }
        [HttpPost("fallout/merge")]
        public IActionResult FalloutMerge([FromBody] string[] bo)
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
                        Task<string> s = PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    if (geom == null)
                        geom = _geometryAnalysisService.GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);
                    else
                        geom = geom.Union(_geometryAnalysisService.GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir));

                }
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = new
                {
                    damageGeometry = Translate.Geometry2GeoJson(geom),
                    radValue = 1
                }
            });
        }
        [HttpGet("fallout/multi")]
        public IActionResult FalloutMulti()
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
                        Task<string> s = PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    Geometry geom = _geometryAnalysisService.GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);

                    multis.Add(new FalloutVO(id, Translate.Geometry2GeoJson(geom), 1));
                }

            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = multis
            });
        }
        [HttpPost("fallout/multi")]
        public IActionResult FalloutMulti([FromBody] string[] bo)
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
                        Task<string> s = PostAsyncJson(url, postBody);
                        s.Wait();
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);

                        wind_speed = Double.Parse(jo["return_data"]["wind_speed"].ToString());
                        wind_dir = Double.Parse(jo["return_data"]["wind_dir"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    Geometry geom = _geometryAnalysisService.GetFalloutGeometry(lon, lat, yield, alt, wind_speed, wind_dir);

                    multis.Add(new FalloutVO(id, Translate.Geometry2GeoJson(geom), 1));

                }
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = multis
            });
        }

        public static async Task<string> PostAsyncJson(string url, string json)
        {
            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
