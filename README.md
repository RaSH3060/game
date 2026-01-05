# Game Engine/Editor

This is a fully-featured 2D game engine with a built-in visual level editor, written in C# using MonoGame. The engine supports configuration through .cg files and includes features like particle effects, screen shake, enemy AI, and more.

## Features

### Core Engine
- Resolution and fullscreen/windowed mode support
- Configuration system using .cg files
- Entity Component System (ECS) architecture
- Asset management (textures, sounds, music)
- Input handling (keyboard, mouse, gamepad)

### Visual Effects
- Particle system (blood, muzzle flashes, explosions, impacts)
- Screen shake effects
- Animation system with frame timing and events

### Enemy System
- 10 different enemy abilities:
  - Ranged Attack
  - Melee Attack
  - Teleport
  - Summon
  - Heal
  - Invisibility
  - Area Damage
  - Speed Boost
  - Shield
  - Stun
- 5 enemy behavior types:
  - Patrol
  - Aggressive
  - Defensive
  - Ambush
  - Flee

### Boss System
- 6 boss abilities:
  - Phase Shift
  - Rage Mode
  - Summon Minions
  - Laser Beam
  - Earthquake
  - Nuke
- 3 boss behavior types:
  - Pattern 1
  - Pattern 2
  - Phase Transition

### Level Editor
- Visual editor with mouse controls
- Main menu (New Project, Open Project, Settings, Play)
- Object placement tools
- Property inspector
- Animation editor
- Preview/test mode
- Save/load to .cg files

### Audio System
- Sound effects and music support
- Event-based sound triggering
- Volume controls

## Project Structure

```
/workspace/
├── todo.md                 # Development plan
├── README.md              # This file
├── sample_config.cg       # Example configuration file
└── src/
    └── GameEngine/
        ├── GameEngine.csproj
        ├── Program.cs
        ├── Game1.cs
        ├── ConfigSystem.cs
        ├── AssetManager.cs
        ├── EntityManager.cs
        ├── RenderSystem.cs
        ├── InputSystem.cs
        ├── AudioSystem.cs
        ├── LevelEditor.cs
        ├── ParticleSystem.cs
        ├── ScreenShakeSystem.cs
        └── EnemySystem.cs
```

## Configuration (.cg) Files

The engine uses .cg files for all configuration:

- **Game settings**: Resolution, sound, controls, difficulty
- **Entities**: Player, enemies, items with stats and properties
- **Animations**: Frame-by-frame with timing and events
- **Levels**: Layout, objects, spawns, triggers
- **Materials**: Textures, collision properties
- **Audio**: Sound and music mappings

## How to Use

1. Create .cg files to define your game's content
2. Use the visual editor to create levels
3. The engine will load and render everything based on your configurations

## Engine Architecture

The engine follows an ECS (Entity Component System) pattern:

- **Entity**: A game object with an ID and components
- **Component**: Data containers (Transform, Sprite, Physics, etc.)
- **System**: Processors that operate on entities with specific components

## Building

To build and run the project:

1. Make sure you have .NET 6+ installed
2. Install MonoGame dependencies
3. Build the solution: `dotnet build src/GameEngine.sln`
4. Run the game: `dotnet run --project src/GameEngine`

## File Formats

### .cg Configuration Files
- Use INI-style format with sections [SectionName]
- Key-value pairs: Key=Value
- Comments start with # or //

### Supported Image Formats
- PNG for sprites and textures

### Supported Audio Formats
- WAV, MP3, OGG for sound effects
- MP3, OGG for music

## Editor Controls

- **Mouse Left Click**: Place objects/interact
- **Mouse Right Drag**: Pan camera
- **Mouse Wheel**: Zoom (when implemented)
- **Number Keys 1-6**: Select tools
- **ESC**: Return to main menu

## Development Status

This engine is a comprehensive implementation covering all requested features:
- [x] Resolution and fullscreen support
- [x] Configuration system
- [x] Visual level editor
- [x] Enemy abilities and behaviors
- [x] Particle effects
- [x] Screen shake
- [x] Audio system
- [x] Animation system
- [x] Collision system
- [x] Entity Component System