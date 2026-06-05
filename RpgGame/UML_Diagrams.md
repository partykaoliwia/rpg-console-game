# UML diagrams - RpgGame MVC, stage 6

Ten dokument pokazuje najwazniejsze zaleznosci w projekcie po refaktoryzacji do MVC oraz po dodaniu multiplayera TCP/JSON.
Diagramy sa zapisane w formacie Mermaid, wiec mozna je wyrenderowac np. w VS Code, GitHub Markdown albo mermaid.live.

## 1. Diagram komponentow MVC i multiplayer

Ten diagram pokazuje glowny podzial odpowiedzialnosci:

- `GameModel` jest source of truth dla swiata gry.
- `LocalModel` trzyma tylko lokalny stan UI/inputu konkretnego gracza.
- `View` renderuje stan, ale go nie zmienia.
- Kontrolery inicjuja akcje.
- W multiplayerze klient wysyla DTO, a serwer wykonuje komendy na autorytatywnym `GameModel`.

```mermaid
flowchart LR
    Program["Program.cs<br/>composition root"]

    subgraph Model["Model"]
        GameModel["GameModel<br/>world state + game rules"]
        LocalModel["LocalModel<br/>local input/UI state"]
        GameEventLog["GameEventLog<br/>game events"]
    end

    subgraph View["View"]
        ConsoleGameView["ConsoleGameView"]
        ConsoleServerView["ConsoleServerView"]
        Visitors["View Visitors<br/>TileVisitor, ItemVisitor, CharacterVisitor"]
    end

    subgraph Controllers["Controller"]
        SinglePlayerController["SinglePlayerController"]
        ClientInputController["ClientInputController"]
        ClientUpdateController["ClientUpdateController"]
        ServerController["ServerController"]
        CommandRegistry["CommandRegistry"]
    end

    subgraph Network["Network TCP/JSON"]
        GameClient["GameClient"]
        GameServer["GameServer"]
        JsonReader["JsonMessageReader"]
        JsonWriter["JsonMessageWriter"]
    end

    subgraph DTO["Dto protocol"]
        ClientCommandDto["ClientCommandDto"]
        ServerStateDto["ServerStateDto"]
        WelcomeDto["WelcomeDto"]
        GameStateDto["GameStateDto"]
    end

    Program --> SinglePlayerController
    Program --> ClientInputController
    Program --> ClientUpdateController
    Program --> ServerController
    Program --> ConsoleGameView
    Program --> ConsoleServerView

    SinglePlayerController --> GameModel
    SinglePlayerController --> LocalModel
    ClientInputController --> LocalModel
    ClientInputController --> GameClient
    ClientInputController --> GameModel
    ClientUpdateController --> GameModel
    ClientUpdateController --> LocalModel
    ServerController --> GameModel
    ServerController --> CommandRegistry

    ConsoleGameView --> GameModel
    ConsoleGameView --> LocalModel
    ConsoleGameView --> Visitors
    ConsoleServerView --> GameModel

    GameModel --> GameEventLog

    GameClient <--> JsonReader
    GameClient <--> JsonWriter
    GameServer <--> JsonReader
    GameServer <--> JsonWriter
    GameClient <--> GameServer

    ClientInputController --> ClientCommandDto
    GameClient --> ClientCommandDto
    GameServer --> ClientCommandDto
    ServerController --> ServerStateDto
    ServerController --> WelcomeDto
    ServerStateDto --> GameStateDto
    WelcomeDto --> GameStateDto
```

## 2. Diagram klas - glowny Model gry

Ten diagram pokazuje rdzen domeny. Najwazniejsze jest to, ze reguly gry sa w `GameModel`: ruch, kolizja ze sciana, rozpoczecie i zakonczenie walki, smierc przeciwnika, loot, ekwipunek i reakcje przeciwnikow.

