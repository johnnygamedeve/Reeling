Axioms


We make dynamic, cooperative, sci-fi and fantasy games that immerse the player in a virtual space emphasizing interactivity by using core mechanics to overcome compelling challenges. 
Co-op (Gameplay gameplay gameplay)
Core Mechanics (Avoid mechanical overlap)
Compelling challenges (Gameplay should lead to gameplay. A master player should have MORE to deal with or less tools to deal with it, NOT the other way around) 
Great Controller to Interaction feel
Idea

Couch co-op Third Person Fishing Game, fish with your  friend. Hang out while drinking a beer with a friend. Chill experience, think power wash simulator.

Fixed Camera Fishing game for 2 players
Game Overview
Title: Reeling
Platform: S&box 
Players: 1-2, local co-op or online multiplayer 
Genre: Chill / relaxation
Pitch A two player fishing game set at a permanent golden hour. No UI, no progression, no rewards. Just two people sitting by the water, casting a line, and listening to music from a boombox. The goal is not to win — it is to be there.
Core fantasy The player should feel like they are actually sitting somewhere beautiful with nothing to do but fish.
Design pillars
No clutter. No HUD, no menus during play, no numbers on screen.
Everything in the world. All feedback is physical — sound, rumble, animation, VFX.


Core Loop

1.Idle: 
Rod has not been cast, character just idles

2.Cast: 
The player holds L2 to play out the casting animation and the player presses R2 2 seconds into that animation, any later or earlier and the casting is a shorter distance.

3.Wait:
You can either wait, a fish will show up in about 40 seconds, or you can start reeling slowly, at about 30 rpm we will let testing decide, so fishes will see the bait move and want to take it, if you do it to fast fish wont catch, otherwise this will assure some catchings earlier

4. Bite:  
rod starts being pulled, making a sound fx, a vfx of water moving splashing where the rod has been cast appears, player catches the rod before it falls as a no input animation, and the reeling minigame starts.

5. Reel:
5a. When the fish is pulling (water splash vfx moving away from the player), player has to reel in slowly, same 30 rpm but we will test, with the right stick so the fish won't get too far that it escapes, about 100 meters but we will decide in engine with engine units later, but also not generate so much friction the line breaks, lets say it has a health bar and any rpm above 30 damages the healthbar directly so player loses health per second we will decide the health of the line in testing.
5b. When the fish is not pulling (no water splash vfx) the player can reel as fast as possible with no issues, higher rpm the closer the fish will get.

6. Outcome

Catch → Animation holding and admiring the fish goes on then pressing R2, fishes are all the same but different sizes. Fish is thrown back in the water. Return to Idle.
Fail →  Player does a fall back animation, if the line broke he has to reset the line (there are unlimited resets), this would happen automatically through an animation, and then goes back to idle.


Control reference

Input
Action
Notes
Cast Phase
L2 hold
Begin cast animation
Holds until R2 is pressed
R2 press
Release cast
At 2s = max distance. Earlier or later = shorter cast
Wait Phase
Right stick ↑
Slow bait movement
Stay below ~30 RPM or fish are spooked for 10s
Reel Phase
Right stick Circles either direction during fish pull
Reel in slowly
Stay below ~30 RPM. Above threshold damages line HP
Right stick Circles either direction when fish not pulling
Reel in fast
No RPM limit. Higher RPM = fish approaches faster
Catch Phase
R2 press
Confirm and release fish
Pressed after admire animation plays
Anytime
Options press
Pause menu
Interact with menu normally with X and O
Anytime Outside of reel phase
Square press 
Turn boombox on/off 
Disabled during reel phase 
Left D-pad 
Previous boombox song 
Disabled during reel phase, only functional when boombox is on 
Right D-pad 
Next boombox song 
Disabled during reel phase, only functional when boombox is on 


Fishing system
Spawn & bite timing:
When the line is in the water a timer starts with a random value between 20 and 60 seconds. When the timer expires a fish bites. The player has no way to see the timer. If the player moves the bait slowly below the 30 RPM threshold during the wait phase, the timer is shortened. Exact reduction amount to be decided in testing.

Size:
Fish size is determined by a random weighted roll at the moment of bite. Smaller sizes are significantly more common. Larger sizes are rare. There are no species differences — all fish are the same model scaled to the rolled size. Size has no effect on gameplay difficulty, pull strength, or reel behaviour. It is a purely cosmetic outcome and exists to give players an informal bragging metric.

Escape conditions:
A fish escapes if it travels approximately 100 meters from the cast point during the reel phase, or if the line HP reaches zero. Both return the player to Idle. Exact distance to be confirmed in engine units during implementation.

Feedback & Diegetic Design
Cast timing:
 The fishing line lags behind the rod during the backswing. When the line reaches approximately 90 degrees the throw is at maximum power. No UI prompt. The visual is the cue.

