# Bagel Quest, rebuilt for the browser

(Draft for the itch.io devlog, written on October 17, 2026. Written in the repo; the user posts it. Cut freely.)

Bagel Quest was my entry for A Game By Its Cover 2023, a small forage-and-craft quest in VB.NET and MonoGame: get a bagel. It lived as downloads for Windows, Linux and Mac. Now there is a version you can play in a tab.

## What it is

You are Loxy: two hit points, ten energy, twenty jools. The wilderness is a map of grass, trees, farms, clay pits and quarries cut by rivers, with one town in it. To win you must hold a bagel, and a bagel needs dough, a torch and sticks, cooked at a furnace you build yourself. Dough needs flour from wheat and a pot of water; the pot comes from the potter. A torch needs charcoal, which you get by building a campfire and putting it out. Along the way there are rats in the cellar to hunt for the innkeeper and things that will fight you in the trees.

## How it was rebuilt

I rebuilt it in Odin over a few sessions with Claude Code, an AI model. I decided what to keep, what to fix and what to leave alone, and I played it. Claude wrote the Odin. We worked in phases, each ending with me approving it: read the old game and write down what it does; the drawing; the data; the world and rules; the scripted bits (the old game ran Lua for every dialogue and action); the screens; saves; sound and controls; and this release.

The part I care about most: **the new game is checked against the old one.** A small throwaway program ran the real 2023 screens and recorded 127 of them, and the new game has to draw each one pixel for pixel. Where it does not, the difference is listed with the exact number of pixels and the reason. It plays the same scripted game in the browser as in a native build and the two must end in exactly the same state.

## What I decided

- **Faithful, with the clear bugs fixed.** Combat, crafting, prices and the map are the original's. The game does not get easier or harder.
- **The signs have real text now.** Four town signs said "This is sign #2", "#5", "#6" and "#8". They have real text now (jokes written with Claude, easy to change).
- **The silent sound effects are used.** The 2023 game shipped six sounds nothing ever played. They are on crafting, eating, picking things up, levelling up, shopping, learning and the wins.
- **Music.** I decided against the original music in the browser build. For now the page makes its own simple tunes with Web Audio, and they are placeholders.
- **Controls:** keyboard (including the original's AZERTY keys), gamepad, and on-screen buttons on a phone.

## The honest part

Reading the old game turned up real bugs, and the comparison found more:

- Dropping your last stack of items crashed the old game.
- The Equipment screen showed `System.Object[]` instead of the name of what you were holding.
- Eating a seasoned rat always said "That was a spicy one!", sometimes twice.
- The healer's "you lose jools" line printed the placeholder `{jools}` instead of a number.

All fixed. Some oddities I kept on purpose and wrote down rather than change: building a fire spends your energy even when you lack the wood, and the healer only ever heals you up to five health.

If something looks wrong, tell me what you did and what you saw.
