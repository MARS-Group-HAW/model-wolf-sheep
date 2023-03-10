using System;
using Mars.Common.Core;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace WolfSheepGrassPredation.Model
{
    public class Grass : IAgent<GrasslandLayer>
    {
        public double Regrowth { get; set; }
        public double InitValue { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Init(GrasslandLayer layer)
        {
            Grassland = layer;
            Energy = InitValue;
            Position = Position.CreatePosition(X, Y);
        }

        public double Energy { get; set; }
        public Position Position { get; set; }

        private GrasslandLayer Grassland { get; set; }

        public void Tick()
        {
            Energy += Regrowth;

            if (Grassland.PrecipitationLayer.IsInRaster(Position))
            {
                var precipitation = Grassland.PrecipitationLayer.GetValueByGeoPosition(Position);
                if (precipitation > 0)
                {
                    //Regrowth is higher
                    Energy += precipitation / 100;
                }
            }
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public int Eat(int consumption)
        {
            if (consumption > Energy)
            {
                var result = Energy;
                Energy = 0;
                return result.Value<int>();
            }
            else
            {
                Energy -= consumption;
                return consumption;
            }
        }
    }
}