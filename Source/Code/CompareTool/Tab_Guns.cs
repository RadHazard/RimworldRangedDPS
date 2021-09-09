using System;
using RangedDPS.GUIUtils;
using RangedDPS.StatUtilities;
using RangedDPS.Utilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// Comparison tab for guns
    /// </summary>
    public class Tab_Guns : Tab
    {
        [TweakValue("___RangedDPS", 0f, 100f)]
        private static float GunsMenuTopSection = 100f;

        [TweakValue("___RangedDPS", 0f, 10f)]
        private static float GunsMenuShooterTargetMargin = 2f;

        // TODO translation
        private Lazy<string> label = new Lazy<string>(() => "Graph".Translate());

        public override string Label => "Guns";//label.Value;

        private ShooterStats shooter;

        private TargetStats target = TargetStats.StandardTarget;


        /// <summary>
        /// Runs before the window opens
        /// </summary>
        public Tab_Guns()
        {
            // == TODO debug stuff ==
            graph.ClearFunctions();

            //var miniturret = (Building_TurretGun)ThingUtils.MakeThingByName("Turret_MiniTurret");

            var testGun1 = new GunStats(ThingUtils.MakeThingByName("Gun_AssaultRifle", quality: QualityCategory.Normal));
            var testIdiot1 = new SimulatedShooterStats(12f);

            var testGun2 = new GunStats(ThingUtils.MakeThingByName("Gun_LMG", quality: QualityCategory.Normal));
            var testIdiot2 = new SimulatedShooterStats(12f);

            var testGun3 = new GunStats(ThingUtils.MakeThingByName("Gun_AssaultRifle", quality: QualityCategory.Excellent));
            var testIdiot3 = new SimulatedShooterStats(12f);

            var testGun4 = new GunStats(ThingUtils.MakeThingByName("Gun_LMG", quality: QualityCategory.Excellent));
            var testIdiot4 = new SimulatedShooterStats(12f);

            var testGun5 = new GunStats(ThingUtils.MakeThingByName("Gun_AssaultRifle", quality: QualityCategory.Legendary));
            var testIdiot5 = new SimulatedShooterStats(12f);

            var testGun6 = new GunStats(ThingUtils.MakeThingByName("Gun_LMG", quality: QualityCategory.Legendary));
            var testIdiot6 = new SimulatedShooterStats(12f);

            var smolTarget = new SimulatedTargetStats(0.5f);
            var bigTarget = new SimulatedTargetStats(2.0f);

            graph.AddFunctions(
                //GraphFunction_TurretDPS.FromTurret(miniturret),
                new GraphFunction_DPS(testGun1, testIdiot1, bigTarget),
                new GraphFunction_DPS(testGun2, testIdiot2, bigTarget),
                new GraphFunction_DPS(testGun3, testIdiot3, bigTarget),
                new GraphFunction_DPS(testGun4, testIdiot4, bigTarget),
                new GraphFunction_DPS(testGun5, testIdiot5, bigTarget),
                new GraphFunction_DPS(testGun6, testIdiot6, bigTarget));
            // == debug stuff ==
        }

        /// <summary>
        /// Draws the selection menu
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        protected override void DoMenuContents(Rect inRect)
        {
            inRect.SplitHorizontally(GunsMenuTopSection, out Rect shooterTargetRect, out Rect gunsRect);

            Rect shooterRect = shooterTargetRect.LeftHalf().Margin(GunsMenuShooterTargetMargin);
            Rect targetRect = shooterTargetRect.RightHalf().Margin(GunsMenuShooterTargetMargin);

            DoShooterWidget(shooterRect);
            DoTargetWidget(targetRect);
        }

        /// <summary>
        /// Draws the shooter selection widget
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoShooterWidget(Rect inRect)
        {
            string shooterLabel = shooter?.Label ?? "(none)"; // TODO translate
            if (Widgets.ButtonText(inRect, shooterLabel))
            {
                Find.WindowStack.Add(new Dialog_PickShooter(s => shooter = s, shooter));
            }
        }

        /// <summary>
        /// Draws the target selection widget
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoTargetWidget(Rect inRect)
        {
            string targetLabel = target?.Label ?? "(none)"; // TODO translate
            if (Widgets.ButtonText(inRect, targetLabel))
            {
                //TODO
            }
        }}
}
