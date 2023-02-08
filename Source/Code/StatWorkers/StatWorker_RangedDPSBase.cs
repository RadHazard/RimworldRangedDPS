using System;
using System.Text;
using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS.StatWorkers
{
    public abstract class StatWorker_RangedDPSBase : StatWorker
    {
        /// <summary>
        /// Calculates a stats breakdown for the given stat request.
        /// Logs an error and returns null if the thing is null or not a ranged weapon.
        /// </summary>
        /// <returns>The stats of the passed-in weapon.</returns>
        /// <param name="req">The request to get stats for.</param>
        protected static GunStats GetWeaponStats(StatRequest req)
        {
            var weapon = req.Thing ?? (req.Def as ThingDef)?.GetConcreteExample();
            if (weapon == null)
            {
                Log.Error($"[RangedDPS] Could not find a valid weapon thing when trying to" +
                          $"calculate the stat for {req.Def.defName}");

                return null!; // TODO
            }
            return GetWeaponStats(weapon);
        }

        /// <summary>
        /// Calculates a stats breakdown of the given ranged weapon.
        /// Logs an error and returns null if the thing is null or not a ranged weapon.
        /// </summary>
        /// <returns>The stats of the passed-in weapon.</returns>
        /// <param name="weapon">The gun to get stats for.</param>
        protected static GunStats GetWeaponStats(Thing? weapon)
        {
            if (weapon == null)
            {
                Log.Error($"[RangedDPS] Tried to get the ranged weapon stats of a null weapon");
                return null!;
            }

            if (!weapon.def.IsRangedWeapon)
            {
                Log.Error($"[RangedDPS] Tried to get the ranged weapon stats of {weapon.def.defName}," +
                          " which is not a ranged weapon");
                return null!;
            }

            return new GunStats(weapon);
        }

        /// <summary>
        /// Returns a string that provides a breakdown of both accuracy and DPS over the full range of the given weapon.
        /// Shooter is optional, and if provided the DPS is adjusted to account for the shooter's shooting accuracy.
        /// </summary>
        /// <returns>A string providing a breakdown of the performance of the given weapon at various ranges.</returns>
        /// <param name="weapon">The weapon to calculate a breakdown for.</param>
        /// <param name="shooter">(Optional) The shooter (pawn or turret) using the weapon.</param>
        protected static string DPSRangeBreakdown(RangedWeaponStats weapon, ShooterStats? shooter = null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            // Min Range
            var minRange = Math.Max(weapon.MinRange, 1f);
            var minRangeHitChance = weapon.GetAdjustedHitChance(minRange, shooter);
            var minRangeDps = weapon.GetAdjustedDPS(minRange, shooter);
            stringBuilder.AppendLine(FormatValueRangeString(minRange, minRangeDps, minRangeHitChance));

            // Ranges between Min - Max, in steps of 5
            var startRange = (float)Math.Ceiling(minRange / 5) * 5;
            for (var range = startRange; range < weapon.MaxRange; range += 5)
            {
                var hitChance = weapon.GetAdjustedHitChance(range, shooter);
                var dps = weapon.GetAdjustedDPS(range, shooter);
                stringBuilder.AppendLine(FormatValueRangeString(range, dps, hitChance));
            }

            // Max Range
            var maxRangeHitChance = weapon.GetAdjustedHitChance(weapon.MaxRange, shooter);
            var maxRangeDps = weapon.GetAdjustedDPS(weapon.MaxRange, shooter);
            stringBuilder.AppendLine(FormatValueRangeString(weapon.MaxRange, maxRangeDps, maxRangeHitChance));

            return stringBuilder.ToString();
        }

        protected static string FormatValueRangeString(float range, float value, float hitChance)
        {
            return string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    range, value, hitChance);
        }

    }
}
