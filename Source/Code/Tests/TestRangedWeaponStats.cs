#if DEBUG
using RimTest;
using RimWorld;
using Verse;
using static RimTest.Assertion;

namespace RangedDPS.Tests
{
    [TestSuite]
    public static class TestRangedWeaponStats
    {
        // This is a lot more lenient than the regular stats because the manual calculations I did are based on what
        // the UI shows.  The UI rounds to two decimal places at most, so there's going to be some error in the results.
        private const float DERIVED_DELTA = 0.05f;

        [Test]
        public static void StatsFromSingleShotGun()
        {
            Thing gun = UnittestUtils.GetThingByName("Gun_BoltActionRifle");
            RangedWeaponStats stats = new RangedWeaponStats(gun);

            // Base stats
            Assert(stats.shotDamage).To.Be.EqualTo(18);
            Assert(stats.warmup).To.Be.Approximately(1.7f);
            Assert(stats.cooldown).To.Be.Approximately(1.5f);

            Assert(stats.BurstShotCount).To.Be.EqualTo(1);

            Assert(stats.MinRange).To.Be.Approximately(0f);
            Assert(stats.MaxRange).To.Be.Approximately(36.9f);

            Assert(stats.AccuracyTouch).To.Be.Approximately(0.65f);
            Assert(stats.AccuracyShort).To.Be.Approximately(0.80f);
            Assert(stats.AccuracyMedium).To.Be.Approximately(0.90f);
            Assert(stats.AccuracyLong).To.Be.Approximately(0.80f);

            // Derived stats
            Assert(stats.GetFullCycleTime()).To.Be.Approximately(3.2f);
            Assert(stats.GetRawDPS()).To.Be.Approximately(5.625f);
            Assert(stats.FindOptimalRange()).To.Be.Approximately(25f);

            Assert(stats.GetAdjustedHitChanceFactor(1f)).To.Be.Approximately(0.65f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(3f)).To.Be.Approximately(0.65f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f)).To.Be.Approximately(0.80f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f)).To.Be.Approximately(0.90f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(36.9f)).To.Be.Approximately(0.821f, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(1f)).To.Be.Approximately(0.65f * 5.625f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(3f)).To.Be.Approximately(0.65f * 5.625f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f)).To.Be.Approximately(0.80f * 5.625f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f)).To.Be.Approximately(0.90f * 5.625f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(36.9f)).To.Be.Approximately(0.821f * 5.625f, DERIVED_DELTA);
        }

        [Test]
        public static void StatsFromBurstFireGun()
        {
            Thing gun = UnittestUtils.GetThingByName("Gun_HeavySMG");
            RangedWeaponStats stats = new RangedWeaponStats(gun);

            // Base stats
            Assert(stats.shotDamage).To.Be.EqualTo(12);
            Assert(stats.warmup).To.Be.Approximately(0.9f);
            Assert(stats.cooldown).To.Be.Approximately(1.65f);

            Assert(stats.BurstShotCount).To.Be.EqualTo(3);
            Assert(stats.BurstDelayTicks).To.Be.EqualTo(11);

            Assert(stats.MinRange).To.Be.Approximately(0f);
            Assert(stats.MaxRange).To.Be.Approximately(22.9f);

            Assert(stats.AccuracyTouch).To.Be.Approximately(0.85f);
            Assert(stats.AccuracyShort).To.Be.Approximately(0.65f);
            Assert(stats.AccuracyMedium).To.Be.Approximately(0.35f);
            Assert(stats.AccuracyLong).To.Be.Approximately(0.20f);

            // Derived stats
            Assert(stats.GetFullCycleTime()).To.Be.Approximately(2.916667f);
            Assert(stats.GetRawDPS()).To.Be.Approximately(12.3429f);
            Assert(stats.FindOptimalRange()).To.Be.Approximately(1f);

            Assert(stats.GetAdjustedHitChanceFactor(1f)).To.Be.Approximately(0.85f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(3f)).To.Be.Approximately(0.85f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f)).To.Be.Approximately(0.65f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(22.9f)).To.Be.Approximately(0.398f, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(1f)).To.Be.Approximately(0.85f * 12.3429f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(3f)).To.Be.Approximately(0.85f * 12.3429f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f)).To.Be.Approximately(0.65f * 12.3429f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(22.9f)).To.Be.Approximately(0.398f * 12.3429f, DERIVED_DELTA);
        }

        [Test]
        public static void StatsFromTurret()
        {
            Building_TurretGun turret = (Building_TurretGun)UnittestUtils.GetThingByName("Turret_MiniTurret");
            RangedWeaponStats stats = new RangedWeaponStats(turret);

            // Base stats
            Assert(stats.shotDamage).To.Be.EqualTo(11);
            Assert(stats.warmup).To.Be.Approximately(0f);
            Assert(stats.cooldown).To.Be.Approximately(4.80f);

            Assert(stats.BurstShotCount).To.Be.EqualTo(2);
            Assert(stats.BurstDelayTicks).To.Be.EqualTo(8);

            Assert(stats.MinRange).To.Be.Approximately(0f);
            Assert(stats.MaxRange).To.Be.Approximately(28.9f);

            Assert(stats.AccuracyTouch).To.Be.Approximately(0.70f);
            Assert(stats.AccuracyShort).To.Be.Approximately(0.64f);
            Assert(stats.AccuracyMedium).To.Be.Approximately(0.41f);
            Assert(stats.AccuracyLong).To.Be.Approximately(0.22f);

            // Derived stats
            Assert(stats.GetFullCycleTime()).To.Be.Approximately(4.933333f);
            Assert(stats.GetRawDPS()).To.Be.Approximately(4.4595f);
            Assert(stats.FindOptimalRange()).To.Be.Approximately(1f);

            Assert(stats.GetAdjustedHitChanceFactor(1f)).To.Be.Approximately(0.70f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(3f)).To.Be.Approximately(0.70f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f)).To.Be.Approximately(0.64f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f)).To.Be.Approximately(0.41f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(28.9f)).To.Be.Approximately(0.361f, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(1f)).To.Be.Approximately(0.70f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(3f)).To.Be.Approximately(0.70f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f)).To.Be.Approximately(0.64f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f)).To.Be.Approximately(0.41f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(28.9f)).To.Be.Approximately(0.361f * 4.4595f, DERIVED_DELTA);
        }

        [Test]
        public static void WeaponQualityAffectsStats()
        {
            Thing gun = UnittestUtils.GetThingByName("Gun_HeavySMG", quality: QualityCategory.Legendary);
            RangedWeaponStats stats = new RangedWeaponStats(gun);

            // Base stats
            Assert(stats.shotDamage).To.Be.EqualTo(18);
            Assert(stats.warmup).To.Be.Approximately(0.9f);
            Assert(stats.cooldown).To.Be.Approximately(1.65f);

            Assert(stats.BurstShotCount).To.Be.EqualTo(3);
            Assert(stats.BurstDelayTicks).To.Be.EqualTo(11);

            Assert(stats.MinRange).To.Be.Approximately(0f);
            Assert(stats.MaxRange).To.Be.Approximately(22.9f);

            Assert(stats.AccuracyTouch).To.Be.Approximately(1.00f);
            Assert(stats.AccuracyShort).To.Be.Approximately(0.975f);
            Assert(stats.AccuracyMedium).To.Be.Approximately(0.525f);
            Assert(stats.AccuracyLong).To.Be.Approximately(0.30f);

            // Derived stats
            Assert(stats.GetFullCycleTime()).To.Be.Approximately(2.916667f);
            Assert(stats.GetRawDPS()).To.Be.Approximately(18.5143f);
            Assert(stats.FindOptimalRange()).To.Be.Approximately(1f);

            Assert(stats.GetAdjustedHitChanceFactor(1f)).To.Be.Approximately(1.00f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(3f)).To.Be.Approximately(1.00f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f)).To.Be.Approximately(0.975f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(22.9f)).To.Be.Approximately(0.598f, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(1f)).To.Be.Approximately(1.00f * 18.5143f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(3f)).To.Be.Approximately(1.00f * 18.5143f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f)).To.Be.Approximately(0.975f * 18.5143f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(22.9f)).To.Be.Approximately(0.598f * 18.5143f, DERIVED_DELTA);
        }

        [Test]
        public static void ShooterSkillAffectsStats()
        {
            Thing gun = UnittestUtils.GetThingByName("Gun_SniperRifle");
            RangedWeaponStats stats = new RangedWeaponStats(gun);

            Pawn clueless = UnittestUtils.GetTestPawn(0);
            Pawn professional = UnittestUtils.GetTestPawn(10);
            Pawn legendary = UnittestUtils.GetTestPawn(20);

            float expectedDPS = stats.GetRawDPS();

            // Raw DPS (shouldn't change since only traits affect aim time)
            Assert(stats.GetRawDPS(clueless)).To.Be.Approximately(expectedDPS);
            Assert(stats.GetRawDPS(professional)).To.Be.Approximately(expectedDPS);
            Assert(stats.GetRawDPS(legendary)).To.Be.Approximately(expectedDPS);

            // Optimal range
            Assert(stats.FindOptimalRange(clueless)).To.Be.Approximately(1f);
            Assert(stats.FindOptimalRange(professional)).To.Be.Approximately(12f);
            Assert(stats.FindOptimalRange(legendary)).To.Be.Approximately(25f);

            // Hit chance
            Assert(stats.GetAdjustedHitChanceFactor(3f, clueless)).To.Be.Approximately(0.352f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f, clueless)).To.Be.Approximately(0.173f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f, clueless)).To.Be.Approximately(0.047f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(40f, clueless)).To.Be.Approximately(0.018f, DERIVED_DELTA);

            Assert(stats.GetAdjustedHitChanceFactor(3f, professional)).To.Be.Approximately(0.456f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f, professional)).To.Be.Approximately(0.486f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f, professional)).To.Be.Approximately(0.402f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(40f, professional)).To.Be.Approximately(0.260f, DERIVED_DELTA);

            Assert(stats.GetAdjustedHitChanceFactor(3f, legendary)).To.Be.Approximately(0.485f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f, legendary)).To.Be.Approximately(0.620f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f, legendary)).To.Be.Approximately(0.669f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(40f, legendary)).To.Be.Approximately(0.589f, DERIVED_DELTA);

            // DPS
            Assert(stats.GetAdjustedDPS(3f, clueless)).To.Be.Approximately(0.352f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f, clueless)).To.Be.Approximately(0.173f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f, clueless)).To.Be.Approximately(0.047f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(40f, clueless)).To.Be.Approximately(0.018f * expectedDPS, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(3f, professional)).To.Be.Approximately(0.456f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f, professional)).To.Be.Approximately(0.486f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f, professional)).To.Be.Approximately(0.402f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(40f, professional)).To.Be.Approximately(0.260f * expectedDPS, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(3f, legendary)).To.Be.Approximately(0.485f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f, legendary)).To.Be.Approximately(0.620f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f, legendary)).To.Be.Approximately(0.669f * expectedDPS, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(40f, legendary)).To.Be.Approximately(0.589f * expectedDPS, DERIVED_DELTA);
        }

        [Test]
        public static void ShooterTraitsAffectsStats()
        {
            Thing gun = UnittestUtils.GetThingByName("Gun_SniperRifle");
            RangedWeaponStats stats = new RangedWeaponStats(gun);

            Pawn triggerHappy = UnittestUtils.GetTestPawn(10);
            triggerHappy.AddTrait("ShootingAccuracy", -1);

            Pawn carefulShooter = UnittestUtils.GetTestPawn(10);
            carefulShooter.AddTrait("ShootingAccuracy", 1);

            // Raw DPS
            Assert(stats.GetRawDPS(triggerHappy)).To.Be.Approximately(6.1728f);
            Assert(stats.GetRawDPS(carefulShooter)).To.Be.Approximately(3.7453f);

            // Optimal range
            Assert(stats.FindOptimalRange(triggerHappy)).To.Be.Approximately(1f);
            Assert(stats.FindOptimalRange(carefulShooter)).To.Be.Approximately(12f);

            // Hit chance
            Assert(stats.GetAdjustedHitChanceFactor(3f, triggerHappy)).To.Be.Approximately(0.422f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f, triggerHappy)).To.Be.Approximately(0.355f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f, triggerHappy)).To.Be.Approximately(0.209f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(40f, triggerHappy)).To.Be.Approximately(0.092f, DERIVED_DELTA);

            Assert(stats.GetAdjustedHitChanceFactor(3f, carefulShooter)).To.Be.Approximately(0.473f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f, carefulShooter)).To.Be.Approximately(0.563f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f, carefulShooter)).To.Be.Approximately(0.546f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(40f, carefulShooter)).To.Be.Approximately(0.426f, DERIVED_DELTA);

            // DPS
            Assert(stats.GetAdjustedDPS(3f, triggerHappy)).To.Be.Approximately(0.422f * 6.1728f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f, triggerHappy)).To.Be.Approximately(0.355f * 6.1728f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f, triggerHappy)).To.Be.Approximately(0.209f * 6.1728f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(40f, triggerHappy)).To.Be.Approximately(0.092f * 6.1728f, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(3f, carefulShooter)).To.Be.Approximately(0.473f * 3.7453f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f, carefulShooter)).To.Be.Approximately(0.563f * 3.7453f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f, carefulShooter)).To.Be.Approximately(0.546f * 3.7453f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(40f, carefulShooter)).To.Be.Approximately(0.426f * 3.7453f, DERIVED_DELTA);
        }

        [Test]
        public static void TurretBaseAffectsStats()
        {
            Building_TurretGun turret = (Building_TurretGun)UnittestUtils.GetThingByName("Turret_MiniTurret");
            RangedWeaponStats stats = new RangedWeaponStats(turret);

            Assert(stats.FindOptimalRange(turret)).To.Be.Approximately(1f);

            Assert(stats.GetAdjustedHitChanceFactor(1f, turret)).To.Be.Approximately(0.672f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(3f, turret)).To.Be.Approximately(0.619f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(12f, turret)).To.Be.Approximately(0.392f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(25f, turret)).To.Be.Approximately(0.148f, DERIVED_DELTA);
            Assert(stats.GetAdjustedHitChanceFactor(28.9f, turret)).To.Be.Approximately(0.111f, DERIVED_DELTA);

            Assert(stats.GetAdjustedDPS(1f, turret)).To.Be.Approximately(0.672f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(3f, turret)).To.Be.Approximately(0.619f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(12f, turret)).To.Be.Approximately(0.392f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(25f, turret)).To.Be.Approximately(0.148f * 4.4595f, DERIVED_DELTA);
            Assert(stats.GetAdjustedDPS(28.9f, turret)).To.Be.Approximately(0.111f * 4.4595f, DERIVED_DELTA);
        }
    }
}
#endif