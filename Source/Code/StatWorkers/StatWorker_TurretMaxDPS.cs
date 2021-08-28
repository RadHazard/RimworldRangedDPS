using RimWorld;

namespace RangedDPS
{

    public class StatWorker_TurretMaxDPS : StatWorker_TurretDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            return GetTurretStats(req).GetRawDPS();
        }
    }
}
