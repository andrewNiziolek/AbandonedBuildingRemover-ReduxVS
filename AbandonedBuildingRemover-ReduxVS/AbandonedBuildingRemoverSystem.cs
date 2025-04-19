using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Areas;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Game.Tools;

namespace AbandonedBuildingRemover
{
    [RequireMatchingQueriesForUpdate]
    public partial class AbandonedBuildingRemoverSystem : GameSystemBase
    {
        private EntityQuery _abandonedBuildingQuery;
        private EndSimulationEntityCommandBufferSystem _endSimEcb;

        protected override void OnCreate()
        {
            base.OnCreate();

            _abandonedBuildingQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Abandoned>(),
                    ComponentType.ReadOnly<Building>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });

            _endSimEcb = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();

            RequireForUpdate(_abandonedBuildingQuery);
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimEcb.CreateCommandBuffer();
            var subAreaLookup = GetBufferLookup<SubArea>(true);
            var subLaneLookup = GetBufferLookup<SubLane>(true);
            var subNetLookup = GetBufferLookup<SubNet>(true);

            Entities
                .WithStoreEntityQueryInField(ref _abandonedBuildingQuery)
                .ForEach((Entity entity) =>
                {
                    if (subAreaLookup.TryGetBuffer(entity, out var subAreas))
                    {
                        foreach (var subArea in subAreas)
                        {
                            ecb.AddComponent<Deleted>(subArea.m_Area);
                        }
                    }

                    if (subLaneLookup.TryGetBuffer(entity, out var subLanes))
                    {
                        foreach (var subLane in subLanes)
                        {
                            ecb.AddComponent<Deleted>(subLane.m_SubLane);
                        }
                    }

                    if (subNetLookup.TryGetBuffer(entity, out var subNets))
                    {
                        foreach (var subNet in subNets)
                        {
                            ecb.AddComponent<Deleted>(subNet.m_SubNet);
                        }
                    }

                    ecb.AddComponent<Deleted>(entity);
                }).ScheduleParallel();

            _endSimEcb.AddJobHandleForProducer(Dependency);
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => phase == SystemUpdatePhase.GameSimulation ? 16 : 1;
    }
}
