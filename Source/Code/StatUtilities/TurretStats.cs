using System;
using RimWorld;
using Verse;

namespace RangedDPS.StatUtilities
{
    public class TurretStats : RangedWeaponStats
    {
        public readonly Building_TurretGun turret;
        public readonly CompRefuelable compRefuelable;

        /// <summary>
        /// Gets a value indicating whether this turret uses fuel (barrel refurbishing, ammo, etc.).
        /// </summary>
        /// <value><c>true</c> if needs fuel; otherwise, <c>false</c>.</value>
        public bool NeedsFuel { get { return compRefuelable != null; } }

        /// <summary>
        /// Gets the amount of shots this turret can shoot per unit of fuel
        /// </summary>
        /// <value>The fuel per shot, or float.MaxValue if the turret does not use fuel.</value>
        public float ShotsPerFuel
        {
            get
            {
                if (!NeedsFuel) return float.MaxValue;
                return compRefuelable.Props.FuelMultiplierCurrentDifficulty;
            }
        }

        /// <summary>
        /// Gets the amount of fuel used per point of damage dealt by the turret (assuming the shot hits).
        /// Returns 0 if the turret does not use fuel.
        /// </summary>
        /// <value>The fuel per damage, or float.MaxValue if the turret does not use fuel.</value>
        public float DamagePerFuel {
            get {
                if (!NeedsFuel) return float.MaxValue;
                return ShotsPerFuel * shotDamage;
            }
        }

        public TurretStats(Building_TurretGun turret) : base(turret)
        {
            this.turret = turret;
            compRefuelable = turret.TryGetComp<CompRefuelable>();
        }

        /// <summary>
        /// Gets the accuracy-adjusted damage this turret can do per unit of fuel at a particular range.
        /// </summary>
        /// <returns>The accuracy-adjusted ranged damage per fuel of the turret.</returns>
        /// <param name="range">The range of the shot.</param>
        public float GetAdjustedDamagePerFuel(float range)
        {
            return DamagePerFuel * Math.Min(GetAdjustedHitChanceFactor(range, turret), 1f);
        }
    }
}
