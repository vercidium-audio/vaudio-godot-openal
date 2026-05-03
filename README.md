# Vercidium Audio

Raytraced audio system with realistic occlusion and reverb for Godot 4.

13/04/2026 - this repository is updated to use the latest v1.1.0 C# SDK:
- [Announcement post](https://www.patreon.com/posts/vercidium-audio-153487929)
- [Breaking changes](https://docs.vercidium.com/raytraced-audio/Breaking+Changes+v1.1.0)
- [Full changelog](https://docs.vercidium.com/raytraced-audio/Changelog)

## Features

- Muffle sounds in real time
- Accurate reverb in any environment
- Innovative event-based raytracing system
- Realistic energy-based model using materials
- Dynamic scene updates - automatically handles moving objects

## Requirements

- **OpenAL Audio plugin** - This plugin depends on the [godot_openal](https://github.com/vercidium-audio/godot-openal) plugin and must be enabled first. If you wish to integrate with other audio middleware, use the raw [vaudio-godot](https://github.com/vercidium-audio/vaudio-godot) plugin
- Godot 4.x with C# support
- Vercidium Audio SDK

## Installation

Read in [project setup](./PROJECT_SETUP.md).

## Visual Studio

To run your Godot project from Visual Studio, click the small dropdown arrow next to `your_game` and click `your_game Debug Properties`.

Create a new launch profile by clicking the green icon in the top left, and rename it to `Godot`. Then set:
- the executable path
- command line parameters
- the working directory to `.`

![Godot debug properties in visual studio](docs/godot_visual_studio.png)

Then close the window, click the same small dropdown arrow, and select `Godot`. Use this launch profile from now on.

## Requirements

This plugin requires a license for the Vercidium Audio C# SDK. [Apply here](https://vercidium.com/audio) to get early access.