```mermaid
classDiagram
    class GameModel {
        +Board Board
        +string DungeonThemeName
        +GameEventLog EventLog
        +IReadOnlyList~Player~ Players
        +IReadOnlyList~Enemy~ Enemies
        +IReadOnlyList~CombatEncounter~ Encounters
        +AddPlayer(name) Player
        +RemovePlayer(playerId) bool
        +MovePlayer(playerId, dx, dy) void
        +BeginEncounterWithAdjacentEnemy(playerId) void
        +EndEncounterForPlayer(playerId) void
        +AttackEnemyWithLeftHand(playerId, enemyId, attack) void
        +AttackEnemyWithRightHand(playerId, enemyId, attack) void
        +CollectItem(playerId, itemIndex) void
        +EquipLeftHand(playerId, inventoryIndex) void
        +DropInventoryItem(playerId, inventoryIndex) void
        +UpdateEnemies() bool
        +ReplaceState(...) void
    }

    class Board {
        +int Width
        +int Height
        +GetTile(x, y) ITile
        +SetTile(x, y, tile) void
    }

    class ITile {
        <<interface>>
        +bool IsWalkable
        +IReadOnlyList~IItem~ Items
        +AddItem(item) void
        +TryRemoveItem(item) bool
        +GetTopItem() IItem
        +Accept(visitor) void
    }

    class FloorTile
    class WallTile

    class Player {
        +int Id
        +string Name
        +int X
        +int Y
        +int Health
        +IReadOnlyList~IItem~ Inventory
        +IItem LeftHand
        +IItem RightHand
        +TryAddToInventory(item) bool
        +EquipToLeftHand(item) bool
        +EquipToRightHand(item) bool
        +EquipToBothHands(item) bool
        +TakeDamage(damage) int
    }

    class Enemy {
        <<abstract>>
        +int Id
        +string Name
        +int X
        +int Y
        +int Health
        +int Strength
        +int Armor
        +int MoveEveryMs
        +TakeDamage(damage) int
        +Die(eventLog) void
        +UpdateOnSound(...) void
        +UpdateOnDeath(eventLog) void
        +Accept(visitor) void
    }

    class Harpy
    class Giant

    class CombatEncounter {
        +int Sequence
        +int PlayerId
        +int EnemyId
    }

    class GameEventLog {
        +Add(message) void
        +GetAll() IReadOnlyList~string~
        +GetRecent(count) IReadOnlyList~string~
        +ReplaceWith(events) void
    }

    class SoundPublisher
    class SpeciesPublisher
    class ISoundSubscriber {
        <<interface>>
    }
    class ISpeciesSubscriber {
        <<interface>>
    }

    GameModel *-- Board
    GameModel *-- GameEventLog
    GameModel o-- Player
    GameModel o-- Enemy
    GameModel o-- CombatEncounter
    GameModel *-- SoundPublisher
    GameModel *-- SpeciesPublisher

    Board *-- ITile
    ITile <|.. FloorTile
    ITile <|.. WallTile

    Enemy <|-- Harpy
    Enemy <|-- Giant
    ISoundSubscriber <|.. Enemy
    ISpeciesSubscriber <|.. Enemy

    Player o-- IItem
    FloorTile o-- IItem
```

## 3. Diagram klas - komendy, kontrolery i protokol DTO

Ten diagram pokazuje, jak wejscie gracza jest zamieniane na komendy. W singleplayerze komenda wykonuje sie na lokalnym `GameModel`. W multiplayerze klient wysyla `ClientCommandDto`, a serwer odtwarza komendy przez `CommandRegistry`.

```mermaid
classDiagram
    class IPlayerCommand {
        <<interface>>
        +Execute(model, playerId) void
        +ToDto(playerId) ClientCommandDto
    }

    class MoveCommand
    class BeginCombatCommand
    class EndCombatCommand
    class AttackCommand
    class CollectCommand
    class EquipCommand
    class UnequipCommand
    class DropItemCommand

    IPlayerCommand <|.. MoveCommand
    IPlayerCommand <|.. BeginCombatCommand
    IPlayerCommand <|.. EndCombatCommand
    IPlayerCommand <|.. AttackCommand
    IPlayerCommand <|.. CollectCommand
    IPlayerCommand <|.. EquipCommand
    IPlayerCommand <|.. UnequipCommand
    IPlayerCommand <|.. DropItemCommand

    class CommandRegistry {
        -Dictionary~ClientCommandKind, Func~ClientCommandDto,IPlayerCommand~~ fromDto
        +FromDto(dto) IPlayerCommand
    }

    class ClientCommandDto {
        +int PlayerId
        +ClientCommandKind Kind
        +AttackKind AttackKind
        +HandSlotKind HandSlot
        +int DeltaX
        +int DeltaY
        +int TargetId
        +int ItemIndex
    }

    class ServerStateDto {
        +int StateSequence
        +GameStateDto State
    }

    class WelcomeDto {
        +int PlayerId
        +int StateSequence
        +GameStateDto State
    }

    class GameStateMapper {
        +ToDto(model) GameStateDto
        +ApplyTo(model, dto) void
    }

    class SinglePlayerController {
        +HandleKey(key) void
        +Tick() void
    }

    class ClientInputController {
        +HandleKeyAsync(key, token) Task
    }

    class ClientUpdateController {
        +Apply(serverState) void
    }

    class ServerController {
        +ConnectPlayer(name) WelcomeDto
        +Handle(playerId, commandDto) ServerStateDto
        +DisconnectPlayer(playerId) ServerStateDto
        +Tick() ServerStateDto
        +CreateState() ServerStateDto
    }

    SinglePlayerController --> IPlayerCommand : executes
    ClientInputController --> IPlayerCommand : creates
    ClientInputController --> ClientCommandDto : sends
    ServerController --> CommandRegistry
    CommandRegistry --> IPlayerCommand : recreates
    ServerController --> GameStateMapper
    ServerController --> ServerStateDto
    ServerController --> WelcomeDto
    ClientUpdateController --> GameStateMapper
```

