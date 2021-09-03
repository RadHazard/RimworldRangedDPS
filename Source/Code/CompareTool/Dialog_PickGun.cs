using System;
using UnityEngine;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// A selection dialogue to pick a gun
    /// 
    /// TODO
    /// Weapon selection
    ///- Real weapons should be selectable.
    ///  - Add a button somewhere in the weapon UI to add/"favorite" a weapon?
    ///  - Add a tool to click a weapon on the map?
    ///- Fake weapons should also be selectable.
    ///  - Allow searching by label or def name.
    ///  - Add selectable quality and stuff, but only for weapons with quality/stuff.
    ///  - Fake weapons should be saved to the "favorites" screen until removed
    /// 
    /// </summary>
    public class Dialog_PickGun : Window
    {
        public override void DoWindowContents(Rect inRect)
        {
            throw new NotImplementedException();
        }
        //private Pawn pawn;

        //private string curName;

        //private string curTitle;

        //private Name CurPawnName
        //{
        //    get
        //    {
        //        NameTriple nameTriple = pawn.Name as NameTriple;
        //        if (nameTriple != null)
        //        {
        //            return new NameTriple(nameTriple.First, curName, nameTriple.Last);
        //        }
        //        if (pawn.Name is NameSingle)
        //        {
        //            return new NameSingle(curName, false);
        //        }
        //        throw new InvalidOperationException();
        //    }
        //}

        //public override Vector2 InitialSize => new Vector2(500f, 175f);

        //public Dialog_PickGun(Pawn pawn)
        //{
        //    this.pawn = pawn;
        //    curName = pawn.Name.ToStringShort;
        //    if (pawn.story != null)
        //    {
        //        if (pawn.story.title != null)
        //        {
        //            curTitle = pawn.story.title;
        //        }
        //        else
        //        {
        //            curTitle = "";
        //        }
        //    }
        //    forcePause = true;
        //    absorbInputAroundWindow = true;
        //    closeOnClickedOutside = true;
        //    closeOnAccept = false;
        //}

        //public override void DoWindowContents(Rect inRect)
        //{
        //    bool flag = false;
        //    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        //    {
        //        flag = true;
        //        Event.current.Use();
        //    }
        //    Text.Font = GameFont.Medium;
        //    string text = CurPawnName.ToString().Replace(" '' ", " ");
        //    if (curTitle == "")
        //    {
        //        text = text + ", " + pawn.story.TitleDefaultCap;
        //    }
        //    else if (curTitle != null)
        //    {
        //        text = text + ", " + curTitle.CapitalizeFirst();
        //    }
        //    Widgets.Label(new Rect(15f, 15f, 500f, 50f), text);
        //    Text.Font = GameFont.Small;
        //    string text2 = Widgets.TextField(new Rect(15f, 50f, inRect.width / 2f - 20f, 35f), curName);
        //    if (text2.Length < 16 && CharacterCardUtility.ValidNameRegex.IsMatch(text2))
        //    {
        //        curName = text2;
        //    }
        //    if (curTitle != null)
        //    {
        //        string text3 = Widgets.TextField(new Rect(inRect.width / 2f, 50f, inRect.width / 2f - 20f, 35f), curTitle);
        //        if (text3.Length < 25 && CharacterCardUtility.ValidNameRegex.IsMatch(text3))
        //        {
        //            curTitle = text3;
        //        }
        //    }
        //    if (Widgets.ButtonText(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "OK", true, true, true) | flag)
        //    {
        //        if (string.IsNullOrEmpty(curName))
        //        {
        //            curName = ((NameTriple)pawn.Name).First;
        //        }
        //        pawn.Name = CurPawnName;
        //        if (pawn.story != null)
        //        {
        //            pawn.story.Title = curTitle;
        //        }
        //        Find.WindowStack.TryRemove(this, true);
        //        Messages.Message(pawn.def.race.Animal ? "AnimalGainsName".Translate(curName) : "PawnGainsName".Translate(curName, pawn.story.Title, pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true), pawn, MessageTypeDefOf.PositiveEvent, false);
        //    }
        //}
    }
}
