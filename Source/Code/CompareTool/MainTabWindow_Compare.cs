using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

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
        private enum CompareTab : byte
        {
            Freeform
        }

        protected virtual float ExtraBottomSpace => 53f;
        protected virtual float ExtraTopSpace => 0f;

        private readonly Vector2 size = new Vector2(1500f, 500f); // TODO - calculate this

        private readonly List<TabRecord> tabs;

        private readonly Tab_Freeform freeformTab;

        private Tab currentTab;

        public override Vector2 RequestedTabSize => new Vector2(size.x + Margin * 2f, size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);

        public MainTabWindow_Compare()
        {
            tabs = new List<TabRecord>
            {
                new TabRecord("Graph".Translate(), () => currentTab = freeformTab, () => currentTab == freeformTab)
            };

            freeformTab = new Tab_Freeform();

            currentTab = freeformTab;
        }

        /// <summary>
        /// Draws the window contents
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        public override void DoWindowContents(Rect inRect)
        {
            inRect.yMin += ExtraTopSpace;
            inRect.yMax += ExtraBottomSpace;

            TabDrawer.DrawTabs(inRect, tabs);
            currentTab.DoTabContents(inRect);
        }
    }
}
