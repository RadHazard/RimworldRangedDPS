using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
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
        [UsedImplicitly]
        private static float MainMargin = 6f;

        [TweakValue("___RangedDPS", 0f, 200f)]
        [UsedImplicitly]
        private static float TabWidth = 100f;
        
        [TweakValue("___RangedDPS", 0f, 300f)]
        [UsedImplicitly]
        private static float IconWidth = 150f;
        
        [TweakValue("___RangedDPS", 0f, 20f)]
        [UsedImplicitly]
        private static float IconMargin = 6f;
        
        [TweakValue("___RangedDPS", 0f, 60f)]
        [UsedImplicitly]
        private static float StatHeight = 32f;
        
        [TweakValue("___RangedDPS", 300f, 1000f)]
        [UsedImplicitly]
        private static float Height = 500f;
        
        [TweakValue("___RangedDPS", 300f, 1000f)]
        [UsedImplicitly]
        private static float Width = 500f;

        private readonly Action<ShooterStats?> callback;

        private readonly List<TabRecord> tabs;
        private readonly ScrollableList_PawnSelect colonistList;

        private Pawn? selectedPawn;
        private ShooterStats? shooterStats;

        private ShooterTab currentTab;

        private Vector2 statScrollPosition = new(0f, 0f);

        public override Vector2 InitialSize => new(Height, Width);

        public Dialog_PickShooter(Action<ShooterStats?> callback, ShooterStats? currentStats)
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
                new("Colonist", () => currentTab = ShooterTab.Colonist, () => currentTab == ShooterTab.Colonist),
                new("Simulated", () => currentTab = ShooterTab.Simulated, () => currentTab == ShooterTab.Simulated)
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
            var content = inRect.Margins(bottomMargin: 40f); // the accept button is 35 pixes + some extra

            var (shooterDisplay, shooterSelect, buttons) = content.SplitHorizontallyThreeWay(200f, 35f, 6f);

            Widgets.DrawMenuSection(shooterDisplay);
            DoShooterDisplay(shooterDisplay);

            shooterSelect.y += 32f; // Add space for tabs
            Widgets.DrawMenuSection(shooterSelect);
            TabDrawer.DrawTabs(shooterSelect, tabs, TabWidth);

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
            var content = inRect.Margin(MainMargin);

            switch (shooterStats)
            {
                case PawnShooterStats pawnStats:
                {
                    var pawn = pawnStats.Pawn;
                    content.SplitVertically(IconWidth, out var pawnDisplay, out var statsDisplay);

                    // Draw the pawn
                    pawnDisplay.SplitHorizontally(IconWidth, out var pawnIcon, out var pawnName);
                    Widgets.ThingIcon(pawnIcon.Margin(IconMargin), pawn);
                    Widgets.Label(pawnName, pawn.LabelCap);

                    // Draw the pawn's stats
                    statsDisplay.SplitHorizontally(StatHeight, out var statsPane, out var statsExplanation);

                    // TODO translate
                    var shootingAccuracyWorker = StatDefOf.ShootingAccuracyPawn.Worker;
                    var aimSpeedWorker = StatDefOf.AimingDelayFactor.Worker;
                    var statRequest = StatRequest.For(pawn);

                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"Shooting Accuracy: {shootingAccuracyWorker.GetValue(statRequest):P}");
                    stringBuilder.AppendLine($"Aiming Time: {aimSpeedWorker.GetValue(statRequest):P}");
                    Widgets.Label(statsPane, stringBuilder.ToString());

                    stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(shootingAccuracyWorker
                        .GetExplanationUnfinalized(statRequest, ToStringNumberSense.Factor).TrimEndNewlines());
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(" ------------------------------- ");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(aimSpeedWorker
                        .GetExplanationUnfinalized(statRequest, ToStringNumberSense.Factor).TrimEndNewlines());
                    var explanation = stringBuilder.ToString();
                    var lines = explanation.Count(c => c.Equals('\n')) + 1;
                    var height = Text.LineHeight * lines + Text.SpaceBetweenLines * (lines - 1);

                    var viewRect = new Rect
                    {
                        width = statsExplanation.width,
                        height = height
                    };
                    Widgets.BeginScrollView(statsExplanation, ref statScrollPosition, viewRect);
                    GUI.BeginGroup(viewRect);
                    try
                    {
                        Widgets.Label(viewRect, explanation);
                    }
                    finally
                    {
                        GUI.EndGroup();
                        Widgets.EndScrollView();
                    }
                    break;
                }
                case SimulatedShooterStats simStats:
                {
                    //TODO
                    break;
                }
                default:
                {
                    Widgets.Label(content, "(None)"); // TODO 
                    break;
                }
            }
        }

        /// <summary>
        /// Draws the colonist select tab.
        /// </summary>
        /// <param name="inRect">In rect.</param>
        private void DoColonistTab(Rect inRect)
        {
            var content = inRect.Margin(MainMargin);
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
            statScrollPosition = new Vector2(0f, 0f);
        }
    }
}
