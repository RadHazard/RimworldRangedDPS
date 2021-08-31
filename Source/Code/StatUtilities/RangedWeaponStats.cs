using System;
using System.Linq;
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
        public abstract string Label { get; }

        public abstract float Warmup { get; }
        public abstract float Cooldown { get; }

        public abstract int ShotDamage { get; }
        public abstract float ArmorPenetration { get; }

        public abstract int BurstShotCount { get; }
        public abstract int BurstDelayTicks { get; }

        public abstract float MinRange { get; }
        public abstract float MaxRange { get; }

        public abstract float AccuracyTouch { get; }
        public abstract float AccuracyShort { get; }
        public abstract float AccuracyMedium { get; }
        public abstract float AccuracyLong { get; }

        protected VerbProperties GetShootVerb(ThingDef thingDef)
        {
            // Note - the game uses the first shoot verb and ignores the rest for whatever reason.  Do the same here
            var verb = (from v in thingDef.Verbs
                        where !v.IsMeleeAttack
                        select v).FirstOrDefault();
            if (verb == null) Log.Error($"[RangedDPS] Could not find a valid shoot verb for ThingDef {thingDef.defName}");
            return verb;
        }


        /// <summary>
        /// Gets the full cycle time of this weapon (The time from beginning to aim a shot to the end of the cooldown).
        /// If shooter is provided, the shooter's aim speed will be factored into the cycle time
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon.</returns>
        /// <param name="shooter">(Optional) The shooter wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        public float GetFullCycleTime(Pawn shooter = null)
        {
            float aimFactor = shooter?.GetStatValue(StatDefOf.AimingDelayFactor, true) ?? 1f;
            return (Warmup * aimFactor) + Cooldown + ((BurstShotCount - 1) * BurstDelayTicks).TicksToSeconds();
        }

        /// <summary>
        /// Gets the raw DPS of this weapon (The DPS assuming all shots hit their target).
        /// If shooter is provided, the shooter's aim speed will be factored into the DPS
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon.</returns>
        /// <param name="shooter">(Optional) The Pawn wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        public float GetRawDPS(Pawn shooter = null)
        {
            return ShotDamage * BurstShotCount / GetFullCycleTime(shooter);
        }

        /// <summary>
        /// Gets the adjusted hit chance factor of a shot.  This is equivalent to shootVerb.GetHitChanceFactor() unless
        /// a shooter is provided, in which case it will also be adjusted based on the shooter's hit chance.
        /// 
        /// This value can be greater than 1.0 in the case of weapons with overcapped accuracy.
        /// </summary>
        /// <returns>The adjusted hit chance factor.</returns>
        /// <param name="range">The range of the shot.</param>
        /// <param name="shooter">(Optional) The turret or pawn shooting the weapon.</param>
        public float GetAdjustedHitChanceFactor(float range, Thing shooter = null)
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
            {
                hitChance *= ShotReport.HitFactorFromShooter(shooter, range);
            }

            return hitChance;
        }

        /// <summary>
        /// Gets the accuracy-adjusted DPS of this weapon at a particular range.  If shooter is provided, the shooter's
        /// aim speed and shooting accuracy will be factored into the DPS calculation.
        /// </summary>
        /// <returns>The accuracy-adjusted ranged DPS of the weapon.</returns>
        /// <param name="range">The range of the shot.</param>
        /// <param name="shooter">(Optional) The turret or pawn shooting the weapon.</param>
        public float GetAdjustedDPS(float range, Thing shooter = null)
        {
            return GetRawDPS(shooter as Pawn) * Math.Min(GetAdjustedHitChanceFactor(range, shooter), 1f);
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
        /// <param name="shooter">(Optional) The turret or pawn shooting the weapon.</param>
        public float FindOptimalRange(Thing shooter = null)
        {
            int minRangeInt = (int)Math.Max(1.0, Math.Ceiling(MinRange));
            int maxRangeInt = (int)Math.Floor(MaxRange);
            return Enumerable.Range(minRangeInt, maxRangeInt).MaxBy(range => GetAdjustedHitChanceFactor(range, shooter));
        }
    }

    /// <summary>
    /// RangedWeaponStats for a handheld gun
    /// </summary>
    public class GunStats : RangedWeaponStats
    {
        private readonly Thing weapon;
        private readonly VerbProperties shootVerb;
        private readonly ProjectileProperties projectile;

        public override string Label => weapon.LabelCap;

        public override float Warmup => shootVerb.warmupTime;
        public override float Cooldown => weapon.GetStatValue(StatDefOf.RangedWeapon_Cooldown, true);

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
        private readonly VerbProperties shootVerb;
        private readonly ProjectileProperties projectile;
        private readonly float cooldown;

        public override string Label => turret.gun.LabelCap;

        // Note that turrets completely ignore the warmup and cooldown stat of the weapon
        public override float Warmup => turret.def.building.turretBurstWarmupTime;
        public override float Cooldown => cooldown;

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

        public TurretGunStats(Building_TurretGun turret)
        {
            this.turret = turret;
            shootVerb = GetShootVerb(turret.gun.def);

            // Note that the projectile can potentially be null if the weapon is loadable, like a morter
            projectile = turret.gun.TryGetComp<CompChangeableProjectile>()?.Projectile?.projectile
                    ?? shootVerb.defaultProjectile?.projectile;

            // Logic duplicated from Building_TurretGun.BurstCooldownTime()
            if (turret.def.building.turretBurstCooldownTime >= 0f)
                cooldown = turret.def.building.turretBurstCooldownTime;
            else
                cooldown = turret.AttackVerb.verbProps.defaultCooldownTime;
        }
    }
}
