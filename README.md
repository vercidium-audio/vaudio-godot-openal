# Vercidium Audio

Raytraced audio system with realistic occlusion and reverb for Godot 4.

## Features

- Muffle sounds in real time
- Accurate reverb in any environment
- Innovative event-based raytracing system
- Realistic energy-based model using materials
- Dynamic scene updates - automatically handles moving objects

## References
- [godot-openal repo](https://github.com/vercidium-audio/godot-openal)
- [Vercidum Audio documentation](https://docs.vercidium.com/raytraced-audio/v110/Getting+Started)

## Requirements

- This plugin depends on the [godot-openal](https://github.com/vercidium-audio/godot-openal) plugin. If you wish to use other audio middleware instead, use the raw [vaudio-godot](https://github.com/vercidium-audio/vaudio-godot) plugin
- Godot 4.x with C# support
- [Vercidium Audio SDK](https://vercidium.com/audio)

## Installation

Read in [project setup](./PROJECT_SETUP.md).

## Visual Studio

To run your Godot project from Visual Studio, click the small dropdown arrow next to `your_game` and click `your_game Debug Properties`.

Create a new launch profile by clicking the green icon in the top left, and rename it to `Godot`. Then set:
- the executable path
- command line parameters to `--path, --verbose`
- working directory to `.`

![Godot debug properties in visual studio](docs/godot_visual_studio.png)

Then close the window, click the same small dropdown arrow, and select `Godot`. Use this launch profile from now on.

## Licencing

The Vercidium Audio SDK is free for non-commercial products only. To purchase a licence for commercial use, head over to the [Vercidium Audio website](https://vercidium.com/audio).