using System;
using RangedDPS.StatUtilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretShooterDPS : StatWorker_TurretDPSBase
    {

        public override bool IsDisabledFor(Thing thing)
        {
            if (!base.IsDisabledFor(thing))
            {
                return StatDefOf.ShootingAccuracyTurret.Worker.IsDisabledFor(thing);
            }
            return true;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Building_TurretGun turret = GetTurret(req);
            TurretGunStats turretStats = GetTurretStats(req);

            float optimalRange = turretStats.FindTurretOptimalRange();
            return turretStats.GetTurretAdjustedDPS(optimalRange);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            Building_TurretGun turret = GetTurret(optionalReq);
            TurretGunStats turretStats = GetTurretStats(optionalReq);

            int optimalRange = Mathf.RoundToInt(turretStats.FindTurretOptimalRange());

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

            Building_TurretGun turret = GetTurret(req);
            TurretGunStats turretStats = GetTurretStats(req);

            return DPSRangeBreakdown(turretStats, turretStats.Shooter);
        }

    }
}
