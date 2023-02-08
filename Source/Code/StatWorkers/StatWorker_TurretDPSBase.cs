using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS.StatWorkers
{
    public abstract class StatWorker_TurretDPSBase : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!(req.Def is ThingDef thingDef && (thingDef.building?.turretGunDef?.IsRangedWeapon ?? false)))
                return false;

            // Don't show DPS for unloaded mortars
            var comp = GetTurretWeapon(req).TryGetComp<CompChangeableProjectile>();
            if (comp != null)
                return comp.Loaded;

            return true;
        }

        protected static Building_TurretGun GetTurret(StatRequest req)
        {
            if (req.Thing is Building_TurretGun turret)
                return turret;

            if ((req.Def as ThingDef)?.GetConcreteExample() is Building_TurretGun defTurret)
                return defTurret;
            
            Log.Error($"[RangedDPS] Tried to get turret stats for {req.Def.defName}, which is not a turret");
            return null!;
        }

        protected static Thing GetTurretWeapon(StatRequest req)
        {
            return GetTurret(req).gun;
        }

        protected static TurretGunStats GetTurretStats(StatRequest req)
        {
            return GetTurretStats(GetTurret(req));
        }

        /// <summary>
        /// Calculates a stats breakdown of the given turret.
        /// Logs an error and returns null if the thing is null.
        /// </summary>
        /// <returns>The stats of the passed-in turret.</returns>
        /// <param name="turret">The turret to get stats for.</param>
        protected static TurretGunStats GetTurretStats(Building_TurretGun? turret)
        {
            if (turret == null)
            {
                Log.Error($"[RangedDPS] Tried to get the ranged weapon stats of a null turret");
                return null!;
            }

            return new TurretGunStats(turret);
        }
    }
}
