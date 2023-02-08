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
        private readonly float shootingSkill;
        private readonly SimulatedShooterAim aimLevel;

        public override string Label
        {
            get
            {
                return aimLevel switch
                {
                    SimulatedShooterAim.TriggerHappy => $"Lvl {shootingSkill} (TH)",
                    SimulatedShooterAim.CarefulShooter => $"Lvl {shootingSkill} (CS)",
                    _ => $"Lvl {shootingSkill}"
                };
            }
        }

        private float EffectiveShootingSkill
        {
            get
            {
                return aimLevel switch
                {
                    SimulatedShooterAim.CarefulShooter => shootingSkill + 5f,
                    SimulatedShooterAim.TriggerHappy => shootingSkill - 5f,
                    _ => shootingSkill
                };
            }
        }

        public override float AimSpeed
        {
            get
            {
                return aimLevel switch
                {
                    SimulatedShooterAim.CarefulShooter => 1.25f,
                    SimulatedShooterAim.TriggerHappy => 0.5f,
                    _ => 1f
                };
            }
        }

        public override float ShootingAccuracy => StatDefOf.ShootingAccuracyPawn.postProcessCurve.Evaluate(EffectiveShootingSkill);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RangedDPS.StatUtilities.FakeShooterStats"/> class.
        /// </summary>
        /// <param name="shootingSkill">The shooting skill of the pawn to simulate.</param>
        /// <param name="aimLevel">The aim quality trait of the pawn.</param>
        public SimulatedShooterStats(float shootingSkill = 8f, SimulatedShooterAim aimLevel = SimulatedShooterAim.Normal)
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
        public Pawn Pawn { get; }

        public override string Label => Pawn.LabelShortCap;

        public override float AimSpeed => Pawn.GetStatValue(StatDefOf.AimingDelayFactor);
        public override float ShootingAccuracy => Pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn);

        public PawnShooterStats(Pawn shooter)
        {
            Pawn = shooter;
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
        public override float ShootingAccuracy => turret.GetStatValue(StatDefOf.ShootingAccuracyTurret);

        public TurretShooterStats(Building_TurretGun turret)
        {
            this.turret = turret;
        }

    }
}
