using RimWorld;
using UnityEngine;
using Verse;

namespace RangedDPS.StatUtilities
{
    /// <summary>
    /// Wrapper class that represents an arbitrary target to shoot at
    /// </summary>
    public abstract class TargetStats
    {
        /// <summary>
        /// Gets the standard reference target.
        /// Equivalent to an unarmored human not in cover
        /// </summary>
        /// <value>The standard target.</value>
        public static TargetStats StandardTarget { get; } = new SimulatedTargetStats();

        public abstract string Label { get; }

        public abstract float Size { get; }
        public abstract float Cover { get; }

        public float TotalHitFactor => Size * (1f - Cover);

        /// <summary>
        /// Calculates the effective damage reduction of this target's sharp armor against the specified armor penetration
        /// </summary>
        /// <returns>The damage reduction.</returns>
        /// <param name="armorPenetration">The armor penetration of the weapon.</param>
        public abstract float GetSharpDamageReduction(float armorPenetration = 0f);

        /// <summary>
        /// Calculates the effective damage reduction of this target's blunt armor against the specified armor penetration
        /// </summary>
        /// <returns>The damage reduction.</returns>
        /// <param name="armorPenetration">The armor penetration of the weapon.</param>
        public abstract float GetBluntDamageReduction(float armorPenetration = 0f);

        /// <summary>
        /// Calculates the effective damage reduction of this target's heat armor against the specified armor penetration
        /// </summary>
        /// <returns>The damage reduction.</returns>
        /// <param name="armorPenetration">The armor penetration of the weapon.</param>
        public abstract float GetHeatDamageReduction(float armorPenetration = 0f);


        /// <summary>
        /// Calculates the effective damage reduction of a given armor rating and a specified armor penetration
        /// </summary>
        /// <returns>The damage reduction.</returns>
        /// <param name="armorPenetration">The armor penetration of the weapon.</param>
        protected static float GetArmorDamageReduction(float armor, float armorPenetration)
        {
            float effectiveArmor = Mathf.Max(armor - armorPenetration, 0f);

            // Vanilla rolls a random float [0 - 1)
            // if x < armor/2, the attack deflects harmlessly
            // if armor/2 < x < armor, the attack does half damage
            // otherwise, it does full damage
            float blockChance = Mathf.Min(effectiveArmor / 2f, 1f);
            float mitigationChance = Mathf.Min(effectiveArmor / 2f, 1f - blockChance);
            float penetrateChance = Mathf.Max(1f - (blockChance + mitigationChance), 0f);

            return penetrateChance + (mitigationChance / 2);
        }
    }

    /// <summary>
    /// Wrapper class that represents a simulated target
    /// </summary>
    public class SimulatedTargetStats : TargetStats
    {
        private readonly float size;
        private readonly float cover;

        private readonly ArmorStats armor;

        public override string Label => $"Size {size.ToString("0.0")} target";
        public override float Size => size;
        public override float Cover => cover;

        public SimulatedTargetStats(float size = 1f, float cover = 0f, ArmorStats armor = default)
        {
            this.size = size;
            this.cover = cover;
            this.armor = armor;
        }

        public override float GetSharpDamageReduction(float armorPenetration = 0f) => GetArmorDamageReduction(armor.sharp, armorPenetration);
        public override float GetBluntDamageReduction(float armorPenetration = 0f) => GetArmorDamageReduction(armor.blunt, armorPenetration);
        public override float GetHeatDamageReduction(float armorPenetration = 0f) => GetArmorDamageReduction(armor.heat, armorPenetration);
    }

    /// <summary>
    /// Wrapper class that represents an actual pawn to shoot at
    /// </summary>
    public class PawnTargetStats : TargetStats
    {
        private readonly Pawn pawn;
        private readonly float cover;
        private readonly ArmorStats additionalArmor;

        public override string Label => pawn.LabelCap;
        public override float Size => pawn.BodySize;
        public override float Cover => cover;

        public PawnTargetStats(Pawn pawn, float cover = 0f, ArmorStats additionalArmor = default)
        {
            this.pawn = pawn;
            this.cover = cover;
            this.additionalArmor = additionalArmor;
        }

        private float GetArmorDamageReduction(StatDef armorStat, float extraArmor, float armorPenetration)
        {
            // Vanilla rolls against each individual armor piece separately, so we have to calculate them separately
            return GetArmorDamageReduction(pawn.GetStatValue(armorStat, true), armorPenetration)
                * GetArmorDamageReduction(extraArmor, armorPenetration);
        }

        // TODO check if these are the right stats
        public override float GetSharpDamageReduction(float armorPenetration = 0f) =>
                GetArmorDamageReduction(StatDefOf.ArmorRating_Sharp, additionalArmor.sharp, armorPenetration);
        public override float GetBluntDamageReduction(float armorPenetration = 0f) =>
                GetArmorDamageReduction(StatDefOf.ArmorRating_Blunt, additionalArmor.blunt, armorPenetration);
        public override float GetHeatDamageReduction(float armorPenetration = 0f) =>
                GetArmorDamageReduction(StatDefOf.ArmorRating_Heat, additionalArmor.heat, armorPenetration);
    }

    /// <summary>
    /// A struct representing the various armor stats
    /// </summary>
    public struct ArmorStats
    {
        public float sharp;
        public float blunt;
        public float heat;
    }
}
