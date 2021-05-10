# Model wolf sheep

The `Wolf-Sheep-Predation` model is a simple predator-prey model that is easily understandable both by its semantic
concept and by its source code implementation. Sheep move around randomly and consume grass, which gives them energy.
Each tick the sheep consumes energy and if the energy is turning zero, the sheep dies. The more sheep exist in the same
region, the more like it is that they die by insufficient energy. Eventually sheep reproduce by creating a new sheep at
the same position but suffering from a big energy loss. Wolves act quite similar to sheep (energy loss, reproduction).
To gain energy, they hunt sheep instead of eating grass. If a sheep is nearby, a wolf follows it and kills it on close
range.

## Starting

Download the project folder and run the `Wolf-Sheep-Predation` model.

```bash
sh run.sh
```

Make sure that the default scenario described in the `WolfSheepPredation/config.json`, is available.

## Configuration

The scenario provides various attributes that can be modified to influence the simulation results. See as an example the
paramtization of the ```Sheep```.

- `count` defines the amount of sheep at the start of the simulation
- `SheepGainFromFood` defines the amount of energy a `Sheep` gains from eating grass.
- `SheepReproduce` defines the probability percentage of a reproduction for every tick and every agent

```json
{
  "agents": [
    {
      "name": "Sheep",
      "count": 30,
      "mapping": [
        {
          "parameter": "SheepGainFromFood",
          "value": 4
        },
        {
          "parameter": "SheepReproduce",
          "value": 5
        }
      ]
    }
  ]
}
```

You can find more information about scenario
configuration [here](https://mars.haw-hamburg.de/articles/core/model-configuration/sim_config_options.html).

## Visualization

The scenario is configured to run with a simulation visualization (see config.json).

```json
{
  "globals": {
    ...
    "console": true,
    "pythonVisualization": true
  }
  ...
}
```

To start the python visualization, check
the [Visualization](https://git.haw-hamburg.de/mars/model-deployments/-/tree/master/Visualization) repository.

The visualization displays the current world state of the simulation. So start the simulation script first before
running the simulation.

The simulation can be started directly with the following script. Remember to only open a single instance of a visualization at a time. It will always display the current simulation. So if a new simulation is started, then the visualization tool will connect to the new simulation run.

```bash
sh run_vis_only.sh
```

## Analyze

The result of the simulation is written in a CSV file. To change the output format modify the scenario configuration (
config.json). The results of the CSV can be directly analyzed and visualized with a preferred GIS tool. A script in the
language R is provided that displays a graph, which shows the correlation of population count to time.

To run the analysis use the following script.

```bash
sh run_analysis.sh
```