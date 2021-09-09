using System;
using System.Collections.Generic;
using System.Linq;
using RangedDPS.GUIUtils;
using RangedDPS.StatUtilities;
using RangedDPS.Utilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// A selection dialogue to pick a gun
    /// 
    /// TODO
    /// Shooter selection
    ///- Real pawns should be selectable
    ///  - Add a button in the pawn UI to compare?
    ///  - Add a list of all colonists for quick selection
    ///  - Add a tool to click any pawn on the map?
    ///- Fake pawns should also be selectable
    ///  - Allow skill to be set
    ///  - Allow traits to be added(look up traits dynamically?)
    /// 
    /// </summary>
    public class Dialog_PickShooter : Window
    {
        enum ShooterTab
        {
            Colonist,
            Simulated
            //TODO - pick-a-pawn tool?
        }

        [TweakValue("___RangedDPS", 0f, 20f)]
        private static float PickShooterMargin = 6f;

        [TweakValue("___RangedDPS", 0f, 200f)]
        private static float PickShooterTabWidth = 100f;

        private readonly Action<ShooterStats> callback;

        private readonly List<TabRecord> tabs;
        private readonly ScrollableList_PawnSelect colonistList;

        private Pawn selectedPawn;
        private ShooterStats shooterStats;

        private ShooterTab currentTab;

        public override Vector2 InitialSize => new Vector2(500f, 500f);

        public Dialog_PickShooter(Action<ShooterStats> callback, ShooterStats currentStats)
        {
            forcePause = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnClickedOutside = true;

            this.callback = callback;
            shooterStats = currentStats;

            tabs = new List<TabRecord>
            {
                //TODO translate
                new TabRecord("Colonist", () => currentTab = ShooterTab.Colonist, () => currentTab == ShooterTab.Colonist),
                new TabRecord("Simulated", () => currentTab = ShooterTab.Simulated, () => currentTab == ShooterTab.Simulated)
            };

            // TODO is this the right list?
            colonistList = new ScrollableList_PawnSelect(SelectPawn, PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists);

            // Default to the colonist tab if the current shooter is either a colonist or null
            if (currentStats is SimulatedShooterStats)
            {
                currentTab = ShooterTab.Simulated;
            }
            else
            {
                if (currentStats is PawnShooterStats pawnStats)
                    selectedPawn = pawnStats.Pawn;
                currentTab = ShooterTab.Colonist;
            }
        }

        /// <summary>
        /// Draws the window contents.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        public override void DoWindowContents(Rect inRect)
        {
            Rect content = inRect.Margins(bottomMargin: 40f); // the accept button is 35 pixes + some extra

            (Rect shooterDisplay, Rect shooterSelect, Rect buttons) = inRect.SplitHorizontallyThreeWay(200f, 35f, 6f);

            Widgets.DrawMenuSection(shooterDisplay);
            DoShooterDisplay(shooterDisplay);

            shooterSelect.y += 32f; // Add space for tabs
            Widgets.DrawMenuSection(shooterSelect);
            TabDrawer.DrawTabs(shooterSelect, tabs, PickShooterTabWidth);

            switch (currentTab)
            {
                case ShooterTab.Colonist:
                    DoColonistTab(shooterSelect);
                    break;
                case ShooterTab.Simulated:
                    DoSimulatedTab(shooterSelect);
                    break;
            }

            // Accept button
            if (Widgets.ButtonText(buttons, "OK")) //TODO translate
            {
                callback(shooterStats);
                Find.WindowStack.TryRemove(this);
            }
        }

        /// <summary>
        /// Draws the currently selected shooter.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoShooterDisplay(Rect inRect)
        {
            Rect content = inRect.Margin(PickShooterMargin);

            if (shooterStats is PawnShooterStats pawnStats)
            {

            }
            else if (shooterStats is SimulatedShooterStats simStats)
            {

            }
            else
            {
                Widgets.Label(content, "(None)"); // TODO 
            }
        }

        /// <summary>
        /// Draws the colonist select tab.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoColonistTab(Rect inRect)
        {
            Rect content = inRect.Margin(PickShooterMargin);
            colonistList.Draw(content, selectedPawn.YieldIfNotNull());
        }

        /// <summary>
        /// Draws the simulated shooter tab.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoSimulatedTab(Rect inRect)
        {
            //TODO
        }

        /// <summary>
        /// Sets the pawn as the selected shooter
        /// </summary>
        /// <param name="pawn">Pawn.</param>
        private void SelectPawn(Pawn pawn)
        {
            selectedPawn = pawn;
            shooterStats = new PawnShooterStats(pawn);
        }
    }
}
