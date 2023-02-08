using System;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace RangedDPS.StatWorkers
{
    [UsedImplicitly]
    public class StatWorker_RangedWeaponDPS : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
                return req.Def is ThingDef { IsRangedWeapon: true };
            
            return false;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            var weaponStats = GetWeaponStats(req);

            var bestAccuracy = new[] {
                weaponStats.AccuracyTouch,
                weaponStats.AccuracyShort,
                weaponStats.AccuracyMedium,
                weaponStats.AccuracyLong
            }.Max();

            return weaponStats.GetRawDPS() * Math.Min(bestAccuracy, 1);
        }

        public override string GetStatDrawEntryLabel(StatDef statDef, float value, ToStringNumberSense numberSense,
            StatRequest optionalReq, bool finalized = true)
        {
            var weaponStats = GetWeaponStats(optionalReq);

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

            return DPSRangeBreakdown(GetWeaponStats(req));
        }

    }
}
