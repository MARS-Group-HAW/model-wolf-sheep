using System;
using System.Linq;
using Mars.Common.Core;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics.Statistics;

namespace WolfSheepGrassPredation.Model
{
    /// <summary>
    ///     Sheep walk around by chance.
    ///     If grass is under the sheep, it eats the grass. Otherwise do nothing, this tick.
    ///     Every few rounds a new sheep is spawned, which receives half of the energy  
    /// </summary>
    public class Sheep : IAgent<GrasslandLayer>, IPositionable
    {
        [PropertyDescription]
        public UnregisterAgent UnregisterHandle { get; set; }

        [PropertyDescription]
        public int SheepGainFromFood { get; set; }

        [PropertyDescription]
        public int SheepReproduce { get; set; }

        [PropertyDescription]
        public int Xpos { get; set; }

        [PropertyDescription]
        public int Ypos { get; set; }

        [PropertyDescription]
        public int InitEnergy { get; set; }

        public void Init(GrasslandLayer layer)
        {
            _grassland = layer;

            if (InitEnergy > 0)
            {
                // Spawn at predefined position
                Energy = InitEnergy;
                Position = Position.CreatePosition(Xpos, Ypos);
            }
            else
            {
                // Spawn somewhere on the grid
                Energy = RandomHelper.Random.Next(2 * SheepGainFromFood);
                Position = _grassland.FindRandomPosition();
            }

            _grassland.SheepEnvironment.Insert(this);
        }

        private GrasslandLayer _grassland;

        public Position Position { get; set; }

        public string Type => "Sheep";

        public string Rule { get; private set; }
        public int Energy { get; private set; }

        public void Tick()
        {
            EnergyLoss();
            Spawn(SheepReproduce);
            RandomMove();

            if (_grassland[Position] > 0)
            {
                Rule = "R1 - Eat grass";
                EatGrass();
            }
            else
            {
                Rule = "R2 - No food found";
            }
        }

        private void EnergyLoss()
        {
            Energy -= 1;
            if (Energy <= 0)
            {
                Kill();
            }
        }

        private void Spawn(int percent)
        {
            if (RandomHelper.Random.Next(100) < percent)
            {
                var sheep = _grassland.AgentManager.Spawn<Sheep, GrasslandLayer>().First();
                sheep.Position = (Position) Position.Clone();
                _grassland.SheepEnvironment.PosAt(sheep, sheep.Position.PositionArray);
                sheep.Energy = Energy;
                Energy /= 2;
            }
        }

        private void RandomMove()
        {
            //Sheep moves 1 step straight or diagonal(1.4243)
            var bearing = RandomHelper.Random.Next(360);
            Position = _grassland.SheepEnvironment.MoveTowards(this, bearing, 1);
        }

        private void EatGrass()
        {
            var grass = _grassland.Grasses[Position.X.Value<int>(), Position.Y.Value<int>()];
            Energy += grass.Eat(SheepGainFromFood);
        }

        public void Kill()
        {
            _grassland.SheepEnvironment.Remove(this);
            UnregisterHandle.Invoke(_grassland, this);
        }

        public Guid ID { get; set; }
    }
}