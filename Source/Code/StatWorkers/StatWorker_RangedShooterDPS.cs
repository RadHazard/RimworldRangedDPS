using System;
using System.Collections.Generic;
using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedShooterDPS : StatWorker_RangedDPSBase
    {

        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
            {
                return req.Thing is Pawn pawn && (GetPawnWeapon(pawn)?.def?.IsRangedWeapon ?? false);
            }
            return false;
        }

        public override bool IsDisabledFor(Thing thing)
        {
            if (!base.IsDisabledFor(thing))
            {
                return StatDefOf.ShootingAccuracyPawn.Worker.IsDisabledFor(thing);
            }
            return true;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Pawn pawn = req.Thing as Pawn;
            RangedWeaponStats weaponStats = GetWeaponStats(GetPawnWeapon(pawn));

            float optimalRange = weaponStats.FindOptimalRange(pawn);
            return weaponStats.GetAdjustedDPS(optimalRange, pawn);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            Pawn pawn = optionalReq.Thing as Pawn;
            RangedWeaponStats weaponStats = GetWeaponStats(GetPawnWeapon(pawn));

            int optimalRange = (int)weaponStats.FindOptimalRange(pawn);

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

            Pawn pawn = req.Thing as Pawn;
            RangedWeaponStats weaponStats = GetWeaponStats(GetPawnWeapon(pawn));
            return DPSRangeBreakdown(weaponStats, pawn);
        }

        public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
        {
            if (statRequest.Thing is Pawn pawn && pawn?.equipment?.Primary != null)
            {
                yield return new Dialog_InfoCard.Hyperlink(pawn.equipment.Primary, -1);
            }
        }

        protected static Thing GetPawnWeapon(Pawn pawn)
        {
            return pawn?.equipment?.Primary;
        }
    }
}
