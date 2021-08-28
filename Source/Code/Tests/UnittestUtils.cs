#if DEBUG
using System;
using System.Linq;
using RimTest;
using RimWorld;
using Verse;

namespace RangedDPS.Tests
{
    public static class UnittestUtils
    {
        private const float DELTA = 0.0001f;

        public static Thing GetThingByName(string defName, string stuffDefName = null, QualityCategory quality = QualityCategory.Normal)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamed(defName);
            ThingDef stuff = (stuffDefName != null) ? DefDatabase<ThingDef>.GetNamed(stuffDefName) : GenStuff.RandomStuffFor(def);

            Thing thing = ThingMaker.MakeThing(def, stuff);
            thing.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Outsider);

            return thing;
        }

        public static Pawn GetTestPawn(int shootSkill = 10)
        {
            PawnKindDef kindDef = DefDatabase<PawnKindDef>.GetNamed("StrangerInBlack");
            Faction faction = FactionUtility.DefaultFactionFrom(kindDef.defaultFactionType);

            TraitDef shootingAccuracy = DefDatabase<TraitDef>.GetNamed("ShootingAccuracy");

            Pawn pawn = PawnGenerator.GeneratePawn(kindDef, faction);
            while (pawn.story.traits.HasTrait(shootingAccuracy))
            {
                pawn = PawnGenerator.GeneratePawn(kindDef, faction);
            } 

            pawn.health.Reset();

            var shooting = (from skill in pawn.skills.skills
                            where skill.def.defName.Equals("Shooting")
                            select skill).First();
            shooting.Level = 0;
            shooting.EnsureMinLevelWithMargin(shootSkill);

            return pawn;
        }

        public static void AddTrait(this Pawn pawn, string defName, int degree)
        {
            TraitDef traitDef = DefDatabase<TraitDef>.GetNamed(defName);
            Trait trait = new Trait(traitDef, degree, false);
            pawn.story?.traits.GainTrait(trait);
        }

        public static void Approximately(this AssertValue assert, float val)
        {
            assert.Approximately(val, DELTA);
        }

        public static void Approximately(this AssertValue assert, float val, float delta)
        {
            assert.BetweenInclusive(val - delta, val + delta);
        }
    }
}
#endif