using System;
using RimWorld;
using Verse;

namespace RangedDPS.StatUtilities
{
    /// <summary>
    /// Wrapper class to unify the stats of a shooter,
    /// regardless of whether it's a turret, a pawn, or a hypothetical shooter
    /// </summary>
    public abstract class ShooterStats
    {
        public abstract string Label { get; }
        public abstract float AimSpeed { get; }
        public abstract float ShootingAccuracy { get; }
    }

    /// <summary>
    /// Shooter stats for a simulated pawn
    /// </summary>
    public class SimulatedShooterStats : ShooterStats
    {
        protected readonly float shootingSkill;
        protected readonly SimulatedShooterAim aimLevel;

        public override string Label
        {
            get
            {
                if (aimLevel == SimulatedShooterAim.TriggerHappy)
                    return $"Shooting {shootingSkill} (Trigger Happy)"; // TODO translate
                if (aimLevel == SimulatedShooterAim.CarefulShooter)
                    return $"Shooting {shootingSkill} (Careful Shooter)"; // TODO translate
                return $"Shooting {shootingSkill}";//TODO translate
            }
        }

        protected float EffectiveShootingSkill
        {
            get
            {
                switch (aimLevel)
                {
                    case SimulatedShooterAim.CarefulShooter:
                        return shootingSkill + 5f;
                    case SimulatedShooterAim.TriggerHappy:
                        return shootingSkill - 5f;
                    default:
                        return shootingSkill;
                }
            }
        }

        public override float AimSpeed
        {
            get
            {
                switch (aimLevel)
                {
                    case SimulatedShooterAim.CarefulShooter:
                        return 1.25f;
                    case SimulatedShooterAim.TriggerHappy:
                        return 0.5f;
                    default:
                        return 1f;
                }
            }
        }

        public override float ShootingAccuracy => StatDefOf.ShootingAccuracyPawn.postProcessCurve.Evaluate(EffectiveShootingSkill);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RangedDPS.StatUtilities.FakeShooterStats"/> class.
        /// </summary>
        /// <param name="shootingSkill">The shooting skill of the pawn to simulate.</param>
        /// <param name="aimLevel">The aim quality trait of the pawn.</param>
        protected SimulatedShooterStats(float shootingSkill = 8f, SimulatedShooterAim aimLevel = SimulatedShooterAim.Normal)
        {
            this.shootingSkill = shootingSkill;
            this.aimLevel = aimLevel;
        }
    }

    /// <summary>
    /// The aim quality of a fake shooter
    /// </summary>
    public enum SimulatedShooterAim
    {
        TriggerHappy,
        Normal,
        CarefulShooter
    }

    /// <summary>
    /// Shooter stats for a pawn
    /// </summary>
    public class PawnShooterStats : ShooterStats
    {
        private readonly Pawn shooter;

        public override string Label => shooter.LabelShortCap;

        public override float AimSpeed => shooter.GetStatValue(StatDefOf.AimingDelayFactor, true);
        public override float ShootingAccuracy => shooter.GetStatValue(StatDefOf.ShootingAccuracyPawn, true);

        public PawnShooterStats(Pawn shooter)
        {
            this.shooter = shooter;
        }
    }

    /// <summary>
    /// Shooter stats for a turret
    /// </summary>
    public class TurretShooterStats : ShooterStats
    {
        private readonly Building_TurretGun turret;

        public override string Label => turret.LabelShortCap;

        public override float AimSpeed => 1f; // Turrets don't have an aim speed factor
        public override float ShootingAccuracy => turret.GetStatValue(StatDefOf.ShootingAccuracyTurret, true);

        public TurretShooterStats(Building_TurretGun turret)
        {
            this.turret = turret;
        }

    }
}
