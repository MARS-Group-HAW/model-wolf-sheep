using System;
using System.Collections.Generic;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using WolfSheepGrassPredation.Model;

namespace WolfSheepGrassPredation
{
    internal static class Program
    {
        private static void Main()
        {
            var description = new ModelDescription();
            description.AddLayer<GrasslandLayer>();
            description.AddLayer<PrecipitationLayer>();
            description.AddLayer<SheepSchedulerLayer>();
            
            description.AddAgent<Grass, GrasslandLayer>();
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
                    StartPoint = DateTime.Parse("2018-01-01T00:00:00"),
                    EndPoint = DateTime.Parse("2018-04-01T00:00:00"),
                    DeltaTUnit = TimeSpanUnit.Days,
                    OutputTarget = OutputTargetType.None,
                    CsvOptions =
                    {
                        Delimiter = ";",
                        NumberFormat = "en-EN"
                    },
                    ShowConsoleProgress = true,
                },
                LayerMappings = new List<LayerMapping>
                {
                    new LayerMapping
                    {
                        Name = nameof(GrasslandLayer),
                        File = "Resources/grid.csv"
                    },
                    new LayerMapping
                    {
                        Name = nameof(PrecipitationLayer),
                        File = "Resources/prec_wp.zip"
                    },
                    new LayerMapping
                    {
                        Name = nameof(SheepSchedulerLayer),
                        File = "Resources/sheep_schedule.csv"
                    }
                },
                AgentMappings = new List<AgentMapping>
                {
                    new AgentMapping
                    {
                        Name = nameof(Sheep),
                        File = "Resources/sheep.csv",
                        InstanceCount = 3,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "SheepGainFromFood", Value = 4},
                            new IndividualMapping {Name = "SheepReproduce", Value = 5},
                        },
                        OutputTarget = OutputTargetType.SqLite,
                        OutputFilter = new List<OutputFilter>
                        { // only write tick = 3 into the result output
                            new OutputFilter
                            {
                                ParameterName = "Tick",
                                Values = new object[]{3},
                                Operator = ContainsOperator.In
                            }
                        }
                    },
                    new AgentMapping
                    {
                        Name = nameof(Wolf),
                        InstanceCount = 1,
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new IndividualMapping {Name = "WolfGainFromFood", Value = 30},
                            new IndividualMapping {Name = "WolfReproduce", Value = 10},
                        }
                    }
                }
            };
        }
    }
}