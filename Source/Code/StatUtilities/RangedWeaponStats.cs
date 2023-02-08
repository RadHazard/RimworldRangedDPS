using System;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace RangedDPS.StatUtilities
{
    /// <summary>
    /// Wrapper class to unify the stats of a ranged weapon (without a shooter),
    /// regardless of whether it's a turret or a handheld gun
    /// </summary>
    public abstract class RangedWeaponStats
    {
        [PublicAPI] public abstract string Label { get; }

        [PublicAPI] public abstract float Warmup { get; }
        [PublicAPI] public abstract float Cooldown { get; }

        [PublicAPI] public abstract int ShotDamage { get; }
        [PublicAPI] public abstract float ArmorPenetration { get; }

        [PublicAPI] public abstract int BurstShotCount { get; }
        [PublicAPI] public abstract int BurstDelayTicks { get; }

        [PublicAPI] public abstract float MinRange { get; }
        [PublicAPI] public abstract float MaxRange { get; }

        [PublicAPI] public abstract float AccuracyTouch { get; }
        [PublicAPI] public abstract float AccuracyShort { get; }
        [PublicAPI] public abstract float AccuracyMedium { get; }
        [PublicAPI] public abstract float AccuracyLong { get; }

        /// <summary>
        /// Gets the shoot verb of the given ThingDef
        /// </summary>
        /// <param name="thingDef">The ThingDef to get the verb from</param>
        /// <returns>The shoot verb</returns>
        protected static VerbProperties GetShootVerb(ThingDef thingDef)
        {
            // Note - the game uses the first shoot verb and ignores the rest for whatever reason.  Do the same here.
            // TODO - check if this is still true in 1.4
            var verb = (from v in thingDef.Verbs
                        where !v.IsMeleeAttack
                        select v).FirstOrDefault();
            if (verb == null)
            {
                Log.Error($"[RangedDPS] Could not find a valid shoot verb for ThingDef {thingDef.defName}!");
                verb = new VerbProperties(); // Make a null object to avoid cascading NPEs
            }
            return verb;
        }

        /// <summary>
        /// Gets the full cycle time of this weapon (The time from beginning to aim a shot to the end of the cooldown).
        /// If shooter is provided, the shooter's aim speed will be factored into the cycle time
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon.</returns>
        /// <param name="shooter">(Optional) The shooter wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        [PublicAPI]
        public float GetFullCycleTime(ShooterStats? shooter = null)
        {
            var aimFactor = shooter?.AimSpeed ?? 1f;
            return (Warmup * aimFactor) + Cooldown + ((BurstShotCount - 1) * BurstDelayTicks).TicksToSeconds();
        }

        /// <summary>
        /// Gets the raw DPS of this weapon (The DPS assuming all shots hit their target).
        /// If shooter is provided, the shooter's aim speed will be factored into the DPS
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon.</returns>
        /// <param name="shooter">(Optional) The shooter wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float GetRawDPS(ShooterStats? shooter = null, TargetStats? target = null)
        {
            target ??= TargetStats.StandardTarget;

            return ShotDamage * target.GetSharpDamageReduction(ArmorPenetration) * BurstShotCount / GetFullCycleTime(shooter);
        }

        /// <summary>
        /// Gets the adjusted hit chance of a shot.  This is the total chance a shot from this weapon will hit, taking
        /// into account the shooter's and target's stats, if provided.
        /// 
        /// This value can be greater than 1.0 in the case of weapons with overcapped accuracy.
        /// </summary>
        /// <returns>The adjusted hit chance factor.</returns>
        /// <param name="range">The range of the shot.</param>
        /// <param name="shooter">(Optional) The shooter wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float GetAdjustedHitChance(float range, ShooterStats? shooter = null, TargetStats? target = null)
        {
            // replicated from ShootVerb.GetHitChanceFactor()
            float hitChance;
            if (range <= 3f)
                hitChance = AccuracyTouch;
            else if (range <= 12f)
                hitChance = Mathf.Lerp(AccuracyTouch, AccuracyShort, Mathf.InverseLerp(3f, 12f, range));
            else if (range <= 25f)
                hitChance = Mathf.Lerp(AccuracyShort, AccuracyMedium, Mathf.InverseLerp(12f, 25f, range));
            else if (range <= 40f)
                hitChance = Mathf.Lerp(AccuracyMedium, AccuracyLong, Mathf.InverseLerp(25f, 40f, range));
            else
                hitChance = AccuracyLong;
            // Note - vanilla clamps this value between 0.01 and 1, but there are several mods that allow overcapped accuracy
            // In vanilla, weapons can't have >100% accuracy anyway so not capping it won't hurt things

            if (shooter != null)
                hitChance *= ShotReport.HitFactorFromShooter(shooter.ShootingAccuracy, range);

            if (target != null)
                hitChance *= target.TotalHitFactor;

            return hitChance;
        }

        /// <summary>
        /// Gets the accuracy-adjusted DPS of this weapon at a particular range.  If shooter is provided, the shooter's
        /// aim speed and shooting accuracy will be factored into the DPS calculation.
        /// </summary>
        /// <returns>The accuracy-adjusted ranged DPS of the weapon.</returns>
        /// <param name="range">The range of the shot.</param>
        /// <param name="shooter">(Optional) The shooter wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float GetAdjustedDPS(float range, ShooterStats? shooter = null, TargetStats? target = null)
        {
            return GetRawDPS(shooter, target) * Math.Min(GetAdjustedHitChance(range, shooter, target), 1f);
        }

        /// <summary>
        /// Calculates and returns the optimal range of the weapon (the range at which accuracy is highest).  If
        /// shooter is provided, the calculation correctly accounts for the shooter's accuracy as well as that of the
        /// weapon.
        /// </summary>
        /// <returns>
        /// The range, in cells, at which this weapon performs best (for the <paramref name="shooter"/> if provided, or
        /// in general if not).
        /// </returns>
        /// <param name="shooter">(Optional) The shooter wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float FindOptimalRange(ShooterStats? shooter = null, TargetStats? target = null)
        {
            var minRangeInt = Math.Max(1, Mathf.CeilToInt(MinRange));
            var maxRangeInt = Mathf.FloorToInt(MaxRange);
            return Enumerable.Range(minRangeInt, maxRangeInt).MaxBy(range => GetAdjustedHitChance(range, shooter, target));
        }
    }

    /// <summary>
    /// RangedWeaponStats for a handheld gun
    /// </summary>
    public class GunStats : RangedWeaponStats
    {
        private readonly Thing weapon;
        private readonly VerbProperties shootVerb;
        private readonly ProjectileProperties? projectile;

        public override string Label => weapon.LabelCap;

        public override float Warmup => shootVerb.warmupTime;
        public override float Cooldown => weapon.GetStatValue(StatDefOf.RangedWeapon_Cooldown);

        public override int ShotDamage => projectile?.GetDamageAmount(weapon) ?? 0;
        public override float ArmorPenetration => projectile?.GetArmorPenetration(weapon) ?? 0;

        public override int BurstShotCount => shootVerb.burstShotCount;
        public override int BurstDelayTicks => shootVerb.ticksBetweenBurstShots;

        public override float MinRange => shootVerb.minRange;
        public override float MaxRange => shootVerb.range;

        public override float AccuracyTouch => weapon.GetStatValue(StatDefOf.AccuracyTouch);
        public override float AccuracyShort => weapon.GetStatValue(StatDefOf.AccuracyShort);
        public override float AccuracyMedium => weapon.GetStatValue(StatDefOf.AccuracyMedium);
        public override float AccuracyLong => weapon.GetStatValue(StatDefOf.AccuracyLong);

        public GunStats(Thing weapon)
        {
            this.weapon = weapon;
            shootVerb = GetShootVerb(weapon.def);
            projectile = shootVerb.defaultProjectile?.projectile;
        }
    }

    /// <summary>
    /// RangedWeaponStats for a turret's gun
    /// </summary>
    public class TurretGunStats : RangedWeaponStats
    {
        private readonly Building_TurretGun turret;
        private readonly CompRefuelable? compRefuelable;
        private readonly VerbProperties shootVerb;
        private readonly ProjectileProperties? projectile;

        public override string Label => turret.gun.LabelCap;

        // Note that turrets completely ignore the warmup and cooldown stat of the weapon
        // 1.4 changed this into a range, but averaging it out gives the same result
        public override float Warmup => turret.def.building.turretBurstWarmupTime.Average;
        public override float Cooldown { get; }

        public override int ShotDamage => projectile?.GetDamageAmount(turret.gun) ?? 0;
        public override float ArmorPenetration => projectile?.GetArmorPenetration(turret.gun) ?? 0;

        public override int BurstShotCount => shootVerb.burstShotCount;
        public override int BurstDelayTicks => shootVerb.ticksBetweenBurstShots;

        public override float MinRange => shootVerb.minRange;
        public override float MaxRange => shootVerb.range;

        public override float AccuracyTouch => turret.gun.GetStatValue(StatDefOf.AccuracyTouch);
        public override float AccuracyShort => turret.gun.GetStatValue(StatDefOf.AccuracyShort);
        public override float AccuracyMedium => turret.gun.GetStatValue(StatDefOf.AccuracyMedium);
        public override float AccuracyLong => turret.gun.GetStatValue(StatDefOf.AccuracyLong);

        /// <summary>
        /// The shooter stats of this turret
        /// </summary>
        /// <value>The shooter.</value>
        [PublicAPI]
        public ShooterStats Shooter { get; }

        /// <summary>
        /// Gets a value indicating whether this turret uses fuel (barrel refurbishing, ammo, etc.).
        /// </summary>
        /// <value><c>true</c> if needs fuel; otherwise, <c>false</c>.</value>
        [PublicAPI]
        public bool NeedsFuel => compRefuelable != null;

        /// <summary>
        /// Gets the amount of shots this turret can shoot per unit of fuel
        /// </summary>
        /// <value>The fuel per shot, or float.MaxValue if the turret does not use fuel.</value>
        [PublicAPI]
        public float ShotsPerFuel
        {
            get
            {
                if (compRefuelable == null) return float.MaxValue;
                return compRefuelable.Props.FuelMultiplierCurrentDifficulty;
            }
        }

        /// <summary>
        /// Gets the amount of fuel used per point of damage dealt by the turret (assuming the shot hits).
        /// Returns 0 if the turret does not use fuel.
        /// </summary>
        /// <value>The fuel per damage, or float.MaxValue if the turret does not use fuel.</value>
        [PublicAPI]
        public float DamagePerFuel
        {
            get
            {
                if (compRefuelable == null) return float.MaxValue;
                return ShotsPerFuel * ShotDamage;
            }
        }

        /// <summary>
        /// Gets the accuracy-adjusted damage this turret can do per unit of fuel at a particular range.
        /// </summary>
        /// <returns>The accuracy-adjusted ranged damage per fuel of the turret.</returns>
        /// <param name="range">The range of the shot.</param>
        [PublicAPI]
        public float GetAdjustedDamagePerFuel(float range)
        {
            return DamagePerFuel * Math.Min(GetTurretAdjustedHitChance(range), 1f);
        }

        public TurretGunStats(Building_TurretGun turret)
        {
            this.turret = turret;
            Shooter = new TurretShooterStats(turret);
            compRefuelable = turret.TryGetComp<CompRefuelable>();
            shootVerb = GetShootVerb(turret.gun.def);

            // Note that the projectile can potentially be null if the weapon is loadable, like a mortar
            projectile = turret.gun.TryGetComp<CompChangeableProjectile>()?.Projectile?.projectile
                    ?? shootVerb.defaultProjectile?.projectile;

            // Logic duplicated from Building_TurretGun.BurstCooldownTime()
            if (turret.def.building.turretBurstCooldownTime >= 0f)
                Cooldown = turret.def.building.turretBurstCooldownTime;
            else
                Cooldown = turret.AttackVerb.verbProps.defaultCooldownTime;
        }

        /// <summary>
        /// Gets the raw DPS of this turret (The DPS assuming all shots hit their target).
        /// </summary>
        /// <returns>The raw ranged DPS of the turret.</returns>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float GetTurretRawDPS(TargetStats? target = null) => GetRawDPS(Shooter, target);

        /// <summary>
        /// Gets the adjusted hit chance of a shot.  This is the chance a shot from this turret will hit, taking into
        /// account the target's stats, if provided.
        /// 
        /// This value can be greater than 1.0 in the case of weapons with overcapped accuracy.
        /// </summary>
        /// <returns>The adjusted hit chance factor.</returns>
        /// <param name="range">The range of the shot.</param>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float GetTurretAdjustedHitChance(float range, TargetStats? target = null) => GetAdjustedHitChance(range, Shooter, target);

        /// <summary>
        /// Gets the accuracy-adjusted DPS of this turret at a particular range.
        /// </summary>
        /// <returns>The accuracy-adjusted ranged DPS of the weapon.</returns>
        /// <param name="range">The range of the shot.</param>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float GetTurretAdjustedDPS(float range, TargetStats? target = null) => GetAdjustedDPS(range, Shooter, target);

        /// <summary>
        /// Calculates and returns the optimal range of the turret + gun (the range at which accuracy is highest).
        /// weapon.
        /// </summary>
        /// <returns>The range, in cells, at which this turret performs best</returns>
        /// <param name="target">(Optional) The target we're shooting at, or null to assume an unarmored human not in cover</param>
        [PublicAPI]
        public float FindTurretOptimalRange(TargetStats? target = null) => FindOptimalRange(Shooter, target);
    }
}
