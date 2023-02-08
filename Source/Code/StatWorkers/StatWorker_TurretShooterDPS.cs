using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace RangedDPS.StatWorkers
{
    [UsedImplicitly]
    public class StatWorker_TurretShooterDPS : StatWorker_TurretDPSBase
    {

        public override bool IsDisabledFor(Thing thing)
        {
            if (!base.IsDisabledFor(thing))
                return StatDefOf.ShootingAccuracyTurret.Worker.IsDisabledFor(thing);
            
            return true;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
                return 0f;

            var turretStats = GetTurretStats(req);

            var optimalRange = turretStats.FindTurretOptimalRange();
            return turretStats.GetTurretAdjustedDPS(optimalRange);
        }

        public override string GetStatDrawEntryLabel(StatDef statDef, float value, ToStringNumberSense numberSense,
            StatRequest optionalReq, bool finalized = true)
        {
            var turretStats = GetTurretStats(optionalReq);

            var optimalRange = Mathf.RoundToInt(turretStats.FindTurretOptimalRange());

            return string.Format("{0} ({1})",
                value.ToStringByStyle(statDef.toStringStyle, numberSense),
                string.Format("StatsReport_RangedDPSOptimalRange".Translate(), optimalRange));
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
                return "";

            var turretStats = GetTurretStats(req);

            return DPSRangeBreakdown(turretStats, turretStats.Shooter);
        }

    }
}
