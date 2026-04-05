# Farm Game

A 3D farm game prototype built with Godot 4.6 and C# (.NET 8).

## Tech Stack

- **Engine**: Godot 4.6 (Forward Plus renderer)
- **Language**: C# (.NET 8) via `Godot.NET.Sdk/4.6.2`
- **Physics**: Jolt Physics 3D
- **Root Namespace**: `FarmGame`
- **Assembly Name**: `Farm Game`

## Running the Project

Open the project in the Godot 4.6 editor and press F5 (or the Play button). There is no CLI build step — the Godot editor compiles and launches the game.

## Project Structure

```
Scripts/
  Main.cs                        # Root Node3D — raycasts from camera to find hovered tile, toggles debug UI
  Controls/
    Player.cs                    # CharacterBody3D — WASD movement, mouse look, jump, dispatches tile interactions
    Interactions/
      IInteractable.cs           # Interface: PrimaryInteraction / SecondaryInteraction / TertiaryInteraction
      Interaction.cs             # Abstract base class for all interaction results
      InteractionType.cs         # Enum: Primary, Secondary, Tertiary
      NoInteraction.cs           # Null-object interaction (no-op)
      ReplaceTileInteraction.cs  # Interaction that swaps a tile for a different TileType
  Environment/
    Field.cs                     # Grid of Tile nodes; owns FieldRenderer; exposes WorldToGridPosition / GridToWorldPosition
    FieldRenderer.cs             # Batches tiles by TileType into MultiMeshInstance3D; owns TerrainCollision StaticBody3D
    FieldType.cs                 # Enum: Grass, Stone, Dirt, Random
  Tiles/
    Tile.cs                      # Abstract Node3D + IInteractable; CreateMesh() builds an ArrayMesh via SurfaceTool
    TileType.cs                  # Enum: Grass, Dirt, Stone, Edge
    TileFactory.cs               # Static factory: CreateTile(TileType) → Tile
    GrassTile.cs / DirtTile.cs / StoneTile.cs / EdgeTile.cs   # Concrete tiles (Height, Material, TileType)
  UI/
    Debug.cs                     # CanvasLayer showing FPS, player position, hovered tile info
    TileIndicator.cs             # Shader-based plane mesh that hovers over the looked-at tile
    HitIndicator.cs              # Small red box marking the exact raycast hit point (debug only)
Scenes/                          # Godot .tscn scene files
Shaders/                         # .gdshader files (tile_indicator, debug_shader)
```

## Input Map

| Action               | Binding          |
|----------------------|------------------|
| Move                 | WASD / Arrow keys |
| Sprint               | Shift            |
| Jump                 | Space            |
| Look                 | Mouse move       |
| Primary interaction  | Left click       |
| Secondary interaction| Right click      |
| Tertiary interaction | Middle click     |
| Toggle mouse capture | Escape           |
| Toggle debug panel   | P                |

## Key Conventions

- All scripts use the `FarmGame.Scripts.*` namespace hierarchy matching the folder structure.
- Tiles are rendered in batches using `MultiMeshInstance3D` (one per `TileType`) for performance. When tiles change, call `FieldRenderer.Clear()` then re-render.
- `Field` stores tiles in a `Dictionary<Vector2I, Tile>`. Grid origin is `(0,0)`, positive X is east, positive Y (grid) is south.
- Edge tiles (at `x = -1`, `x = Width`, `z = -1`, `z = Height`) border the playable area but are not part of the main grid.
- New tile types require: a new `TileType` enum value, a concrete `Tile` subclass, and a `TileFactory` case.
- New interactions require: a subclass of `Interaction`, handling in `Player.HandleInteraction()`, and the relevant `IInteractable` override on the target tile.
- `Tile.MaterialOverride` (set at runtime) takes precedence over `Tile.Material` (defined per subclass) when building the mesh.
