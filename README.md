# Swarm Survival
A proof of concept Unity mobile project demonstrating my skills in completing the requirements for the Ares Spawner Assignment.
Instead of simply creating a hastily made prototype with stitched together features, I decided to take the time available to me
to make a small scale but more modular and extensable codebase which incorporates assets from well-made asset packs.

## Description
Swarm Survival is a game where many AI controlled creatures will spawn into a level and start randomly moving around.
Once 2 creatures collide with each other 1 of 2 things will happen:
- 2 of the same creature type colliding into one another triggers a 3rd creature to spawn around the location of the collision.
- 2 different creatures colliding into one another triggers them both to be destroyed.

Creatures active in the level are constantly vying for domination, and it is the player's goal to ensure none of the 4 different species
go extinct for as long as possible. The player can interact with the world by tapping on any of the 4 spawners at the corners of the level
to hasten the spawn rates and attempt to balance out creature numbers.

## Game Systems
### Agents
Each creature to be spawned is an AI agent. Agents use NavMeshAgent components with a baked NavMesh for movement. Agent colliders are all
trigger colliders and each have a kinematic rigidbody component to facilitate collisions. Agents can also be given additional functionality
from other components or scripts. Optional functionality includes: object pooling, animated movements, and a DoTween animation to ease agents
in and out of the scene when instantiated or destroyed.

### Spawners
Each of the differently colored caves in the corners are spawners. Spawners create an object pool for 1 specific type of agent and will
begin spawning them during runtime according to the timings set up in their data asset (see below). Spawners have a state machine controlling
their spawn timings and possible interactions. Their 3 states are: Default Spawning, Accelerated, and Cooldown. Spawners can be tapped on
during runtime which will trigger them to go into accelerated mode, unless they are currently in cooldown. Spawners can also be extended
to have particle systems on top of them to denote what state they are in.

### ScriptableObject Data
For both agents and spawners, each different type needs different versions of the same parameters that can be easily tweaked. I decided
to accomplish this by using scriptable objects.
- For agents, their data defines what type of creature they are, options for movement and
obstacle avoidance (directly overriding their NavMeshAgents) and time delays for when they can collide again after initialization or a previous
collision.
- For spawners, their data defines what prefab to spawn, settings for their internal object pools, timings to define spawn delays
for both default and accelerated states, and additional delays which define how long each state lasts. Spawn timings are defined as
a list rather than a singular float so more complex sequences of spawning are possible.

## Code Architecture
My thought process when writing code for this project was to go above traditional prototype hardwired code expectations and
create more general, modular, and extensible code with minimal coupling. However, I also decided to use multiple pre-made
systems to reduce the amount of functionality I would need to implement.

### Code Principles
I focused on using the composition, observer, and parts of the singleton design pattern throughout my scripts in this project.
The reasons behind these decisions were:
- I wanted individual scripts to each only manage 1 responsibility and if a single object needed more functionality it would
need to be composited together using multiple scripts instead of 1 big script.
- I wanted scripts to only update when they needed to and to have scripts of differing layers (i.e. Spawner and Creature UI) communicate
with each other without needing to couple them together. So I create an EventManager static class which facilitated project-wide handling
of UnityActions in code.

### Prebuilt Systems
I decided to use NavMeshAgents for movement, Unity's own ObjectPool class for pooling, Rigidbody agents with colliders for collision
handling, the Terrain system for easy grass placement and heightmaps, DoTween for simple animations, and a UI pack for standard UI
layouts and interactions.

A lot of these systems are for quick and easy solutions and can cause performance issues compared to a custom or optimized solution.
But for the purpose of scope, I opted to keep these features instead of creating my own solutions from scratch.

## Performance
### Known Slowdowns
- The 3D scene itself with many meshes and active assets is tough to render on mobile devices, batching or baking all the static
meshes together would help reduce render times.
- Large amounts of skinned mesh renderers, rigidbodies, and shadow casters in the scene cause heavy slowdowns and could use optimization.
- ObjectPool class by default doesn't hard cap max size and doesn't initialize to default size at start. A custom implementation would
likely be more efficient than tacking these additional features onto the class.
- With all UI objects placed within one canvas and with making use of a non-optimized UI asset pack for windows and menus, updates to the
canvas will update more UI elements than what is actually changing and cause slowdowns.
- Semi-large terrain used as the ground and NavMeshSurface is unnecessary for such a small scene, and could be converted into a large mesh.

## Future Possible Features
This project is meant to only be a proof of concept and therefore is a simplistic version of a game concept that should hopefully
drive interest and imagination for a complete version of the game. Some possible additional features I've thought of are:
- Creature relationships between types could define complex behaviors on collision or actively affect spawn timings.
- Players could have the ability to select multiple agents and direct their movement to a certain area.
- Players could have more manual control of agent spawning, spawn timings, or even the ability to decrease a spawner's spawn rate.
- More complex levels could be created with required movement pathways, instead of an open field, or varied locations of the spawners.

## Assets Used
### Art
- RPG Monster Bundle - Used for the meshes and animations of the creatures.
https://assetstore.unity.com/packages/3d/characters/creatures/rpg-monster-bundle-pbr-260493
- Ultimate Low Poly Mining Pack - Used for the environment and spawner meshes.
https://assetstore.unity.com/packages/3d/props/ultimate-low-poly-mining-cave-blacksmith-pack-ores-gems-props-to-189279
- Polygon Nature - Used for the foliage and terrain texures of the environment.
https://assetstore.unity.com/packages/3d/vegetation/trees/polygon-nature-low-poly-3d-art-by-synty-120152
- Modern UI Pack - Used for UI sprites, component, and drag-and-drop UI windows.
https://assetstore.unity.com/packages/tools/gui/modern-ui-pack-201717

### Code
- Modern UI Pack - Prefab UI menus and windows had animations, interactions, and event hookups already implemented.
https://assetstore.unity.com/packages/tools/gui/modern-ui-pack-201717
- DoTween Pro - Used to generate a quick scale up and down animation for agents that is callable from code.
https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416
