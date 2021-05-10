using System;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics.Statistics;

namespace SheepWolfStarter.Model
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

        public void Init(GrasslandLayer layer)
        {
            _grassland = layer;

            if (Energy == 0)
                Energy = 2 * RandomHelper.Random.Next(SheepGainFromFood) + SheepGainFromFood;

            //Spawn somewhere in the grid when the simulation starts or use given Position
            Position ??= _grassland.FindRandomPosition();
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
            Energy -= 2;
            if (Energy <= 0)
            {
                Kill();
            }
        }

        private void Spawn(int percent)
        {
            if (RandomHelper.Random.Next(100) < percent)
            {
               _grassland.AgentManager.Spawn<Sheep, GrasslandLayer>(null, agent =>
                {
                    agent.Position = Position.CreatePosition(Position.X, Position.Y);
                    agent.Energy = Energy / 2;
                }).Take(1).First();
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
            var consumption = _grassland[Position] > SheepGainFromFood
                ? SheepGainFromFood
                : _grassland[Position] - SheepGainFromFood;
            Energy += (int) consumption;
            _grassland[Position] -= consumption;
        }

        public void Kill()
        {
            _grassland.SheepEnvironment.Remove(this);
            UnregisterHandle.Invoke(_grassland, this);
        }

        public Guid ID { get; set; }
    }
}