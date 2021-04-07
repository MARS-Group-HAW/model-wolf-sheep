using System;
using System.Collections.Generic;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using SheepWolfStarter.Model;

namespace SheepWolfStarter
{
    internal static class Program
    {
        private static void Main()
        {
            var description = new ModelDescription();
            description.AddLayer<GrasslandLayer>();
            description.AddAgent<GrassGrowthAgent, GrasslandLayer>();
            description.AddAgent<Sheep, GrasslandLayer>();
            description.AddAgent<Wolf, GrasslandLayer>();

            var config = new SimulationConfig
            {
                Globals =
                {
                    Steps = 100,
                    OutputTarget = OutputTargetType.Csv,
                    CsvOptions =
                    {
                        Delimiter = ";",
                        NumberFormat = "en-EN"
                    }
                },
                LayerMappings = new List<LayerMapping>
                {
                    new LayerMapping
                    {
                        Name = nameof(GrasslandLayer),
                        File = "Resources/grid.csv"
                    }
                },
                AgentMappings = new List<AgentMapping>
                {
                    new AgentMapping
                    {
                        Name = nameof(Sheep),
                        InstanceCount = 1,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "SheepGainFromFood", Value = 4},
                            new IndividualMapping {Name = "SheepReproduce", Value = 4},
                        }
                    },
                    new AgentMapping
                    {
                        Name = nameof(Wolf),
                        InstanceCount = 0,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "WolfGainFromFood", Value = 20},
                            new IndividualMapping {Name = "WolfReproduce", Value = 5},
                        }
                    },
                    new AgentMapping
                    {
                        Name = nameof(GrassGrowthAgent),
                        InstanceCount = 1,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "GrassRegrowthPerStep", Value = 1}
                        }
                    }
                }
            };
            
            var starter = SimulationStarter.Start(description, config);
            var handle = starter.Run();
            Console.WriteLine("Successfully executed iterations: " + handle.Iterations);
            starter.Dispose();
        }
    }
}
    