using RangedDPS.StatUtilities;
using Verse;

namespace RangedDPS.GUI
{
    /// <summary>
    /// A graph function for graphing the DPS of a weapon with or without an associated pawn
    /// </summary>
    internal class GraphFunction_DPS : GraphFunction
    {
        private readonly RangedWeaponStats weaponStats;
        private readonly Thing shooter;

        public GraphFunction_DPS(RangedWeaponStats weaponStats, Thing shooter = null)
        {
            this.weaponStats = weaponStats;
            this.shooter = shooter;
        }

        /// <summary>
        /// Creates a GraphFunction_DPS for the given thing
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="weapon">Weapon thing.</param>
        /// <param name="shooter">Shooter.</param>
        public static GraphFunction_DPS FromThing(Thing weapon, Thing shooter = null)
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

            return new GraphFunction_DPS(new RangedWeaponStats(weapon), shooter);
        }


        public override string Label
        {
            get
            {
                if (shooter != null)
                    return $"{shooter.LabelShortCap} with {weaponStats.weapon.LabelCap}";
                else
                    return $"{weaponStats.weapon.LabelCap}";
            }
        }

        public override float DomainMin => weaponStats.MinRange;
        public override float DomainMax => weaponStats.MaxRange;

        public override float GetValueFor(float x)
        {
            return weaponStats.GetAdjustedDPS(x, shooter);
        }
    }
}
