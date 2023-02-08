using JetBrains.Annotations;
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
        /// <param name="stuffDef">(Optional) Stuff def. Defaults to the default stuff.</param>
        /// <param name="quality">(Optional) Quality. Defaults to normal.</param>
        [PublicAPI]
        public static Thing MakeThingFromDef(ThingDef def, ThingDef? stuffDef = null, QualityCategory quality = QualityCategory.Normal)
        {
            var thing = ThingMaker.MakeThing(def, stuffDef ?? GenStuff.DefaultStuffFor(def));
            thing.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Outsider);

            return thing;
        }

        /// <summary>
        /// Creates an instance of a Thing with the given def, optionally with stuff and/or quality specified
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="def">Def.</param>
        /// <param name="stuffDefName">(Optional) Stuff def name. Defaults to the default stuff.</param>
        /// <param name="quality">(Optional) Quality. Defaults to normal.</param>
        [PublicAPI]
        public static Thing MakeThingFromDef(ThingDef def, string? stuffDefName = null, QualityCategory quality = QualityCategory.Normal)
        {
            var stuff = stuffDefName != null ? DefDatabase<ThingDef>.GetNamed(stuffDefName) : null;
            return MakeThingFromDef(def, stuff, quality);
        }

        /// <summary>
        /// Creates an instance of a Thing with the given def name, optionally with stuff and/or quality specified
        /// </summary>
        /// <returns>The thing.</returns>
        /// <param name="defName">Def name.</param>
        /// <param name="stuffDefName">(Optional) Stuff def name. Defaults to the default stuff.</param>
        /// <param name="quality">(Optional) Quality.</param>
        [PublicAPI]
        public static Thing MakeThingByName(string defName, string? stuffDefName = null, QualityCategory quality = QualityCategory.Normal)
        {
            var def = DefDatabase<ThingDef>.GetNamed(defName);
            var stuff = stuffDefName != null ? DefDatabase<ThingDef>.GetNamed(stuffDefName) : null;

            return MakeThingFromDef(def, stuff, quality);
        }
    }
}
