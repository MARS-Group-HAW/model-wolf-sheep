using System;
using System.Linq;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics.Statistics;

namespace SheepWolfStarter.Model
{
    public class GrasslandLayer : RasterLayer
    {
        private static readonly Random Random = RandomHelper.Random;
        public SpatialHashEnvironment<Sheep> SheepEnvironment { get; private set; }
        public SpatialHashEnvironment<Wolf> WolfEnvironment { get; private set; }

        public IAgentManager AgentManager { get; private set; }

        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            var initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

            SheepEnvironment = new SpatialHashEnvironment<Sheep>(Width - 1, Height - 1);
            SheepEnvironment.CheckBoundaries = true;
            WolfEnvironment = new SpatialHashEnvironment<Wolf>(Width - 1, Height - 1);
            WolfEnvironment.CheckBoundaries = true;

            AgentManager = layerInitData.Container.Resolve<IAgentManager>();
            AgentManager.Spawn<GrassGrowthAgent, GrasslandLayer>().ToList();
            AgentManager.Spawn<Sheep, GrasslandLayer>().ToList();
            // AgentManager.Spawn<Wolf, GrasslandLayer>().ToList();

            return initiated;
        }

        public Position FindRandomPosition()
        {
            var findRandomPosition = Position.CreatePosition(Random.Next(Width - 1), Random.Next(Height - 1));
            if (findRandomPosition.X < 0 || findRandomPosition.Y < 0)
            {
                int i = 0;
            }

            if (findRandomPosition.X == 50 || findRandomPosition.Y == 50)
            {
                int i = 0;
            } //TODO delete

            return findRandomPosition;
        }
    }
}