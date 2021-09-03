using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// World component to store data for the comparison tool
    /// </summary>
    public class CompareStorage : WorldComponent
    {
        public static CompareStorage Component => Find.World.GetComponent<CompareStorage>();

        public CompareStorage(World world) : base(world) { }

        private List<Pawn> favoritedPawns = new List<Pawn>();
        private List<Thing> favoritedWeapons = new List<Thing>();
        private List<ThingStuffPairWithQuality> favoritedFakeWeapons = new List<ThingStuffPairWithQuality>();

        public IReadOnlyList<Pawn> FavoritedPawns => favoritedPawns;
        public IReadOnlyList<Thing> FavoritedWeapons => favoritedWeapons;
        public IReadOnlyList<ThingStuffPairWithQuality> FavoritedFakeWeapons => favoritedFakeWeapons;

        /// <summary>
        /// Adds a pawn as a favorite.
        /// </summary>
        /// <param name="pawn">Pawn.</param>
        public void AddFavoritePawn(Pawn pawn) => favoritedPawns.AddDistinct(pawn);

        /// <summary>
        /// Removes a pawn from the favorites.
        /// </summary>
        /// <param name="pawn">Pawn.</param>
        public void RemoveFavoritePawn(Pawn pawn) => favoritedPawns.Remove(pawn);

        /// <summary>
        /// Adds a weapon as a favorite.
        /// </summary>
        /// <param name="weapon">Weapon.</param>
        public void AddFavoriteWeapon(Thing weapon) => favoritedWeapons.AddDistinct(weapon);

        /// <summary>
        /// Removes a weapon from the favorites.
        /// </summary>
        /// <param name="weapon">Weapon.</param>
        public void RemoveFavoriteWeapon(Thing weapon) => favoritedWeapons.Remove(weapon);

        /// <summary>
        /// Adds a fake weapon as a favorite.
        /// </summary>
        /// <param name="weapon">Weapon.</param>
        public void AddFavoriteFakeWeapon(ThingStuffPairWithQuality weapon)
        {
            if (!favoritedFakeWeapons.Contains(weapon))
                favoritedFakeWeapons.Add(weapon);
        }

        /// <summary>
        /// Removes a fake weapon from the favorites.
        /// </summary>
        /// <param name="weapon">Weapon.</param>
        public void RemoveFavoriteFakeWeapon(ThingStuffPairWithQuality weapon) => favoritedFakeWeapons.Remove(weapon);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref favoritedPawns, nameof(favoritedPawns), LookMode.Reference);
            Scribe_Collections.Look(ref favoritedWeapons, nameof(favoritedWeapons), LookMode.Reference);
            Scribe_Collections.Look(ref favoritedFakeWeapons, nameof(favoritedFakeWeapons), LookMode.Deep);
        }
    }
}