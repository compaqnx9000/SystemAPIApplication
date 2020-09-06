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
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Controllers
{
    [ApiController]
    public class FalloutController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;

        public FalloutController(IGeometryAnalysisService geometryAnalysisService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));

        }
        [HttpGet("fallout/merge")]
        public IActionResult FalloutMerge()
        {
            var geomvo = _geometryAnalysisService.FalloutMerge();

            return new JsonResult(new
            {
                return_status = (geomvo == null?1:0),
                return_msg = geomvo == null ? "当量小于1000吨，无法计算" : "",
                return_data = geomvo
            });
        }
        [HttpPost("fallout/merge")]
        public IActionResult FalloutMerge([FromBody] string[] bo)
        {
            var geomvo = _geometryAnalysisService.FalloutMerge(bo);

            return new JsonResult(new
            {
                return_status = (geomvo == null ? 1 : 0),
                return_msg = geomvo == null ? "当量小于1000吨，无法计算" : "",
                return_data = geomvo
            });
        }
        [HttpGet("fallout/multi")]
        public IActionResult FalloutMulti()
        {
            var vos = _geometryAnalysisService.FalloutMulti();
            return new JsonResult(new
            {
                return_status = (vos.Count()<=0?1:0),
                return_msg = vos.Count() <= 0 ? "当量小于1000吨，无法计算":"",
                return_data = vos
            });
        }
        [HttpPost("fallout/multi")]
        public IActionResult FalloutMulti([FromBody] string[] bo)
        {
            var vos = _geometryAnalysisService.FalloutMulti(bo);
            return new JsonResult(new
            {
                return_status = (vos.Count() <= 0 ? 1 : 0),
                return_msg = vos.Count() <= 0 ? "当量小于1000吨，无法计算" : "",
                return_data = vos
            });
        }
    }
}
