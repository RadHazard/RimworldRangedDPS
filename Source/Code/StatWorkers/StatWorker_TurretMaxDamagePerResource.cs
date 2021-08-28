using System;
using RangedDPS.StatUtilities;
using RimWorld;

namespace RangedDPS
{

    public class StatWorker_TurretMaxDamagePerResource : StatWorker_TurretDPSBase
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
            return turretStats.DamagePerFuel;
        }
    }
}