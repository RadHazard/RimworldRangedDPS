﻿using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedDPSBase : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!(req.Def is ThingDef thingDef))
            {
                return false;
            }

            var shootVerb = GetShootVerb(thingDef);
            return shootVerb != null;
        }

        protected static VerbProperties GetShootVerb(ThingDef thingDef)
        {
            // Note - the game uses the first shoot verb and ignores the rest for whatever reason.  Do the same here
            return (from v in thingDef.Verbs
                    where !v.IsMeleeAttack
                    select v).FirstOrDefault();
        }

        protected float GetRawDPS(VerbProperties shootVerb, Thing thing)
        {
            float fullCycleTime = shootVerb.warmupTime + thing.GetStatValue(StatDefOf.RangedWeapon_Cooldown, true)
                    + ((shootVerb.burstShotCount - 1) * shootVerb.ticksBetweenBurstShots).TicksToSeconds();

            int totalDamage = shootVerb.burstShotCount * shootVerb.defaultProjectile.projectile.GetDamageAmount(thing);

            return totalDamage / fullCycleTime;
        }

    }
}