## 4. Diagram sekwencji - akcja gracza w multiplayer

Przyklad: gracz naciska klawisz ruchu, zbierania itemu albo ataku. Klient nie zmienia swiata bezposrednio. Wysyla DTO do serwera, a serwer waliduje i modyfikuje autorytatywny model.

```mermaid
sequenceDiagram
    actor Player
    participant View as ConsoleGameView
    participant Input as ClientInputController
    participant Client as GameClient
    participant Server as GameServer
    participant ServerController
    participant Registry as CommandRegistry
    participant Model as GameModel
    participant Mapper as GameStateMapper
    participant Update as ClientUpdateController

    Player->>Input: naciska klawisz
    Input->>Input: InputMode.Handle(key)
    Input->>Client: SendCommandAsync(ClientCommandDto)
    Client->>Server: TCP JSON line
    Server->>ServerController: Handle(connectedPlayerId, dto)
    ServerController->>ServerController: sprawdza dto.PlayerId
    ServerController->>Registry: FromDto(dto)
    Registry-->>ServerController: IPlayerCommand
    ServerController->>Model: command.Execute(model, playerId)
    Model->>Model: walidacja + zmiana swiata
    Model->>Model: zapis do GameEventLog
    ServerController->>Mapper: ToDto(authoritativeModel)
    Mapper-->>ServerController: GameStateDto
    ServerController-->>Server: ServerStateDto + StateSequence
    Server-->>Client: broadcast ServerStateDto
    Client-->>Update: ReadStateAsync()
    Update->>Update: ignoruje starsze StateSequence
    Update->>Mapper: ApplyTo(localGameModel, dto)
    Update->>Update: sync encounter/menu/game over
    View->>View: Render(localGameModel, localModel)
```

## 5. Diagram stanow lokalnego inputu

`LocalModel` posiada stos `IInputMode`. To jest lokalny stan klienta/gracza, a nie stan swiata. Dzieki temu singleplayer i multiplayer moga miec ten sam `GameModel` i ten sam `ConsoleGameView`.

```mermaid
stateDiagram-v2
    [*] --> Instruction
    Instruction --> Playing: ContinueInstructions

    Playing --> Inventory: OpenInventory
    Inventory --> Playing: CloseMenu / inventory empty

    Playing --> Loot: Collect on many items
    Loot --> Playing: CloseMenu / no items left

    Playing --> Combat: GameModel has CombatEncounter
    Combat --> WeaponChoice: attack selected and two weapons held
    WeaponChoice --> Combat: CancelAttack
    WeaponChoice --> Playing: attack resolved and encounter ended
    Combat --> Playing: EndCombat / encounter ended / enemy dead

    Playing --> LogHistory: OpenLogHistory
    Inventory --> LogHistory: OpenLogHistory
    Loot --> LogHistory: OpenLogHistory
    Combat --> LogHistory: OpenLogHistory
    LogHistory --> Playing: CloseLogHistory or CloseMenu

    Playing --> GameOver: local player dead
    Combat --> GameOver: local player dead
    GameOver --> [*]: any key
```

## 6. Diagram klas - generowanie mapy: Builder + Strategy + Factory

Ten diagram pokazuje wzorce z etapu 2/4 przeniesione do Modelu. `DungeonDirector` steruje budowaniem, `IGenerationStrategy` okresla kolejnosc krokow, a `IDungeonThemeFactory` dostarcza konkretne itemy, przeciwnikow, tytul i intro dla motywu.

