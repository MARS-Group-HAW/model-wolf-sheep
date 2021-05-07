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
            // model definition
            // all layer, agents, entities that should be part of the simulation have to be added to the model description
            var description = new ModelDescription();
            description.AddLayer<GrasslandLayer>();
            description.AddAgent<GrassGrowthAgent, GrasslandLayer>();
            description.AddAgent<Sheep, GrasslandLayer>();
            description.AddAgent<Wolf, GrasslandLayer>();

            // scenario definition
            // use config.json that holds the specification of the scenario
            var file = File.ReadAllText("config.json");
            var config = SimulationConfig.Deserialize(file);

            //use code defined config
            // var config = GenerateConfig();

            // start simulation
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
                        InstanceCount = 20,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "SheepGainFromFood", Value = 4},
                            new IndividualMapping {Name = "SheepReproduce", Value = 5},
                        }
                    },
                    new AgentMapping
                    {
                        Name = nameof(Wolf),
                        InstanceCount = 3,
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