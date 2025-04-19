using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace AbandonedBuildingRemover_ReduxVS
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(AbandonedBuildingRemover_ReduxVS)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public static object Log { get; internal set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            //updateSystem.UpdateAfter<AbandonedBuildingRemoverSystem>(SystemUpdatePhase.Deserialize);
            updateSystem.UpdateAfter<AbandonedBuildingRemover.AbandonedBuildingRemoverSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
