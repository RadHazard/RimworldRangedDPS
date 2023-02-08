using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace RangedDPS.StatWorkers
{
    [UsedImplicitly]
    public class StatWorker_RangedMaxDPS : StatWorker_RangedDPSBase
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
                return 0f;

            return GetWeaponStats(req).GetRawDPS();
        }

    }
}
