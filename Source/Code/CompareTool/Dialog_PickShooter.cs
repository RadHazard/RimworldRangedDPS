using System;
using System.Collections.Generic;
using RangedDPS.GUIUtils;
using RangedDPS.StatUtilities;
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

        private readonly Action<ShooterStats> callback;

        private readonly List<TabRecord> tabs;

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
                new TabRecord("Real Pawn", () => currentTab = ShooterTab.Colonist, () => currentTab == ShooterTab.Colonist),
                new TabRecord("Simulated", () => currentTab = ShooterTab.Simulated, () => currentTab == ShooterTab.Simulated)
            };

            // Default to the colonist tab if the current shooter is either a colonist or null
            if (currentStats is SimulatedShooterStats)
                currentTab = ShooterTab.Simulated;
            else
                currentTab = ShooterTab.Colonist;
        }

        /// <summary>
        /// Draws the window contents.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        public override void DoWindowContents(Rect inRect)
        {
            Rect content = inRect.Margins(bottomMargin: 40f); // the accept button is 35 pixes + some extra

            content.SplitHorizontally(50f, out Rect shooterDisplay, out Rect shooterSelector, 6f);

            Widgets.DrawMenuSection(shooterDisplay);
            Widgets.DrawMenuSection(shooterSelector);

            TabDrawer.DrawTabs(content, tabs);

            switch (currentTab)
            {
                case ShooterTab.Colonist:
                    DoColonistTab(content);
                    break;
                case ShooterTab.Simulated:
                    DoSimulatedTab(content);
                    break;
            }

            // Accept button
            if (Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK"))
            {
                //AcceptanceReport acceptanceReport = NameIsValid(curName);
                //if (!acceptanceReport.Accepted)
                //{
                //    if (acceptanceReport.Reason.NullOrEmpty())
                //    {
                //        Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                //    }
                //    else
                //    {
                //        Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
                //    }
                //}
                //else
                //{
                    callback(shooterStats);
                    Find.WindowStack.TryRemove(this);
                //}
            }
        }

        /// <summary>
        /// Draws the colonist select tab.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoColonistTab(Rect inRect)
        {
            //TODO
        }

        /// <summary>
        /// Draws the simulated shooter tab.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoSimulatedTab(Rect inRect)
        {
            //TODO
        }
    }
}
