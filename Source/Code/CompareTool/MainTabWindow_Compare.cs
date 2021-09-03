using RimWorld;
using UnityEngine;
using RangedDPS.Utilities;
using RangedDPS.StatUtilities;
using RangedDPS.GUIUtils;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// The main tab window for the comparison tool.  The compare tool lets you compare multiple weapon/pawn/turret
    /// combinations to see how they fare
    /// 
    /// TODO
    /// Tab ideas
    ///- Compare weapons
    ///  - Select one or more weapons to compare(allow turret guns to be used here-)
    ///  - Optionally, include a shooter who will hold the guns
    ///  - Optionally, include a target(presets for common enemy types, as well as custom settings)
    ///
    ///- Compare pawns
    ///  - Select one or more pawns to compare
    ///  - Allow turrets to be selected as well
    ///  - Optionally, include a gun that all pawns will use.  Otherwise, they'll use their current weapon (turrets will be unaffected).
    ///  - Optionally, include a target
    ///
    ///- Compare targets
    ///  - Select one or more targets to compare
    ///  - Have a single weapon to compare
    ///  - Optionally, include a shooter
    ///  - Allow turrets to be selected instead of shooter/gun
    /// 
    /// - Compare turret resources
    ///  - Select one or more turrets
    ///  - Have a single weapon to compare
    ///  - Optionally allow a turret
    ///
    ///- Freeform
    ///  - Select any combination of Pawn + weapon + target for each entry
    ///  - More customaizable but not as easy to use
    /// </summary>
    public class MainTabWindow_Compare : MainTabWindow
    {
        protected virtual float ExtraBottomSpace => 53f;
        protected virtual float ExtraTopSpace => 0f;
        //protected override float Margin => 6f;

        private readonly Vector2 size = new Vector2(1000f, 500f); // TODO - calculate this
        private readonly LineGraph graph = new LineGraph();

        public override Vector2 RequestedTabSize => new Vector2(size.x + Margin * 2f, size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);

        /// <summary>
        /// Runs before the window opens
        /// </summary>
        public override void PreOpen()
        {
            base.PreOpen();

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
        /// Draws the window contents
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        public override void DoWindowContents(Rect inRect)
        {
            UnityEngine.GUI.BeginGroup(inRect);

            Rect graphRect = new Rect(0f, 0f, inRect.width, 450f);
            Rect legendRect = new Rect(0f, graphRect.yMax, inRect.width, 40f);

            graph.Draw(graphRect, legendRect);

            UnityEngine.GUI.EndGroup();
        }
    }
}
