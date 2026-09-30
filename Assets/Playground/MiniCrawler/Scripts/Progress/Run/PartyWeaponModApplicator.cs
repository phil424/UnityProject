using MiniCrawler.WeaponMods;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class PartyWeaponModApplicator
    {
        public static void Apply(GameObject spawnedActor, RunBuild build)
        {
            if (spawnedActor == null || build == null)
                return;

            WeaponModRuntimeState runtimeState = spawnedActor.GetComponent<WeaponModRuntimeState>();

            if (runtimeState == null)
                runtimeState = spawnedActor.AddComponent<WeaponModRuntimeState>();

            runtimeState.Configure(build.WeaponMods);
        }
    }
}