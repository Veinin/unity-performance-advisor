using System;
using System.Linq;
using System.Collections.Generic;

namespace UPA
{
    /// <summary>
    /// 粒子系统分析规则
    /// </summary>
    [Serializable]
    public class ParticleSystemResult
    {
        public string path;
        public List<float> frameTimes;
        public List<int> particleCompCounts;
        public List<int> particleCounts;
        public List<int> textureUseages;
        public List<int> memoryUsages;
        public List<int> drawCalls;
        public PixelDraws pixelDraws;
        public Summary summary;

        public ParticleSystemResult()
        {
            frameTimes = new List<float>();
            particleCompCounts = new List<int>();
            particleCounts = new List<int>();
            memoryUsages = new List<int>();
            textureUseages = new List<int>();
            drawCalls = new List<int>();
            pixelDraws = new PixelDraws();
            summary = new Summary();
        }

        public void Summarize()
        {
            if (drawCalls.Count == 0)
            {
                return;
            }

            summary.drawCallAvg = drawCalls.Average();
            summary.drawCallMax = drawCalls.Max();
            
            summary.frameTimeAvg = frameTimes.Average();
            summary.frameTimeMax = frameTimes.Max();

            summary.textureUseageMax = textureUseages.Max();
            
            summary.memoryUsageAvg = memoryUsages.Average();
            summary.memoryUsageMax = memoryUsages.Max();

            summary.particleCompMax = particleCompCounts.Max();
            
            summary.particleCountAvg = particleCounts.Average();
            summary.particleCountMax = particleCounts.Max();

            summary.overDrawAvg = pixelDraws.GetPixelRate();
            summary.overDrawMax = pixelDraws.GetMaxPixelRate();
        }
    }

    [Serializable]
    public class PixelDraws
    {
        public List<int> pixelTotals;
        public List<int> pixelActualTotals;

        public PixelDraws()
        {
            pixelTotals = new List<int>();
            pixelActualTotals = new List<int>();
        }

        public void UpdateData(int pixelTotal, int pixelActualTotal)
        {
            if (pixelTotal == 0 && pixelActualTotal == 0)
            {
                return;
            }

            pixelTotals.Add(pixelTotal);
            pixelActualTotals.Add(pixelActualTotal);
        }

        public double GetMaxPixelRate()
        {
            double maxRate = 0;

            for (var i = 0; i < pixelTotals.Count; i++)
            {
                var pixel = pixelTotals[i];
                var pixelActual = pixelActualTotals[i];

                if (pixel == 0)
                {
                    continue;
                }

                var rate = pixelActual * 1f / pixel;
                if (rate > maxRate)
                {
                    maxRate = rate;
                }
            }

            return maxRate;
        }

        public double GetPixelRate()
        {
            var pixelAvg = pixelTotals.Average();
            var pixelActualAvg = pixelActualTotals.Average();

            if (pixelAvg == 0)
            {
                return 0;
            }

            return pixelActualAvg * 1f / pixelAvg;
        }
    }

    /// <summary>
    /// 粒子系统分析结论
    /// </summary>
    [Serializable]
    public class Summary
    {
        public double frameTimeAvg;
        public double frameTimeMax;
        public double particleCompMax;
        public double particleCountAvg;
        public double particleCountMax;
        public double textureUseageMax;
        public double memoryUsageAvg;
        public double memoryUsageMax;
        public double drawCallAvg;
        public double drawCallMax;
        public double overDrawAvg;
        public double overDrawMax;
    }
}