```mermaid
classDiagram
    class DungeonDirector {
        +SetBuilder(builder) void
        +SetStrategy(strategy) void
        +ConstructDungeon() void
    }

    class IDungeonBuilder {
        <<interface>>
        +BuildEmptyDungeon() void
        +BuildFilledDungeon() void
        +AddCorridors() void
        +AddRooms() void
        +AddCentralHall(width, height) void
        +AddItems(count) void
        +AddWeapons(count) void
        +AddMoney(count) void
        +AddEnemies(count) void
    }

    class MapBuilder {
        +BuildResult() GeneratedDungeon
    }

    class InstructionBuilder {
        +BuildResult() InstructionGuide
    }

    class IGenerationStrategy {
        <<interface>>
        +Generate(builder) void
    }

    class LabyrinthGenStrategy
    class ArenaGenStrategy
    class LibraryGenStrategy

    class IDungeonThemeFactory {
        <<interface>>
        +ThemeName string
        +IntroMessage string
        +Title IReadOnlyList~string~
        +GenerationStrategy IGenerationStrategy
        +CreateRandomJunk() IItem
        +CreateRandomCurrency() Currency
        +CreateRandomWeapon(index) IItem
        +CreateEnemy(id, x, y, distance) Enemy
        +CreateBoss(id, x, y) Enemy
    }

    class LabyrinthFactory
    class ArenaFactory
    class LibraryFactory

    DungeonDirector --> IDungeonBuilder
    DungeonDirector --> IGenerationStrategy

    IDungeonBuilder <|.. MapBuilder
    IDungeonBuilder <|.. InstructionBuilder

    IGenerationStrategy <|.. LabyrinthGenStrategy
    IGenerationStrategy <|.. ArenaGenStrategy
    IGenerationStrategy <|.. LibraryGenStrategy

    IDungeonThemeFactory <|.. LabyrinthFactory
    IDungeonThemeFactory <|.. ArenaFactory
    IDungeonThemeFactory <|.. LibraryFactory

    MapBuilder --> IDungeonThemeFactory
    InstructionBuilder --> IDungeonThemeFactory
    LabyrinthFactory --> LabyrinthGenStrategy
    ArenaFactory --> ArenaGenStrategy
    LibraryFactory --> LibraryGenStrategy
```

## 7. Diagram klas - itemy: Decorator + Visitor + walka

Ten diagram pokazuje itemy i walke. Modyfikatory itemow sa dekoratorami, a ataki sa Visitorami. Dzieki temu mapowanie DTO i walka nie wymagaja RTTI.

```mermaid
classDiagram
    class IItem {
        <<interface>>
        +Name string
        +Bonuses PlayerStats
        +SoundRange int
        +PickUp(player) bool
        +EquipToLeftHand(player, instance) bool
        +EquipToRightHand(player, instance) bool
        +Unequip(player) void
        +AcceptAttack(visitor, player) CombatStats
        +Accept(visitor) void
    }

    class Weapon {
        <<abstract>>
        +Damage int
        +GetFullDamage(player) int
    }

    class HeavyWeapon
    class LightWeapon
    class MagicWeapon
    class Kopis
    class Labrys
    class Xiphos
    class StymphalianQuill
    class Dory

    class Currency
    class Gold
    class Coin
    class JunkItem
    class OldBook
    class AriadnesThread
    class CyclopsEye
    class IcarusFeather
    class ShatteredShield

    class ItemDecorator {
        <<abstract>>
        -IItem decorated
        +InnerItem IItem
    }

    class SpartanModifier
    class AthenianModifier
    class HephaesteanModifier
    class MidasTouchModifier
    class PrometheanModifier
    class AchillesBaneModifier
    class AphroditesKissModifier
    class DionysianModifier
    class HydrasVenomModifier
    class IcarianModifier

    class IAttackVisitor {
        <<interface>>
        +Name string
        +Visit(HeavyWeapon, player) CombatStats
        +Visit(LightWeapon, player) CombatStats
        +Visit(MagicWeapon, player) CombatStats
        +Visit(IItem, player) CombatStats
    }

    class NormalAttack
    class StealthAttack
    class MagicAttack

    IItem <|.. Weapon
    Weapon <|-- HeavyWeapon
    Weapon <|-- LightWeapon
    Weapon <|-- MagicWeapon
    HeavyWeapon <|-- Kopis
    HeavyWeapon <|-- Labrys
    LightWeapon <|-- Xiphos
    LightWeapon <|-- StymphalianQuill
    MagicWeapon <|-- Dory

    IItem <|.. Currency
    Currency <|-- Gold
    Currency <|-- Coin
    IItem <|.. JunkItem
    JunkItem <|-- OldBook
    JunkItem <|-- AriadnesThread
    JunkItem <|-- CyclopsEye
    JunkItem <|-- IcarusFeather
    JunkItem <|-- ShatteredShield

    IItem <|.. ItemDecorator
    ItemDecorator o-- IItem : wraps
    ItemDecorator <|-- SpartanModifier
    ItemDecorator <|-- AthenianModifier
    ItemDecorator <|-- HephaesteanModifier
    ItemDecorator <|-- MidasTouchModifier
    ItemDecorator <|-- PrometheanModifier
    ItemDecorator <|-- AchillesBaneModifier
    ItemDecorator <|-- AphroditesKissModifier
    ItemDecorator <|-- DionysianModifier
    ItemDecorator <|-- HydrasVenomModifier
    ItemDecorator <|-- IcarianModifier

    IAttackVisitor <|.. NormalAttack
    IAttackVisitor <|.. StealthAttack
    IAttackVisitor <|.. MagicAttack
    IItem --> IAttackVisitor : AcceptAttack
```

