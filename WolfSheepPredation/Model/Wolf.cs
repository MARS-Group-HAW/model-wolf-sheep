using System;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Numerics;
using Mars.Numerics.Statistics;

namespace SheepWolfStarter.Model
{
    public class Wolf : IAgent<GrasslandLayer>, IPositionable
    {
        private GrasslandLayer Grassland { get; set; }

        public void Init(GrasslandLayer layer)
        {
            Grassland = layer;
            
            //Spawn somewhere in the grid when the simulation starts
            var random = RandomHelper.Random;
            Position = Grassland.FindRandomPosition();
            Grassland.WolfEnvironment.Insert(this);
            Energy = random.Next(2 * WolfGainFromFood);
        }

        [PropertyDescription (Name = "unregisterHandle")]
        public UnregisterAgent UnregisterHandle { get; set; }

        public Position Position { get; set; }

        public int WolfGainFromFood { get; set; }
        public int WolfReproduce { get; set; }

        public string Rule { get; set; }
        public int Energy { get; set; }

        public void Tick()
        {
            EnergyLoss();
            Spawn(WolfReproduce);
            RandomMove();
            var target = Grassland.SheepEnvironment.Explore(Position).FirstOrDefault();
            if (target != null)
            {
                var targetDistance = Distance.Chebyshev(Position.PositionArray, target.Position.PositionArray);
                if (targetDistance < 1)
                {
                    Rule = "R3 - Eat Sheep";
                    EatSheep(target);
                }
                else
                {
                    Rule = "R4 - No sheep on my point";
                    // Position = Grassland.WolfEnvironment.MoveTo(this, target.Position, 2);
                    
                    var previous = (Position) Position.Clone();
                    var newPosition = Grassland.WolfEnvironment.MoveTo(this, target.Position, 2);
                    if (newPosition.X >= 0 && newPosition.Y >= 0 && newPosition.X < 50 && newPosition.Y < 50)
                    {
                        Position = newPosition;
                    }
                    else
                    {
                        Position = Grassland.WolfEnvironment.MoveTo(this, previous, 1);
                    }
                    
                }
            }
            else
            {
                Rule = "R5 - No more sheep exist";
            }
        }

        private void EnergyLoss()
        {
            Energy -= 1;
            if (Energy <= 0)
            {
                Grassland.WolfEnvironment.Remove(this);
                UnregisterHandle.Invoke(Grassland, this);
            }
        }

        private void RandomMove()
        {
            Position = Grassland.WolfEnvironment.MoveTo(this, Grassland.FindRandomPosition(), 3);
        }

        // private void RandomMove()
        // {
        //     var previous = (Position) Position.Clone();
        //     var findRandomPosition = Grassland.FindRandomPosition();
        //     var newPosition = Grassland.WolfEnvironment.MoveTo(this, findRandomPosition, 3);
        //     if (newPosition.X >= 0 && newPosition.Y >= 0 && newPosition.X < 50 && newPosition.Y < 50)
        //     {
        //         Position = newPosition;
        //     }
        //     else
        //     {
        //         Position = Grassland.WolfEnvironment.MoveTo(this, previous, 1);
        //     }
        // }
        
        private void EatSheep(Sheep sheep)
        {
            Rule = "R6 - Sheep killed!";
            Energy += WolfGainFromFood;
            sheep.Kill();
        }

        private void Spawn(int percent)
        {
            if (RandomHelper.Random.Next(100) < percent)
            {
                var wolf = Grassland.AgentManager.Create<Wolf>().First();
                wolf.Position = (Position) Position.Clone();
                wolf.Energy = Energy;
                Energy /= 2;
            }
        }

        public Guid ID { get; set; }
    }
}