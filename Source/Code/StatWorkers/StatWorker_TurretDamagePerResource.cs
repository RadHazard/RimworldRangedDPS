using System;
using System.Text;
using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretDamagePerResource : StatWorker_TurretDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!base.ShouldShowFor(req)) return false;

            // Don't show resource usage for turrets without fuel
            return GetTurretStats(req).NeedsFuel;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            TurretStats turretStats = GetTurretStats(req);

            float optimalRange = turretStats.FindOptimalRange(turretStats.turret);
            return turretStats.GetAdjustedDamagePerFuel(optimalRange);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            TurretStats weaponStats = GetTurretStats(optionalReq);
            int optimalRange = (int)weaponStats.FindOptimalRange(weaponStats.turret);

            return string.Format("{0} ({1})",
                value.ToStringByStyle(stat.toStringStyle, numberSense),
                string.Format("StatsReport_RangedDPSOptimalRange".Translate(), optimalRange));
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
            {
                return "";
            }

            TurretStats turretStats = GetTurretStats(req);
            return FuelRangeBreakdown(turretStats);
        }

        /// <summary>
        /// Returns a string that provides a breakdown of both accuracy and fuel usage over the full range of the given
        /// weapon.
        /// </summary>
        /// <returns>A string providing a breakdown of the fuel usage of the given turret at various ranges.</returns>
        /// <param name="turretStats">The turret to caluclate a breakdown for.</param>
        protected static string FuelRangeBreakdown(TurretStats turretStats)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSResourceAccuracy".Translate());

            // Min Range
            float minRange = Math.Max(turretStats.MinRange, 1f);
            float minRangeHitChance = turretStats.GetAdjustedHitChanceFactor(minRange, turretStats.turret);
            float minRangeDPF = turretStats.GetAdjustedDamagePerFuel(minRange);
            stringBuilder.AppendLine(FormatValueRangeString(minRange, minRangeDPF, minRangeHitChance));

            // Ranges between Min - Max, in steps of 5
            float startRange = (float)Math.Ceiling(minRange / 5) * 5;
            for (float range = startRange; range < turretStats.MaxRange; range += 5)
            {
                float hitChance = turretStats.GetAdjustedHitChanceFactor(range, turretStats.turret);
                float dpf = turretStats.GetAdjustedDamagePerFuel(range);
                stringBuilder.AppendLine(FormatValueRangeString(range, dpf, hitChance));
            }

            // Max Range
            float maxRangeHitChance = turretStats.GetAdjustedHitChanceFactor(turretStats.MaxRange, turretStats.turret);
            float maxRangeDPF = turretStats.GetAdjustedDamagePerFuel(turretStats.MaxRange);
            stringBuilder.AppendLine(FormatValueRangeString(turretStats.MaxRange, maxRangeDPF, maxRangeHitChance));

            return stringBuilder.ToString();
        }
    }
}