using System;
using UnityEngine;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// A selection dialogue to pick a gun
    /// 
    /// TODO
    ///  Turret selection
    /// - Turrets should be handled differently than pawns/shooters
    /// - Selecting a turret is equivalent to pawn + shooter
    /// - All turrets should be listed(including mech-only turrets)
    /// - Loadable weapons should have selectable ammo?
    /// - Searchable by name or defname
    /// 
    /// </summary>
    public class Dialog_PickTurret : Window
    {
        public override void DoWindowContents(Rect inRect)
        {
            throw new NotImplementedException();
        }
    }
}
