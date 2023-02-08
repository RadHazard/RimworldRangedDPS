using JetBrains.Annotations;
using RimWorld;
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

            var turret = GetTurret(req);
            var weaponStats = GetTurretStats(req);

            var optimalRange = weaponStats.FindOptimalRange(turret);
            return weaponStats.GetAdjustedDPS(optimalRange, turret);
        }

        public override string GetStatDrawEntryLabel(StatDef statDef, float value, ToStringNumberSense numberSense,
            StatRequest optionalReq, bool finalized = true)
        {
            var turret = GetTurret(optionalReq);
            var weaponStats = GetTurretStats(optionalReq);

            var optimalRange = (int)weaponStats.FindOptimalRange(turret);

            return string.Format("{0} ({1})",
                value.ToStringByStyle(statDef.toStringStyle, numberSense),
                string.Format("StatsReport_RangedDPSOptimalRange".Translate(), optimalRange));
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
                return "";

            var turret = GetTurret(req);
            var weaponStats = GetTurretStats(req);

            return DPSRangeBreakdown(weaponStats, turret);
        }

    }
}
