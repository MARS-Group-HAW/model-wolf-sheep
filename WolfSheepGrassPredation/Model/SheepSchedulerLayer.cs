using Mars.Components.Layers;

namespace WolfSheepGrassPredation.Model
{
    /// <summary>
    ///     Creates new <see cref="Sheep"/> periodically. See referenced file in simulation config.
    /// </summary>
    public class SheepSchedulerLayer : AgentSchedulerLayer<Sheep, GrasslandLayer>
    {
    }
}