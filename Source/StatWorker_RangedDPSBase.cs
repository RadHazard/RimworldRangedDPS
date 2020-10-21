﻿using System;
using System.Text;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedDPSBase : StatWorker
    {
        protected static RangedWeaponStats GetWeaponStats(StatRequest req)
        {
            Thing weapon = req.Thing ?? (req.Def as ThingDef)?.GetConcreteExample();
            if (weapon == null) Log.Error($"[RangedDPS] Could not find a valid weapon thing when trying to caluculate the stat for {req.Def.defName}");
            return GetWeaponStats(weapon);
        }

        /// <summary>
        /// Calculates a stats breakdown of the given ranged weapon.
        /// Logs an error and returns null if the thing is null or not a ranged weapon.
        /// </summary>
        /// <returns>The stats of the passed-in weapon.</returns>
        /// <param name="weapon">The turret to get stats for.</param>
        protected static RangedWeaponStats GetWeaponStats(Thing weapon)
        {
            if (weapon == null)
            {
                Log.Error($"[RangedDPS] Tried to get the ranged weapon stats of a null weapon");
                return null;
            }
            if (!weapon.def.IsRangedWeapon)
            {
                Log.Error($"[RangedDPS] Tried to get the ranged weapon stats of {weapon.def.defName}, which is not a ranged weapon");
                return null;
            }

            return new RangedWeaponStats(weapon);
        }

        /// <summary>
        /// Returns a string that provides a breakdown of both accuracy and DPS over the full range of the given weapon.
        /// Shooter is optional, and if provided the DPS is adjusted to account for the shooter's shooting accuracy.
        /// </summary>
        /// <returns>A string providing a breakdown of the performance of the given weapon at various ranges.</returns>
        /// <param name="gun">The gun to caluclate a breakdown for.</param>
        /// <param name="shooter">(Optional) The shooter (pawn or turret) using the weapon.</param>
        protected static string DPSRangeBreakdown(RangedWeaponStats gun, Thing shooter = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            // Min Range
            float minRange = Math.Max(gun.MinRange, 1f);
            float minRangeHitChance = gun.GetAdjustedHitChanceFactor(minRange, shooter);
            float minRangeDps = gun.GetAdjustedDPS(minRange, shooter);
            stringBuilder.AppendLine(FormatDPSRangeString(minRange, minRangeDps, minRangeHitChance));

            // Ranges between Min - Max, in steps of 5
            float startRange = (float)Math.Ceiling(minRange / 5) * 5;
            for (float range = startRange; range < gun.MaxRange; range += 5)
            {
                float hitChance = gun.GetAdjustedHitChanceFactor(range, shooter);
                float dps = gun.GetAdjustedDPS(range, shooter);
                stringBuilder.AppendLine(FormatDPSRangeString(range, dps, hitChance));
            }

            // Max Range
            float maxRangeHitChance = gun.GetAdjustedHitChanceFactor(gun.MaxRange, shooter);
            float maxRangeDps = gun.GetAdjustedDPS(gun.MaxRange, shooter);
            stringBuilder.AppendLine(FormatDPSRangeString(gun.MaxRange, maxRangeDps, maxRangeHitChance));

            return stringBuilder.ToString();
        }

        protected static string FormatDPSRangeString(float range, float dps, float hitChance)
        {
            return string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    range, dps, hitChance);
        }

    }
}
