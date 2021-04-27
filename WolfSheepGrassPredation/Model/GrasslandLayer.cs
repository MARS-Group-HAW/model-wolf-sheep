using System.Linq;
using System.Threading;
using Mars.Common.Core.Collections;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics.Statistics;

namespace WolfSheepGrassPredation.Model
{
    public class GrasslandLayer : RasterLayer, ISteppedActiveLayer
    {
        [PropertyDescription]
        public PrecipitationLayer PrecipitationLayer { get; set; }

        /// <summary>
        ///     Holds all sheep in a grid environment.
        /// </summary>
        public SpatialHashEnvironment<Sheep> SheepEnvironment { get; private set; }

        /// <summary>
        ///     Holds all wolves in a grid environment.
        /// </summary>
        public SpatialHashEnvironment<Wolf> WolfEnvironment { get; private set; }
        
        /// <summary>
        ///     Holds all grass agents in a 2-dimensional area.
        /// </summary>
        public Grass[,] Grasses;

        /// <summary>
        ///     Responsible to create new agents and initialize them with required dependencies
        /// </summary>
        public IAgentManager AgentManager { get; private set; }

        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            var initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

            SheepEnvironment = new SpatialHashEnvironment<Sheep>(Width - 1, Height - 1) {CheckBoundaries = true};
            WolfEnvironment = new SpatialHashEnvironment<Wolf>(Width - 1, Height - 1) {CheckBoundaries = true};

            // Initially create agents. For periodical creation check SheepSchedulerLayer.
            // For model dependent creation inspect Sheeps and Wolfs reproduce method.
            AgentManager = layerInitData.Container.Resolve<IAgentManager>();
            AgentManager.Spawn<Sheep, GrasslandLayer>().ToList();
            AgentManager.Spawn<Wolf, GrasslandLayer>().ToList();
            //ToList() actually requests and therefore instanciates agents provided by the "Spawn"-stream.

            Grasses = new Grass[Width, Height];
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var initValue = this[x, y];
                    // create and initialize manually
                    // var grass = new Grass {X = x, Y = y, InitValue = initValue, Regrowth = 1};
                    // grass.Init(this);
                    // registerAgentHandle.Invoke(this, grass);

                    // let agent manager handle the creation (and registration, using Spawn instead of Create)
                    var grass = AgentManager.Create<Grass, GrasslandLayer>(null, agent =>
                        {
                            agent.X = x;
                            agent.Y = y;
                            agent.InitValue = initValue;
                            agent.Regrowth = 1;
                        })
                        .Do(agent => registerAgentHandle.Invoke(this, agent)).Take(1).First();

                    Grasses[x, y] = grass;
                }
            }

            return initiated;
        }

        public Position FindRandomPosition()
        {
            var random = RandomHelper.Random;
            return Position.CreatePosition(random.Next(Width - 1), random.Next(Height - 1));
        }

        public void Tick()
        {
            //regrow grass? => now not necessary because Grass-Agent takes care of it
            Thread.Sleep(100);
        }
        
        public void PreTick()
        {
            //do nothing
        }

        public void PostTick()
        {
            //do nothing
        }
    }
}