# Tomorrow Morning — Project Memory

Last compiled: 2026-09-26
Unity project: `D:\Unity_Git\Tomorrow Morning`

This note carries forward relevant design and production context from the user's ChatGPT conversations and the current Unity project. It is intended to prevent repeated rediscovery during Unity work.

## How to use this note

- Treat current project files and explicit, recent user decisions as the source of truth.
- Keep user-confirmed decisions separate from suggestions made by an assistant. Ideas in the “Unapproved design ideas” section are not requirements; ask before implementing them.
- Some older pitch and budget material was exploratory. Do not treat it as a current commitment without checking with the user.
- A separate ChatGPT history titled “GDD Summary and Analysis” concerns **Delivery Empire Simulator**. It is a different game; do not apply its Android/economy/warehouse architecture to Tomorrow Morning.
- Preserve existing project changes. At the time of this note, Git showed extensive local edits to the gameplay scene, terrain, vegetation, prefabs, and settings. Do not reset, clean, or overwrite those changes.

## User-confirmed direction

- **Title:** Tomorrow Morning.
- **Perspective:** Third person.
- **Genre / positioning:** Survival horror with action and exploration.
- **Premise:** The player survives a plane crash on an isolated island.
- **Pitch line used in prior deck work:** “24 Minutes. One Island. Make It To Morning.”
- The player should search for valuable items. In the pitch deck, the user asked to replace the phrase “jump and survival” with wording along the lines of “look for valuable items.” This is a presentation correction; it does not by itself remove jumping from the actual game.
- The user asked to remove “Resource-Management Fans” from the deck's audience section. Do not position the game primarily as a resource-management game. This does not mean survival supplies or needs are forbidden mechanics.
- The user said they are removing the climbing mechanic. The current code still contains a `PlayerClimbing` stub marked temporarily disabled to preserve scene references; the controller still has jump input. Confirm before removing or changing related scene references.
- The user has night gameplay ideas of their own and previously asked for ideas for the daytime. Do not replace the user's night plan with assistant-generated mechanics.
- The user intends to provide the island map and wanted the publisher deck's island layout to follow it. The map itself was not available in the retrieved ChatGPT history; do not invent island geography or claim a generated floorplan matches it.

## World and facility notes

The user requested a **wide rectangular experimental bureau**, not a square building. A previously requested concept floor plan included:

- Reception hall
- Medical room / clinic
- Weapons room
- Two large experimental rooms
- Two resource rooms
- Basement garage
- Restrooms
- Several offices
- Staff accommodation rooms
- Other plausible support rooms for an experimental bureau

The user was open to multiple floors and a basement. This was a blueprint request/concept, not proof that this exact building layout is implemented in the Unity scene.

## Gameplay summary and confidence

Earlier ChatGPT summaries describe the intended loop as: plane crash → explore the island → avoid or fight zombies → scavenge weapons and supplies → survive a 24-minute day/night cycle → reach a helipad for rescue. Those summaries also mention stamina and thirst. Treat this as a useful working description, but confirm detailed mechanics against the user's current design before making scope decisions.

The previous assistant suggested that day should be a preparation phase and night the test of those preparations. The user said they already had night plans and needed day ideas; they did not approve any specific suggestion in the chats retrieved here.

## Unapproved design ideas from earlier brainstorming

These are options to discuss, not accepted features:

- Require a subset of rescue objectives before extraction (examples suggested: radio tower, helicopter fuel, emergency flare, helipad power, flight recorder).
- Noise-based zombie response to gunfire, running, or light; different day/night behavior.
- Randomized loot, limited inventory, safe houses, and a small set of special zombie types.
- Weather, lightweight crafting, wildlife, survivor encounters, environmental puzzles, random world events, map clues, and temporary camps.
- Daytime preparation tasks such as boarding a cabin, restoring a generator, lighting the helipad, or setting traps.
- Suggested locations/reward identities included beach/plane wreckage, jungle, village, fuel station, medical camp, radio tower, dock, and helipad. The final island layout must follow the user's map when supplied.

## Pitch and team context

The user specified a four-person team for pitch materials:

1. Lead developer and game designer
2. Environment and UI artist
3. 3D modeling artist
4. 3D modeling artist

Pitch deck revisions requested by the user: use third-person; say plane crash (not “catastrophic crash”); remove the resource-management audience category; base the island slide on the map the user will share; remove rifle from the deck's arsenal; replace “jump and survival” with a valuable-item search description; retain slides the user marked okay; add a thank-you slide. A prior deck had eight slides, but its original attachment content was not available in the retrieved history.

Funding discussion was exploratory, not approved. ChatGPT first estimated roughly ₹8–15 lakh (with ₹10–15 lakh as a pitch range), then revised upward to ₹14–24 lakh / a ₹20 lakh working target after the user said they would hire more people. Another discussion considered ₹90,000 for improvements including a historical estimate of about ₹9,000 for a US$100 Steam Direct fee. Reconfirm all amounts, hiring plans, and publishing terms before using them; these were not a final approved budget.

## Current Unity project snapshot

Observed in the project on 2026-09-26:

- Unity Editor: `6000.4.7f1`.
- Universal Render Pipeline is in use. The project has PC and Mobile render-pipeline assets.
- The build settings list `Assets/_Project/Scenes/Gameplay/SampleScene.unity` as an enabled scene. Other scenes include `Testing.unity` and `Sandbox/dump.unity`.
- The official Unity CLI MCP is registered in Codex for this project. Unity Pipeline package `com.unity.pipeline` version `0.8.0-exp.1` is installed; `unity pipeline list` reported the Editor reachable on port 7800.
- Project scripts/assets include a Rigidbody-based camera-relative player controller, player health/stamina/combat/animation, a day/night cycle, zombie state-machine/spawn/detection/health scripts, mission scripts and health/stamina UI. There are plane-wreck assets, zombie and player prefabs, knife/pistol assets, terrain, and vegetation.
- `DayNightCycle` source defaults to a 24-minute cycle, current time 8:00, day 06:00–18:00. Scene overrides may differ; inspect the serialized component before relying on those values.
- `MissionManager` currently holds one active mission and displays its marker; this is not the brainstormed multi-objective rescue system.
- `PlayerController` currently reads legacy `Input.GetAxisRaw` / `Input.GetButtonDown`, supports sprint and jump, and uses Rigidbody movement. Check the actual scene camera and input setup before changing controls or perspective.

## Historical performance work

A past screenshot analysis reported approximately 40,598 draw calls, 40,624 instances, about 291 million triangles, and 203 ms GPU frame time. These are historical screenshot values, not a current benchmark. The user explored rendering/culling only what the player can see, GPU instancing, and LOD setup. Prior tree troubleshooting found that an LOD Group had empty renderer lists; a later screenshot confirmed a low-poly tree mesh was instanced while the Terrain billboard was a separate draw. Re-profile before deciding what still needs optimization.

## Related ChatGPT conversations

- “Pitching Tomorrow Morning Game” — pitch corrections, team makeup, and exploratory funding.
- “Demo Planning” — planning a short playable vertical slice; some turns are stored as content references that were not recoverable in this export.
- “Gameplay Improvement Suggestions” — climbing removal and daytime ideas; proposals are unapproved unless separately confirmed.
- “Scene Lag Analysis” and “Player Animation Setup” — profiler and vegetation optimization discussions (the latter title covers optimization work in its retrieved messages).
