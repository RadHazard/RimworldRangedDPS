using System;
using System.Linq;
using JetBrains.Annotations;
using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS.StatWorkers
{
    [UsedImplicitly]
    public class StatWorker_TurretWeaponDPS : StatWorker_TurretDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
                return 0f;

            var weaponStats = GetTurretStats(req);

            var bestAccuracy = new[] {
                weaponStats.AccuracyTouch,
                weaponStats.AccuracyShort,
                weaponStats.AccuracyMedium,
                weaponStats.AccuracyLong
            }.Max();

            return weaponStats.GetRawDPS() * Math.Min(bestAccuracy, 1f);
        }

        public override string GetStatDrawEntryLabel(StatDef statDef, float value, ToStringNumberSense numberSense,
            StatRequest optionalReq, bool finalized = true)
        {
            RangedWeaponStats weaponStats = GetTurretStats(optionalReq);

            var optimalRange = new[] {
                (weaponStats.AccuracyTouch, (int) ShootTuning.DistTouch),
                (weaponStats.AccuracyShort, (int) ShootTuning.DistShort),
                (weaponStats.AccuracyMedium, (int) ShootTuning.DistMedium),
                (weaponStats.AccuracyLong, (int) ShootTuning.DistLong)
            }.MaxBy(acc => acc.Item1);

            return string.Format("{0} ({1})",
                value.ToStringByStyle(statDef.toStringStyle, numberSense),
                string.Format("StatsReport_RangedDPSOptimalRange".Translate(), optimalRange.Item2));
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
                return "";

            var weaponStats = GetTurretStats(req);
            return DPSRangeBreakdown(weaponStats);
        }

    }
}
