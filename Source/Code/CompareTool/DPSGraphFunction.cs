using RangedDPS.GUI;
using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// A graph function for graphing the DPS of a weapon with or without an associated pawn
    /// </summary>
    internal class GraphFunction_DPS : GraphFunction
    {
        private readonly RangedWeaponStats weapon;
        private readonly ShooterStats shooter;
        private readonly TargetStats target;

        public GraphFunction_DPS(RangedWeaponStats weapon, ShooterStats shooter = null, TargetStats target = null)
        {
            this.weapon = weapon;
            this.shooter = shooter;
            this.target = target;
        }

        //TODO keep this?
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
                    return $"{shooter.Label}, {weapon.Label}";//TODO translate
                else
                    return weapon.Label;
            }
        }

        public override float DomainMin => weapon.MinRange;
        public override float DomainMax => weapon.MaxRange;

        public override float GetValueFor(float x)
        {
            return weapon.GetAdjustedDPS(x, shooter, target);
        }
    }

    /// <summary>
    /// A graph function for graphing the DPS of a turret
    /// </summary>
    internal class GraphFunction_TurretDPS : GraphFunction
    {
        private readonly TurretGunStats turret;
        private readonly TargetStats target;

        public GraphFunction_TurretDPS(TurretGunStats turret, TargetStats target = null)
        {
            this.turret = turret;
            this.target = target;
        }

        /// <summary>
        /// Creates a GraphFunction_DPS for the given gun Thing
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="turret">Turret building.</param>
        public static GraphFunction_TurretDPS FromTurret(Building_TurretGun turret)
        {
            if (turret == null)
            {
                Log.Error($"[RangedDPS] Tried to get the ranged weapon stats of a null turret");
                return null;
            }

            return new GraphFunction_TurretDPS(new TurretGunStats(turret));
        }


        public override string Label => turret.Shooter.Label;

        public override float DomainMin => turret.MinRange;
        public override float DomainMax => turret.MaxRange;

        public override float GetValueFor(float x)
        {
            return turret.GetTurretAdjustedDPS(x, target);
        }
    }
}
