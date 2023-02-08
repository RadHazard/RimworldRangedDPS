using System;
using System.Text;
using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS.StatWorkers
{
    public abstract class StatWorker_RangedDPSBase : StatWorker
    {
        protected static RangedWeaponStats GetWeaponStats(StatRequest req)
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
        /// <param name="weapon">The turret to get stats for.</param>
        protected static RangedWeaponStats GetWeaponStats(Thing? weapon)
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

            return new RangedWeaponStats(weapon);
        }

        /// <summary>
        /// Returns a string that provides a breakdown of both accuracy and DPS over the full range of the given weapon.
        /// Shooter is optional, and if provided the DPS is adjusted to account for the shooter's shooting accuracy.
        /// </summary>
        /// <returns>A string providing a breakdown of the performance of the given weapon at various ranges.</returns>
        /// <param name="gun">The gun to calculate a breakdown for.</param>
        /// <param name="shooter">(Optional) The shooter (pawn or turret) using the weapon.</param>
        protected static string DPSRangeBreakdown(RangedWeaponStats gun, Thing? shooter = null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            // Min Range
            var minRange = Math.Max(gun.MinRange, 1f);
            var minRangeHitChance = gun.GetAdjustedHitChanceFactor(minRange, shooter);
            var minRangeDps = gun.GetAdjustedDPS(minRange, shooter);
            stringBuilder.AppendLine(FormatValueRangeString(minRange, minRangeDps, minRangeHitChance));

            // Ranges between Min - Max, in steps of 5
            var startRange = (float)Math.Ceiling(minRange / 5) * 5;
            for (var range = startRange; range < gun.MaxRange; range += 5)
            {
                var hitChance = gun.GetAdjustedHitChanceFactor(range, shooter);
                var dps = gun.GetAdjustedDPS(range, shooter);
                stringBuilder.AppendLine(FormatValueRangeString(range, dps, hitChance));
            }

            // Max Range
            var maxRangeHitChance = gun.GetAdjustedHitChanceFactor(gun.MaxRange, shooter);
            var maxRangeDps = gun.GetAdjustedDPS(gun.MaxRange, shooter);
            stringBuilder.AppendLine(FormatValueRangeString(gun.MaxRange, maxRangeDps, maxRangeHitChance));

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
