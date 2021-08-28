using System;
using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS.StatUtilities
{
    public class RangedWeaponStats
    {
        public readonly Thing weapon;
        public readonly VerbProperties shootVerb;

        public readonly int shotDamage;
        public readonly float warmup;
        public readonly float cooldown;

        public int BurstShotCount { get { return shootVerb.burstShotCount; } }
        public int BurstDelayTicks { get { return shootVerb.ticksBetweenBurstShots; } }

        public float MinRange { get { return shootVerb.minRange; } }
        public float MaxRange { get { return shootVerb.range; } }

        public float AccuracyTouch { get { return weapon.GetStatValue(StatDefOf.AccuracyTouch); } }
        public float AccuracyShort { get { return weapon.GetStatValue(StatDefOf.AccuracyShort); } }
        public float AccuracyMedium { get { return weapon.GetStatValue(StatDefOf.AccuracyMedium); } }
        public float AccuracyLong { get { return weapon.GetStatValue(StatDefOf.AccuracyLong); } }

        public RangedWeaponStats(Thing weapon)
        {
            this.weapon = weapon;
            shootVerb = GetShootVerb(weapon.def);

            // Get the damage from the loaded projectile if the weapon is loadable or the default projectile otherwise
            // TODO - check if it's even possible for regular weapons to have this comp
            var projectile = weapon.TryGetComp<CompChangeableProjectile>()?.Projectile?.projectile
                    ?? shootVerb.defaultProjectile?.projectile;

            // Default to zero damage if we can't find a projectile.
            // Not an error as unloaded mortars don't have projectiles
            shotDamage = projectile?.GetDamageAmount(weapon) ?? 0;
            warmup = shootVerb.warmupTime;
            cooldown = weapon.GetStatValue(StatDefOf.RangedWeapon_Cooldown, true);
        }

        public RangedWeaponStats(Building_TurretGun turret)
        {
            weapon = turret.gun;
            shootVerb = GetShootVerb(weapon.def);

            // Get the damage from the loaded projectile if the weapon is loadable or the default projectile otherwise
            var projectile = weapon.TryGetComp<CompChangeableProjectile>()?.Projectile?.projectile
                    ?? shootVerb?.defaultProjectile?.projectile;

            // Default to zero damage if we can't find a projectile.
            // Not an error as unloaded mortars don't have projectiles
            shotDamage = projectile?.GetDamageAmount(weapon) ?? 0;

            // Note that turrets completely ignore the warmup and cooldown stat of the weapon
            warmup = turret.def.building.turretBurstWarmupTime;

            // Logic duplicated from Building_TurretGun.BurstCooldownTime()
            if (turret.def.building.turretBurstCooldownTime >= 0f)
            {
                cooldown = turret.def.building.turretBurstCooldownTime;
            }
            else
            {
                cooldown = turret.AttackVerb.verbProps.defaultCooldownTime;
            }
        }

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
        /// <param name="shooter">(Optional) The Pawn wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        public float GetFullCycleTime(Pawn shooter = null)
        {
            float aimFactor = shooter?.GetStatValue(StatDefOf.AimingDelayFactor, true) ?? 1f;
            return (warmup * aimFactor) + cooldown + ((BurstShotCount - 1) * BurstDelayTicks).TicksToSeconds();
        }

        /// <summary>
        /// Gets the raw DPS of this weapon (The DPS assuming all shots hit their target).
        /// If shooter is provided, the shooter's aim speed will be factored into the DPS
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon.</returns>
        /// <param name="shooter">(Optional) The Pawn wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        public float GetRawDPS(Pawn shooter = null)
        {
            return shotDamage * BurstShotCount / GetFullCycleTime(shooter);
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
            float hitChance = shootVerb.GetHitChanceFactor(weapon, range);
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
}