Reel RPM: 
Controller rumble intensity increases gradually as the player moves away from the 30 RPM (controller rumbles least at 25 RPM and rumbles most at 125 RPM) threshold during the fish pulling state. A line tension audio cue mirrors the rumble. No on screen indicator. Final rumble curve and audio to be tuned in testing.

Line HP:
Not communicated to the player. The line breaks without warning at zero HP. This is intentional.
Art & Audio Direction 
Visual style:
Deep silhouette aesthetic. All environment geometry, characters, and props render as flat black shapes against a warm golden sky. The only light sources are the sky, the water surface reflection, and any explicitly lit interactive objects. No textures, no surface detail, no colour variation on solid objects. Reference: the screenshots provided by the designer.
Colour palette:
Sky and water are warm golden orange, approximating a permanent golden hour. Background and character are flat black. The rod, line, and water VFX are pure white. No time of day cycle. The lighting state is fixed. Exact hex values to be confirmed during implementation but should sit in a deep amber to warm orange range.
Camera:
Third person fixed perspective during idle and cast phases. When the reel minigame begins the camera quickly zooms in toward the water, tightening focus on the fishing line and water VFX. When the outcome resolves the camera slowly zooms back out to the default position. Zoom speed and distance to be tuned in testing.
VFX:
Water splash and movement effects are the primary game feedback system and should be clearly readable against the golden water surface, they will be pure white with no shadows.
Boombox:
The boombox is a physical object in the world. When off it renders as a flat black silhouette like everything else. When on the buttons are illuminated. No UI overlay. Players interact with it directly in the world. Music track selection to be designed separately.
Audio:
Ambient nature soundscape as the base layer at all times — water, wind, wildlife. No background music from the game itself. All music comes from the boombox object in the world and is therefore optional and player controlled. Key sound design moments: line tension creak during reel, water splash on bite, cast whoosh, reel click rhythm.




World & Environment 
Location:
Single fixed location. One handcrafted environment, not procedurally generated. The setting is a lakeside at a permanent golden hour — warm water in the foreground, mountain silhouettes in the background, foliage framing the left and right edges. Reference screenshots provided by the designer.
Player movement:
None. The player has no movement controls. The game begins with both players already seated at the fishing spot side by side. The only interactions available are fishing and the boombox.
Multiplayer structure The game supports two players. Two modes:
Couch co-op — both players share a single screen and a single camera. The camera behaves as one unit, zooming in and out based on whichever player is currently in the reel phase. If both are reeling simultaneously the camera stays zoomed out to fit both.
Online multiplayer — each player has their own independent camera behaving exactly as described in the Art & Audio Direction section.
World objects:
Two player characters, two fishing rods, and one shared boombox (boombox controls are disabled during the reel phase to avoid accidental input). The boombox is a shared object — either player can interact with it. No other interactive objects planned for V1.
Technical Notes 
Platform:
S&box gamemode.
Player characters:
Players use either their S&box avatar or a default human character. Both should work within the silhouette art direction as flat black shapes. No custom character model required for V1.
Networking:
Built on S&box networking model. Online multiplayer supports two players with independent cameras. Each player's fishing state, fish position are networked values. Boombox state (on/off, current track) is a shared networked object visible and interactable by both players.
Animation:
Uses S&box animation system. Key animations required: idle seated, cast backswing, cast release, rod pulling on bite, rod catch animation, reel loop, catch admire, fail fallback, line reset. All animations play on a seated character with no movement.
The fishing line:
The line is the primary technical risk of the project. It must appear as a natural physics rope visually while simultaneously moving in specific ways depending on the distance of the throw and the exact same way during the pre cast animation so the player has the exact same tell every time on when to cast
Couch co-op camera The shared camera in couch co-op mode is a known technical challenge. The camera must frame both seated players at rest, zoom in on the active player during reel phase, and handle the edge case of both players reeling simultaneously by staying wide enough to frame both. Camera rig behaviour to be prototyped early.
Known technical risks
Fishing line movement 
Couch co-op shared camera logic
RPM detection accuracy from right stick input
Scope & V1 Limits
This is the complete game. The following is not a list of cut features — it is a list of things that were considered and deliberately excluded to keep the experience focused.
Excluded by design
No fish species. Fish are size variants only, determined by a random weighted roll.
No environmental modifiers. Weather, time of day, and location have no effect on gameplay.
No progression system. Players earn nothing, unlock nothing, and track nothing.
No fish journal or catch log.
No HUD or UI of any kind. All feedback is diegetic.
No player movement. Players are seated at the fishing spot for the entire session.
No line HP indicator. The line breaks without warning.
Pending design
Boombox music track selection. The system is designed but the actual tracks are not yet decided.







