using System;
using System.Linq;
using Mars.Common;
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
        [PropertyDescription]
        public UnregisterAgent UnregisterHandle { get; set; }

        [PropertyDescription]
        public int WolfGainFromFood { get; set; }

        [PropertyDescription]
        public int WolfReproduce { get; set; }

        public void Init(GrasslandLayer layer)
        {
            _grassland = layer;

            //Spawn somewhere in the grid when the simulation starts
            var random = RandomHelper.Random;
            Position = _grassland.FindRandomPosition();
            _grassland.WolfEnvironment.Insert(this);
            Energy = random.Next(2 * WolfGainFromFood) + WolfGainFromFood;
        }

        private GrasslandLayer _grassland;

        public Position Position { get; set; }

        public string Type => "Wolf";
        public string Rule { get; private set; }
        public int Energy { get; private set; }

        public void Tick()
        {
            EnergyLoss();
            Spawn(WolfReproduce);

            var target = _grassland.SheepEnvironment.Explore(Position).FirstOrDefault();
            if (target != null)
            {
                var distance = (int) Distance.Euclidean(Position.PositionArray, target.Position.PositionArray);
                TargetDiff = Math.Abs(TargetDistance - distance);
                TargetDistance = distance;
                if (TargetDistance <= 3)
                {
                    Rule = "R3 - Eat Sheep";
                    EatSheep(target);
                }
                else //if (TargetDistance < 20)
                {
                    Rule = $"R4 - Move towards sheep: {TargetDistance} tiles away";
                    MoveTowardsTarget(target);
                }
                // else
                // {
                //     Rule = "R5 - No sheep near by - random move";
                //     RandomMove();
                // }
            }
            else
            {
                Rule = "R6 - No more sheep exist";
            }
        }

        private void MoveTowardsTarget(Sheep target)
        {
            var bearing = Position.GetBearing(target.Position);
            Position = _grassland.WolfEnvironment.MoveTowards(this, bearing, 3);
        }

        public int TargetDiff { get; set; }
        public int TargetDistance { get; set; }

        private void EnergyLoss()
        {
            Energy -= 1;
            if (Energy <= 0)
            {
                _grassland.WolfEnvironment.Remove(this);
                UnregisterHandle.Invoke(_grassland, this);
            }
        }

        private void RandomMove()
        {
            var bearing = RandomHelper.Random.Next(360);
            Position = _grassland.WolfEnvironment.MoveTowards(this, bearing, 3);
        }

        private void EatSheep(Sheep sheep)
        {
            Rule = "R7 - Sheep killed!";
            Energy += WolfGainFromFood;
            sheep.Kill();
        }

        private void Spawn(int percent)
        {
            if (RandomHelper.Random.Next(100) < percent)
            {
                var wolf = _grassland.AgentManager.Spawn<Wolf, GrasslandLayer>().First();
                wolf.Position = (Position) Position.Clone();
                _grassland.WolfEnvironment.PosAt(wolf, wolf.Position.PositionArray);
                wolf.Energy = Energy;
                Energy /= 2;
            }
        }

        public Guid ID { get; set; }
    }
}