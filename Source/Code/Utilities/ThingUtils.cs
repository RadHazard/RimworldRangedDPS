using System;
using RimWorld;
using Verse;

namespace RangedDPS.Utilities
{
    /// <summary>
    /// Various utility methods for interacting with Things
    /// </summary>
    public static class ThingUtils
    {
        /// <summary>
        /// Creates an instance of a Thing with the given def, optionally with stuff and/or quality specified
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="def">Def.</param>
        /// <param name="stuffDef">Stuff def.</param>
        /// <param name="quality">Quality.</param>
        public static Thing MakeThingFromDef(ThingDef def, ThingDef stuffDef = null, QualityCategory quality = QualityCategory.Normal)
        {
            Thing thing = ThingMaker.MakeThing(def, stuffDef ?? GenStuff.RandomStuffFor(def));
            thing.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Outsider);

            return thing;
        }

        /// <summary>
        /// Creates an instance of a Thing with the given def, optionally with stuff and/or quality specified
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="def">Def.</param>
        /// <param name="stuffDefName">Stuff def name.</param>
        /// <param name="quality">Quality.</param>
        public static Thing MakeThingFromDef(ThingDef def, string stuffDefName, QualityCategory quality = QualityCategory.Normal)
        {
            ThingDef stuff = (stuffDefName != null) ? DefDatabase<ThingDef>.GetNamed(stuffDefName) : GenStuff.RandomStuffFor(def);
            return MakeThingFromDef(def, stuff, quality);
        }

        /// <summary>
        /// Creates an instance of a Thing with the given def name, optionally with stuff and/or quality specified
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="defName">Def name.</param>
        /// <param name="stuffDefName">Stuff def name.</param>
        /// <param name="quality">Quality.</param>
        public static Thing MakeThingByName(string defName, string stuffDefName = null, QualityCategory quality = QualityCategory.Normal)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamed(defName);
            ThingDef stuff = (stuffDefName != null) ? DefDatabase<ThingDef>.GetNamed(stuffDefName) : GenStuff.RandomStuffFor(def);

            return MakeThingFromDef(def, stuff, quality);
        }
    }
}
