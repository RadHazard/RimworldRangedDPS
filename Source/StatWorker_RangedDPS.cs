﻿using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedDPS : StatWorker_RangedDPSBase
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return 0f;
            }

            var shootVerb = GetShootVerb(thingDef);
            if (shootVerb == null)
            {
                return 0f;
            }

            float rawDps = GetRawDPS(shootVerb, req.Thing);

            float bestAccuracy = new[] {
                req.Thing.GetStatValue(StatDefOf.AccuracyTouch),
                req.Thing.GetStatValue(StatDefOf.AccuracyShort),
                req.Thing.GetStatValue(StatDefOf.AccuracyMedium),
                req.Thing.GetStatValue(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * bestAccuracy;
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return null;
            }

            var shootVerb = GetShootVerb(thingDef);
            if (shootVerb == null)
            {
                return null;
            }

            float rawDps = GetRawDPS(shootVerb, req.Thing);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            // Min Range
            float minRange = Math.Max(shootVerb.minRange, 3f);
            float minRangeHitChance = shootVerb.GetHitChanceFactor(req.Thing, minRange);
            float minRangeDps = rawDps * minRangeHitChance;
            stringBuilder.AppendLine(FormatDPSRangeString(minRange, minRangeDps, minRangeHitChance));

            // Ranges between Min - Max, in steps of 5
            float startRange = (float)Math.Ceiling(minRange / 5) * 5;
            for (float i = startRange; i <= shootVerb.range; i += 5)
            {
                float hitChance = shootVerb.GetHitChanceFactor(req.Thing, i);
                float dps = rawDps * hitChance;
                stringBuilder.AppendLine(FormatDPSRangeString(i, dps, hitChance));
            }

            // Max Range
            float maxRangeHitChance = shootVerb.GetHitChanceFactor(req.Thing, shootVerb.range);
            float maxRangeDps = rawDps * maxRangeHitChance;
            stringBuilder.AppendLine(FormatDPSRangeString(shootVerb.range, maxRangeDps, maxRangeHitChance));

            return stringBuilder.ToString();
        }

    }
}
