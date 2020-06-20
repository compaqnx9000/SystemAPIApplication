using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using SystemAPIApplication.core;
using SystemAPIApplication.enums;
using SystemAPIApplication.Utils;

namespace SystemAPIApplication.Services
{
    public class GeometryAnalysisService : IGeometryAnalysisService
    {
        const double M2FT = 3.2808399;

        /*  
         *  火球  
         */
        public Geometry GetFireBallGeometry(double lon, double lat, double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            double r =  myAnalyse.CalcfireBallRadius(yield, alt > 0);

            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);


            return geom;
        }
        public double GetFireBallRadius(double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcfireBallRadius(yield, alt > 0);
        }

        /* 
         * 核辐射 
         */

        public Geometry GetNuclearRadiationGeometry(double lon, double lat, double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();

            double r = myAnalyse.CalcNuclearRadiationRadius(yield, alt, Utils.Helpers.Convert.ToRem(1));

            //根据(lng,lat,R)生成Geometry
            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);

            return geom;
        }

        public double GetNuclearRadiationRadius(double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcNuclearRadiationRadius(yield, alt,
                Utils.Helpers.Convert.ToRem(1));
        }

        /* 
         * 冲击波 
         */
        public Geometry GetShockWaveGeometry(double lon, double lat, double yield, double alt)
        {

            MyAnalyse myAnalyse = new MyAnalyse();

            // 求核爆影响范围半径[54609,21249,10101,3734,1537]
            double r = myAnalyse.CalcShockWaveRadius(yield, alt,Utils.Helpers.Convert.ToPsi(1));

            //根据(lng,lat,R)生成Geometry
            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);
            return geom;
        }

        public double GetShockWaveRadius(double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcShockWaveRadius(yield,alt,
                Utils.Helpers.Convert.ToPsi(1));
        }

        /* 
         * 光、热辐射 
         */
        public Geometry GetThermalRadiationGeometry(double lon, double lat, double yield, double alt)
        {

            MyAnalyse myAnalyse = new MyAnalyse();

            double r = myAnalyse.CalcThermalRadiationRadius(yield, lat, Utils.Helpers.Convert.ToThrem(1));

            //根据(lng,lat,R)生成Geometry
            Geometry geom = Translate.BuildCircle(lon, lat, r / 1000.0, 50);

            return geom;
        }
        public double GetThermalRadiationRadius(double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcThermalRadiationRadius(yield, alt, Utils.Helpers.Convert.ToThrem(1));
        }

        /* 
        * 核电磁脉冲 
        */
        public Geometry GetNuclearPulseGeometry(double lon, double lat, double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();

            double r = myAnalyse.CalcNuclearPulseRadius(yield, alt, Utils.Helpers.Convert.ToPluse(1));

            //根据(lng,lat,R)生成Geometry。
            // 注意，因为r返回的就是千米，而BuildCircle，需要传入的也是千米，所以就不用除以1000了
            Geometry geom = Translate.BuildCircle(lon, lat, r, 50);
            return geom;
        }
        public double GetNuclearPulseRadius(double yield, double alt)
        {
            MyAnalyse myAnalyse = new MyAnalyse();
            return myAnalyse.CalcNuclearPulseRadius(yield, alt, Utils.Helpers.Convert.ToPluse(1));
        }

        /*
         * 核沉降
         */
        public Geometry GetFalloutGeometry(double lon, double lat, double yield, double alt, double wind_speed, double wind_dir)
        {
            MyAnalyse myAnalyse = new MyAnalyse();

            List<Coor> coors = myAnalyse.CalcRadioactiveFalloutRegion(
                lon, lat, alt, yield, wind_speed, wind_dir, DamageEnumeration.Light);

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
