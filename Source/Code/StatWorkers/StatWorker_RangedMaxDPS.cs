using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedMaxDPS : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
            {
                return req.Def is ThingDef thingDef && thingDef.IsRangedWeapon;
            }
            return false;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            return GetWeaponStats(req).GetRawDPS();
        }

    }
}
