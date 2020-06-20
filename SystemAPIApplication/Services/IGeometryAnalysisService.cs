using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.enums;

namespace SystemAPIApplication.Services
{
    public interface IGeometryAnalysisService
    {
        // 火球
        double GetFireBallRadius(double yield, double alt);
        Geometry GetFireBallGeometry(double lon,double lat,double yield,double alt);

        // 核辐射
        Geometry GetNuclearRadiationGeometry(double lon, double lat, double yield, double alt);
        double GetNuclearRadiationRadius(double yield, double alt);

        // 冲击波
        Geometry GetShockWaveGeometry(double lon, double lat, double yield, double alt);

        double GetShockWaveRadius(double yield, double alt);

        // 热、光辐射
        Geometry GetThermalRadiationGeometry(double lon, double lat, double yield, double alt);
        double GetThermalRadiationRadius(double yield, double alt);

        // 核电磁脉冲
        Geometry GetNuclearPulseGeometry(double lon, double lat, double yield, double alt);
        double GetNuclearPulseRadius(double yield, double alt);

        // 核沉降
        Geometry GetFalloutGeometry(double lon, double lat, double yield, double alt
            , double wind_speed, double wind_dir);


        
    }
}
