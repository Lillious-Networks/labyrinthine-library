# Labyrinthine Library

A comprehensive C# mod library for **Labyrinthine** (by Valko Game Studios) that provides developers with a clean, unified API to interact with game systems through MelonLoader.

## Overview

Labyrinthine Library is a MelonLoader mod that acts as an abstraction layer for mod developers, exposing game functionality through easy-to-use modules. Whether you're creating a custom mod or extending game behavior, this library handles the complexity of accessing underlying game systems.

## Features

- **Player Management** - Access player references and character controllers
- **Currency System** - Manage tokens and tickets with full arithmetic operations
- **Progression System** - Control player levels and experience points
- **Achievement System** - Unlock and query achievements
- **Save Data Access** - Read/write save game data including unlocks
- **Monster Control** - Direct monsters toward targets or get monster lists
- **UI Notifications** - Display styled toast notifications with animations
- **Game State Detection** - Query game state (in-game, menu, loading, etc.)
- **Anti-Cheat Bypass** - Disable cheat detection for modding purposes

## Installation

1. **Install MelonLoader** - Download and install [MelonLoader](https://melonwiki.xyz/) for Labyrinthine
2. **Place DLL** - Copy the compiled `labyrinthine-library.dll` to the MelonLoader mods folder
3. **Done** - The library auto-initializes when the game starts

## Quick Start

### Basic Usage

```csharp
using labyrinthine_library;
using labyrinthine_library.Modules;
using MelonLoader;

public class Mod : MelonMod
{
    public override void OnUpdate()
    {
        // Always check if the library is initialized and player is in game
        if (!Library.HasInitialized() || !Library.IsInGame() || !Library.IsInitialized())
            return;

        // Access the player GameObject
        var player = Library.Player();
        if (player != null)
        {
            // Manipulate player position
            player.transform.position += player.transform.forward * 0.1f;
        }

        // Show a notification
        Library.ShowToast("Player moved forward!", ToastType.Info);
    }
}
```

### Example 1: Currency Management

```csharp
// Get current token balance
int tokens = Currency.Current(CurrencyType.HardcoreTokens);
MelonLogger.Log($"Current tokens: {tokens}");

// Add tokens
int newBalance = Currency.Increase(CurrencyType.HardcoreTokens, 50);
MelonLogger.Log($"New balance: {newBalance}");

// Decrease tokens
Currency.Decrease(CurrencyType.RareTokens, 25);

// Check if player can afford something
if (Currency.Afforable(250))
{
    MelonLogger.Log("Player can afford item!");
}
```

### Example 2: Player Access and Manipulation

```csharp
// Get the player GameObject
GameObject? player = Library.Player();
if (player != null)
{
    // Access player transform
    Vector3 playerPosition = player.transform.position;
    MelonLogger.Log($"Player at: {playerPosition}");

    // Move player
    player.transform.position += Vector3.forward * 0.5f;
}

// Get character controller
CharacterController? charController = Library.CharacterController();
if (charController != null)
{
    // Disable collider for noclip effect
    charController.GetComponent<Collider>().enabled = false;
}

// Get all players in scene
GameObject[]? allPlayers = Library.Players();
if (allPlayers != null)
{
    MelonLogger.Log($"Total players: {allPlayers.Length}");
}
```

### Example 3: Game State Detection

```csharp
// Check library initialization
if (!Library.HasInitialized())
{
    MelonLogger.Log("Library not initialized yet");
    return;
}

// Query game states
if (Library.IsInGame())
{
    MelonLogger.Log("Player is in an active game session");
}

if (Library.IsInMainMenu())
{
    MelonLogger.Log("Player is in the main menu");
}

if (Library.IsInLobby())
{
    MelonLogger.Log("Player is in the lobby");
}

if (Library.IsInLoadingScreen())
{
    MelonLogger.Log("Game is loading...");
}
```

### Example 4: Currency Advancement (Progression)

```csharp
// Increase player progression currency
int tokens = Currency.Increase(CurrencyType.HardcoreTokens, 100);

// Set exact currency amount
Currency.Set(CurrencyType.RareTokens, 500);

// Donate currency to host (multiplayer)
if (Library.IsInLobby())
{
    Currency.DonateCurrency(50);
    Library.ShowToast("Donated 50 tokens to host!", ToastType.Success);
}
```

### Example 5: Monster Interaction

```csharp
// Get all monsters in the current scene
List<GameObject>? monsters = Monsters.List();
if (monsters != null && monsters.Count > 0)
{
    MelonLogger.Log($"Found {monsters.Count} monsters");
}

// Direct all monsters toward the player
if (Library.IsInGame())
{
    GameObject? player = Library.Player();
    if (player != null)
    {
        Monsters.NavigateToTarget(player);
        Library.ShowToast("Monsters redirected!", ToastType.Warning);
    }
}
```

### Example 6: Save Data Manipulation

```csharp
// Check and modify tutorial progress
if (!Saves.CompletedTutorial())
{
    Saves.CompleteTutorial(true);
}

// Unlock monster types
if (!Saves.MonsterTypeUnlocked(EMonsterType.Wraith))
{
    Saves.UnlockMonsterType(EMonsterType.Wraith);
}

// Unlock maze types
Saves.UnlockMazeType(EMazeType.Mines);
```

### Example 7: Achievements

```csharp
// Unlock a specific achievement
Achievements.Add(EAchievement.FirstWin);

// Unlock all achievements at once
Achievements.UnlockAll();

// Get list of all achievements
List<EAchievement> allAchievements = Achievements.List();
```

### Example 8: UI Notifications

```csharp
// Show different types of notifications
Library.ShowToast("Welcome back!", ToastType.Info);
Library.ShowToast("Achievement unlocked!", ToastType.Success);
Library.ShowToast("Low health warning", ToastType.Warning);
Library.ShowToast("Critical error occurred", ToastType.Error);
```

### Example 9: Complete Noclip Mod Pattern

```csharp
public class NoclipMod : MelonMod
{
    private bool noclipEnabled = false;

    public override void OnUpdate()
    {
        // Safety checks
        if (!Library.HasInitialized() || !Library.IsInGame() || !Library.IsInitialized())
            return;

        // Toggle noclip with backtick key
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            noclipEnabled = !noclipEnabled;
            Library.ShowToast($"Noclip: {(noclipEnabled ? "Enabled" : "Disabled")}",
                noclipEnabled ? ToastType.Success : ToastType.Info);

            var charController = Library.CharacterController();
            if (charController != null)
            {
                charController.GetComponent<Collider>().enabled = !noclipEnabled;
            }
        }

        // Handle noclip movement
        if (noclipEnabled)
        {
            var player = Library.Player();
            if (player == null)
            {
                noclipEnabled = false;
                return;
            }

            float speed = Input.GetKey(KeyCode.LeftShift) ? 0.15f : 0.1f;

            // Vertical movement
            if (Input.GetKey(KeyCode.Space))
                player.transform.position += player.transform.up * speed;
            else if (Input.GetKey(KeyCode.LeftControl))
                player.transform.position -= player.transform.up * speed;

            // Directional movement
            if (Input.GetKey(KeyCode.W))
                player.transform.position += player.transform.forward * speed;
            else if (Input.GetKey(KeyCode.S))
                player.transform.position -= player.transform.forward * speed;

            if (Input.GetKey(KeyCode.D))
                player.transform.position += player.transform.right * speed;
            else if (Input.GetKey(KeyCode.A))
                player.transform.position -= player.transform.right * speed;
        }
    }
}
```

## API Reference

### Main Library Class (Static Methods)

All methods are static and accessed via the `Library` class directly.

```csharp
// Game State Queries
static bool IsInGame()              // Check if player is in active game
static bool IsInLoadingScreen()     // Check if loading screen is shown
static bool IsInMainMenu()          // Check if in main menu
static bool IsInLobby()             // Check if in lobby
static bool IsInitialized()         // Check if game scene initialized
static bool HasInitialized()        // Check if lib finished initializing

// Player Access
static GameObject? Player()                      // Get current player GameObject
static GameObject[]? Players()                   // Get all player GameObjects
static CharacterController? CharacterController()  // Get player character controller
static PlayerController? PlayerController()      // Get player controller component

// UI Notifications
static void ShowToast(string message, ToastType type = ToastType.Info)

// Module Access (Static Properties)
static Currency Currency { get; }              // Currency operations
static Levels Levels { get; }                  // Player level operations
static Experience Experience { get; }          // Player experience operations
static Achievements Achievements { get; }      // Achievement management
static Monsters Monsters { get; }              // Monster operations
static Saves Saves { get; }                    // Save data access
```

### Module APIs

#### Currency Module (Static)
Manage player currency (tokens and tickets).

```csharp
enum CurrencyType { HardcoreTokens, RareTokens, Tickets }

static int Increase(CurrencyType currency, int amount)  // Add currency, returns new balance
static int Decrease(CurrencyType currency, int amount)  // Remove currency, returns new balance
static int Current(CurrencyType currency)               // Get current amount
static int Set(CurrencyType currency, int amount)       // Set exact amount (not for Tickets)
static bool Afforable(int amount)                       // Check if can afford tickets
static void DonateCurrency(int amount)                  // Donate to host (lobby only)
```

#### Levels Module (Static)
Manage player level progression.

```csharp
static void Increase(int levels)     // Increase level
static void Decrease(int levels)     // Decrease level
static void Set(int level)           // Set exact level
static int Current()                 // Get current level
```

#### Experience Module (Static)
Manage player experience points.

```csharp
static void Increase(int xp)                  // Add experience
static void Decrease(int xp)                  // Remove experience
static void Set(int xp)                       // Set exact experience
static int Current()                          // Get current experience
static int RequiredForLevel(int level)        // Get XP needed for level
```

#### Achievements Module (Static)
Manage achievement unlocks.

```csharp
static void Add(EAchievement achievement)     // Unlock a specific achievement
static List<EAchievement> List()              // Get all achievements
static void UnlockAll()                       // Unlock all achievements
```

#### Monsters Module (Static)
Interact with monsters in the game.

```csharp
static List<GameObject>? List()               // Get all monsters in scene
static void NavigateToTarget(GameObject target)  // Direct all monsters to target
```

#### Saves Module (Static)
Access and modify save game data.

```csharp
static bool SkippedTutorial()                      // Check if tutorial was skipped
static void SkipTutorial(bool value)               // Set tutorial skip status
static bool CompletedTutorial()                    // Check if tutorial completed
static void CompleteTutorial(bool value)           // Set tutorial completion
static bool MonsterTypeUnlocked(EMonsterType type) // Check monster unlock
static void UnlockMonsterType(EMonsterType type)   // Unlock monster type
static bool MazeTypeUnlocked(EMazeType type)       // Check maze unlock
static void UnlockMazeType(EMazeType type)         // Unlock maze type
```

### Enums

```csharp
enum CurrencyType { HardcoreTokens, RareTokens, Tickets }
enum ToastType { Info, Success, Warning, Error }
enum EAchievement { /* game achievements */ }
enum EMonsterType { /* monster types */ }
enum EMazeType { /* maze types */ }
enum EMonsterModifier { /* monster modifiers */ }
```

## Building from Source

### Requirements
- Visual Studio 2022 or later
- .NET 6.0 SDK
- MelonLoader framework
- Labyrinthine game files

### Build Steps

1. Clone the repository
2. Install NuGet dependencies:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build -c Release
   ```
4. The compiled DLL will be in `bin/Release/`

### Build Configurations
- **Debug** - Development build with debug symbols
- **Release** - Optimized release build
- **Obfuscar** - Obfuscated release build for distribution

## Architecture

The library follows a **Modular Facade Pattern**:

- **Library.cs** - Main entry point providing a unified API
- **Modules** - Specialized modules for each game system
- **SharedData.cs** - Centralized game state container
- **Initializer.cs** - Scene detection and player discovery

This design allows developers to access game systems through a single `Library` class while maintaining clean separation of concerns.

## Dependencies

- **MelonLoader** v0.7.2 - Mod loader for Unity games
- **Lib.Harmony** v2.4.2 - Runtime method patching
- Unity/IL2CPP runtime libraries

## Version

**Current Version:** 1.0.0

See [Release Notes](releasenotes.txt) for detailed changelog.

## License

Licensed under the Apache License 2.0. See [LICENSE.txt](LICENSE.txt) for details.

## Author

Created by **Lillious**

## Support

For issues, questions, or contributions, please refer to the project repository or contact the maintainers.

---

**Note:** This library is designed for Labyrinthine by Valko Game Studios and requires MelonLoader to function. Use responsibly and in accordance with the game's terms of service.
