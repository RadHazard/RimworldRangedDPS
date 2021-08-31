using RimWorld;
using UnityEngine;
using RangedDPS.Utilities;

namespace RangedDPS.GUI
{
    public class MainTabWindow_Compare : MainTabWindow
    {

        protected virtual float ExtraBottomSpace => 53f;
        protected virtual float ExtraTopSpace => 0f;
        //protected override float Margin => 6f;

        private readonly Vector2 size = new Vector2(1000f, 500f); // TODO - calculate this
        private readonly GUIGraph graph = new GUIGraph();

        public override Vector2 RequestedTabSize => new Vector2(size.x + Margin * 2f, size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);

        /// <summary>
        /// Runs before the window opens
        /// </summary>
        public override void PreOpen()
        {
            base.PreOpen();

            // == TODO debug stuff ==
            graph.ClearFunctions();

            var testGun1 = ThingUtils.MakeThingByName("Gun_LMG", quality: QualityCategory.Normal);
            var testGun2 = ThingUtils.MakeThingByName("Gun_LMG", quality: QualityCategory.Excellent);
            var testGun3 = ThingUtils.MakeThingByName("Gun_AssaultRifle", quality: QualityCategory.Normal);

            graph.AddFunctions(GraphFunction_DPS.FromThing(testGun1),
                GraphFunction_DPS.FromThing(testGun2),
                GraphFunction_DPS.FromThing(testGun3));
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
            Rect legendRect = new Rect(0f, graphRect.yMax, inRect.width / 2f, 40f);

            graph.Draw(graphRect, legendRect);

            UnityEngine.GUI.EndGroup();
        }
    }
}
