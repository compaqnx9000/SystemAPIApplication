using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.Services;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Controllers
{
    [ApiController]
    public class FireBallController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;

        public FireBallController(IGeometryAnalysisService geometryAnalysisService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));
        }
        [HttpGet("fireball/merge")]
        public IActionResult FireballMerge()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.FireballMerge()
            });
        }

        [HttpPost("fireball/merge")]
        public IActionResult FireballMerge([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.FireballMerge(bo)
            });
        }

        [HttpGet("fireball/multi")]
        public IActionResult FireballMulti()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.FireballMulti()
            }); ;
        }

        [HttpPost("fireball/multi")]
        public IActionResult FireballMulti([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.FireballMulti(bo)
            });
        }
    }
}
