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
        private Position _position;
        private GrasslandLayer Grassland { get; set; }

        [PropertyDescription(Name = "unregisterHandle")]
        public UnregisterAgent UnregisterHandle { get; set; }

        public string Rule { get; set; }
        public int Energy { get; set; }
        public int SheepGainFromFood { get; set; }
        public int SheepReproduce { get; set; }

        public Position Position
        {
            get => _position;
            set
            {
                //TODO delete
                if (value.X < 0 || value.Y < 0 || value.X >= 50 || value.Y >= 50)
                { 
                    throw new ApplicationException($"Position outside of bounds: {Position}");
                }

                _position = value;
            }
        }

        public void Init(GrasslandLayer layer)
        {
            Grassland = layer;

            //Spawn somewhere in the grid when the simulation starts
            RandomPosition = Grassland.FindRandomPosition();
            Position = Grassland.FindRandomPosition();
            Grassland.SheepEnvironment.Insert(this);
            Energy = RandomHelper.Random.Next(2 * SheepGainFromFood);
        }

        public void Tick()
        {
            EnergyLoss();
            Spawn(SheepReproduce);
            RandomMove();

            if (Grassland[Position] > 0)
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
                var sheep = Grassland.AgentManager.Create<Sheep>().First();
                sheep.Position = (Position) Position.Clone();
                sheep.Energy = Energy;
                Energy /= 2;
            }
        }

        /// <summary>
        ///     Sheep moves 1 step straight or diagonal(1.4243)
        /// </summary>
        private void RandomMove()
        {
            RandomPosition = Grassland.FindRandomPosition();
            Position = Grassland.SheepEnvironment.MoveTo(this, RandomPosition, 1);
        }

        public Position RandomPosition { get; set; }

        public int WantToX => (int) RandomPosition.X;
        public int WantToY => (int) RandomPosition.Y;

        // private void RandomMove()
        // {
        //     var previous = (Position) Position.Clone();
        //     var findRandomPosition = Grassland.FindRandomPosition();
        //     var newPosition = Grassland.SheepEnvironment.MoveTo(this, findRandomPosition, 1);
        //     if (newPosition.X >= 0 && newPosition.Y >= 0 && newPosition.X < 50 && newPosition.Y < 50)
        //     {
        //         Position = newPosition;
        //     }
        //     else
        //     {
        //         Position = Grassland.SheepEnvironment.MoveTo(this, previous, 1);
        //     }
        // }

        private void EatGrass()
        {
            Energy += SheepGainFromFood;
            Grassland[Position] = Math.Max(Grassland[Position] - 3, 0);
        }

        public void Kill()
        {
            // Grassland.SheepEnvironment.Remove(this);
            // UnregisterHandle.Invoke(Grassland, this);
        }

        public Guid ID { get; set; }
    }
}