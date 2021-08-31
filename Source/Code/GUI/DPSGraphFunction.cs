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
        private readonly ShooterStats shooter;

        public GraphFunction_DPS(RangedWeaponStats weaponStats, ShooterStats shooter = null)
        {
            this.weaponStats = weaponStats;
            this.shooter = shooter;
        }

        /// <summary>
        /// Creates a GraphFunction_DPS for the given gun Thing
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="weapon">Weapon thing.</param>
        /// <param name="shooter">Shooter.</param>
        public static GraphFunction_DPS FromGun(Thing weapon, Pawn shooter = null)
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

            return new GraphFunction_DPS(new GunStats(weapon), new PawnShooterStats(shooter));
        }


        public override string Label
        {
            get
            {
                if (shooter != null)
                    return $"{shooter.Label} with {weaponStats.Label}";//TODO translate
                else
                    return $"{weaponStats.Label}";
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
