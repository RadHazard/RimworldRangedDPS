using System.Collections.Generic;
using JetBrains.Annotations;
using RangedDPS.StatUtilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace RangedDPS.StatWorkers
{
    [UsedImplicitly]
    public class StatWorker_RangedShooterDPS : StatWorker_RangedDPSBase
    {

        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
                return req.Thing is Pawn pawn && (GetPawnWeapon(pawn)?.def?.IsRangedWeapon ?? false);
            
            return false;
        }

        public override bool IsDisabledFor(Thing thing)
        {
            if (!base.IsDisabledFor(thing))
                return StatDefOf.ShootingAccuracyPawn.Worker.IsDisabledFor(thing);
            
            return true;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
                return 0f;

            var pawn = (req.Thing as Pawn)!; // ShouldShowFor ensures this is a valid cast
            var pawnStats = new PawnShooterStats(pawn);
            var weaponStats = GetWeaponStats(GetPawnWeapon(pawn));

            var optimalRange = weaponStats.FindOptimalRange(pawnStats);
            return weaponStats.GetAdjustedDPS(optimalRange, pawnStats);
        }

        public override string GetStatDrawEntryLabel(StatDef statDef, float value, ToStringNumberSense numberSense,
            StatRequest optionalReq, bool finalized = true)
        {
            if (!ShouldShowFor(optionalReq))
                return "";
            
            var pawn = (optionalReq.Thing as Pawn)!; // ShouldShowFor ensures this is a valid cast
            var pawnStats = new PawnShooterStats(pawn);
            var weaponStats = GetWeaponStats(GetPawnWeapon(pawn));

            var optimalRange = Mathf.RoundToInt(weaponStats.FindOptimalRange(pawnStats));

            return string.Format("{0} ({1})",
                value.ToStringByStyle(statDef.toStringStyle, numberSense),
                string.Format("StatsReport_RangedDPSOptimalRange".Translate(), optimalRange));
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
                return "";

            var pawn = (req.Thing as Pawn)!; // ShouldShowFor ensures this is a valid cast
            var pawnStats = new PawnShooterStats(pawn);
            var weaponStats = GetWeaponStats(GetPawnWeapon(pawn));
            return DPSRangeBreakdown(weaponStats, pawnStats);
        }

        public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
        {
            if (statRequest.Thing is Pawn pawn && pawn.equipment?.Primary != null)
                yield return new Dialog_InfoCard.Hyperlink(pawn.equipment.Primary);
        }

        protected static Thing? GetPawnWeapon(Pawn? pawn)
        {
            return pawn?.equipment?.Primary;
        }
    }
}