## 8. Diagram klas - Observer przeciwnikow bez C# event

W projekcie nie uzywa sie C# `event`. Przeciwnicy subskrybuja publisherow jawnie. Reakcje sa zapisywane do `GameEventLog`, a nie bezposrednio do konsoli.

```mermaid
classDiagram
    class ISoundSubscriber {
        <<interface>>
        +X int
        +Y int
        +UpdateOnSound(sourceX, sourceY, distance, eventLog) void
    }

    class ISpeciesSubscriber {
        <<interface>>
        +UpdateOnDeath(eventLog) void
    }

    class SoundPublisher {
        -HashSet~ISoundSubscriber~ subscribers
        +Subscribe(subscriber) void
        +Unsubscribe(subscriber) void
        +NotifySubscribers(x, y, range, board, eventLog) void
    }

    class SpeciesPublisher {
        -HashSet~ISpeciesSubscriber~ subscribers
        +Subscribe(subscriber) void
        +Unsubscribe(subscriber) void
        +NotifySubscribers(eventLog) void
    }

    class Enemy {
        <<abstract>>
        +JoinObservers(species, sound) void
        +LeaveObservers() void
        +Die(eventLog) void
        +UpdateOnSound(...) void
        +UpdateOnDeath(eventLog) void
    }

    class Harpy {
        +UpdateOnSound(...) void
        +UpdateOnDeath(eventLog) void
    }

    class Giant {
        +UpdateOnSound(...) void
        +UpdateOnDeath(eventLog) void
    }

    class GameModel {
        -SoundPublisher soundPublisher
        -SpeciesPublisher harpySpecies
        -SpeciesPublisher giantSpecies
        -RegisterEnemy(enemy) void
        -NotifySound(x, y, range) void
    }

    ISoundSubscriber <|.. Enemy
    ISpeciesSubscriber <|.. Enemy
    Enemy <|-- Harpy
    Enemy <|-- Giant

    SoundPublisher o-- ISoundSubscriber
    SpeciesPublisher o-- ISpeciesSubscriber
    Enemy --> SoundPublisher : subscribes
    Enemy --> SpeciesPublisher : subscribes
    GameModel *-- SoundPublisher
    GameModel *-- SpeciesPublisher
    GameModel --> Enemy : registers by Visitor
```

## 9. Diagram klas - Chain of Responsibility dla inputu

Ten diagram pokazuje wzorzec z etapu 2. Kontrolery nie maja duzych switchy po klawiszach dla kazdej komendy ruchu/ekwipunku/walki, tylko buduja lancuch handlerow.

```mermaid
classDiagram
    class InputHandler {
        <<abstract>>
        -InputHandler next
        +SetNext(handler) InputHandler
        +Handle(key) IPlayerCommand
    }

    class KeyCommandHandler {
        -ConsoleKey key
        -Func~IPlayerCommand~ commandFactory
        +Handle(key) IPlayerCommand
    }

    class NullInputHandler {
        +Handle(key) IPlayerCommand
    }

    class InputChainBuilder {
        -KeyBindingProfile keyBindings
        +AddKey(selector, commandFactory) InputChainBuilder
        +Build() InputHandler
    }

    class KeyBindingProfile
    class IPlayerCommand
    class SinglePlayerController
    class ClientInputController

    InputHandler <|-- KeyCommandHandler
    InputHandler <|-- NullInputHandler
    InputHandler o-- InputHandler : next
    InputChainBuilder --> KeyBindingProfile
    InputChainBuilder --> KeyCommandHandler
    KeyCommandHandler --> IPlayerCommand
    SinglePlayerController --> InputChainBuilder
    ClientInputController --> InputChainBuilder
```

