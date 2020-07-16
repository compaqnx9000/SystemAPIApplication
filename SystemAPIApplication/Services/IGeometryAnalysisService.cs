using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Services
{
    public interface IGeometryAnalysisService
    {
        //核火球
        List<MultiVO> FireballMulti();
        List<MultiVO> FireballMulti(string[] bo);
        GeometryVO FireballMerge();
        GeometryVO FireballMerge(string[] bo);

        // 核辐射
        List<MultiVO> NuclearradiationMulti();
        List<MultiVO> NuclearradiationMulti(string[] bo);
        GeometryVO NuclearradiationMerge();
        GeometryVO NuclearradiationMerge(string[] bo);

        // 冲击波
        List<MultiVO> AirblastMulti();
        List<MultiVO> AirblastMulti(string[] bo);
        GeometryVO AirblastMerge();
        GeometryVO AirblastMerge(string[] bo);

        // 光辐射
        List<MultiVO> ThermalradiationMulti();
        List<MultiVO> ThermalradiationMulti(string[] bo);
        GeometryVO ThermalradiationMerge();
        GeometryVO ThermalradiationMerge(string[] bo);

        // 核电磁脉冲
        List<MultiVO> NuclearpulseMulti();
        List<MultiVO> NuclearpulseMulti(string[] bo);
        GeometryVO NuclearpulseMerge();
        GeometryVO NuclearpulseMerge(string[] bo);

        // 核沉降
        List<FalloutVO> FalloutMulti();
        List<FalloutVO> FalloutMulti(string[] bo);
        GeometryMergeVO FalloutMerge();
        GeometryMergeVO FalloutMerge(string[] bo);

        // 全部融合
        List<DamageMultiVO> Multi();
        List<DamageMultiVO> Multi(string[] bo);
        List<DamageMergeVO>  Merge();
        List<DamageMergeVO>  Merge(string[] bo);


        
    }
}
