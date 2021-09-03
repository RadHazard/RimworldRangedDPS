using System;
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
        public override void DoWindowContents(Rect inRect)
        {
            throw new NotImplementedException();
        }
    }
}
