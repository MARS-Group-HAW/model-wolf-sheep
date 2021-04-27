using System;
using System.Collections.Generic;
using System.IO;
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


            // use config.json 
            // var file = File.ReadAllText("config.json");
            // var config = SimulationConfig.Deserialize(file);

            //use code defined config
            var config = GenerateConfig();

            var starter = SimulationStarter.Start(description, config);
            var handle = starter.Run();
            Console.WriteLine("Successfully executed iterations: " + handle.Iterations);
            starter.Dispose();
        }

        private static SimulationConfig GenerateConfig()
        {
            return new SimulationConfig
            {
                Globals =
                {
                    Steps = 2,
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
                        OutputTarget = OutputTargetType.SqLite,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "SheepGainFromFood", Value = 4},
                            new IndividualMapping {Name = "SheepReproduce", Value = 5},
                        }
                    },
                    new AgentMapping
                    {
                        Name = nameof(Wolf),
                        InstanceCount = 0,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "WolfGainFromFood", Value = 30},
                            new IndividualMapping {Name = "WolfReproduce", Value = 10},
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
        }
    }
}