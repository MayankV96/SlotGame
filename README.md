# Slot Game (Unity)

## Game Overview
A 3-reel slot machine built in Unity. Press **SPIN** to spin the reels. You win when the **middle row** shows the same symbol on all three reels. Wins pay `bet x symbol multiplier`.

- Starting balance: 1000, bet: 10 per spin
- 3 reels x 3 visible rows
- Weighted random symbols (rare symbols pay more)

## Play Online
[Add your GitHub Pages link here]

## Run the WebGL Build Locally
Browsers block opening `index.html` directly. Use a local server:

```bash
cd Build/WebGL
python3 -m http.server 8000
```

Then open http://localhost:8000

## Bonus Features
- **Wild symbol:** substitutes for any symbol to complete a win
- **Win text pulse animation** on a win
- [Sound effects, if you added them]

## My Approach
- **Result first, animation second:** the random result is decided before the reels move. The animation only plays out a decision that was already made, which keeps the game fair and the code simple.
- **Weighted RNG:** each symbol has a weight. A symbol's chance of appearing is its weight divided by the total weight.
- **Data-driven symbols:** each symbol is a `ScriptableObject` (`SymbolData`), so adding or balancing symbols needs no code changes.
- **Smooth reels:** each reel is a long strip of images inside a masked window. The strip slides down with an ease-out curve, and the new result is placed at its top so it lands exactly in view.
- **Clear responsibilities:** `SlotMachine` controls the game flow, `Reel` controls one reel, and `SymbolData` stores symbol data.

## Project Structure
```
Assets/
  Scripts/            SlotMachine, Reel, SymbolData
  Prefabs/            SymbolImage
  ScriptableObjects/  Symbol assets
  Sprites/            Symbol and UI art
  Sounds/             Audio (if any)
  Scenes/             Main
Build/WebGL/          WebGL build
```
## GAME LINK
https://mayankv96.github.io/SlotGame/Build/WebGL/
## Credits
[Asset pack and any sounds used]


