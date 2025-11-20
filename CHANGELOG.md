# Retro Survivors — Development Log

## 1. Unity Project Setup
- Created new Unity 2D project titled **Retro Survivors**.  
- Linked to GitHub via SSH, configured `.gitignore` for Unity and OS temp files.  
- Established folder structure:
  ```
  Assets/
    ├─ Scripts/
    ├─ Prefabs/
    ├─ Sprites/
    ├─ Scenes/
  ```

---

## 2. Scene Setup
### Scene_MainMenu
- Built UI Canvas with background, title, and buttons: **Start / PowerUp / Settings / Quit**.  
- Connected `MainMenuController.cs` to handle button events.  
- Linked `UIManager.cs` for general UI handling (hearts, wave, score).  

### Scene_Entry
- Added core systems: **GameManager**, **UIManager**, **AudioManager**, **InputHandler**.  
- Verified scene transitions: **Start → Entry**, **Quit → Application.Quit**.

---

## 3. UIManager & HUD
- Implemented `UIManager.cs` for HP hearts, Wave counter, Score counter.  
- Added heart prefab to display 10 total hearts.  
- `Damage()` disables hearts visually when hit.  
- Added debug keys: **H (damage)**, **N (wave++)**, **K (score++)**.

---

## 4. Player Setup
- Created Player prefab with SpriteRenderer, Rigidbody2D, and Collider2D.  
- Added `PlayerController.cs` for WASD movement.  
- Fixed scaling issues causing invisible sprites.

---

## 5. Enemy Spawning System
- Added `WaveManager.cs` for wave-based enemy spawns.  
- Base: **5 enemies × 6 batches = 30 per wave**.  
- Each wave increases enemy count by **+2**, total **5 waves**.  
- Spawned from 4 corner points or dynamically around camera.

---

## 6. Enemy Behavior
- `Enemy.cs`: enemy follows player, has hp, maxHP, moveSpeed, scoreValue.  
- `TakeDamage()` + `Die()` functions; triggers score increment.  
- Melee attack via collision; damages player and triggers blood effect.

---

## 7. Player Auto Fire System
- `PlayerAutoFire.cs`: auto-fires projectiles every second.  
- Targets nearest enemy.  
- Projectile prefab destroys itself on hit and calls `Enemy.TakeDamage()`.

---

## 8. Pause / Resume System
- ESC key pauses game via `UIManager`.  
- `Time.timeScale = 0` while paused.  
- Resume and Exit buttons functional.

---

## 9. Enemy HP Bar System
- `EnemyHealthBar.cs`: auto-instantiates HPBar prefab above each enemy.  
- Auto-detects BG/Fill sprites and scales Fill by HP ratio.  
- Destroys HPBar when enemy dies.  
- Handles sorting, materials, and pixel snapping.  
- Fixed overlapping/pink material bug via Unlit material fallback.

---

## 10. Upgrade System
- Integrated all upgrades into a **single weighted random upgrade system** (Normal + Grand unified).  
- Implemented **probability-based selection** for six total upgrades.  
- “**Projectiles +1**” now adds multi-target firing (each projectile tracks a different enemy).  
- Fixed **wave-clear timing** so upgrades appear only after all enemies die.  
- Improved UI font size and bold style for upgrade buttons.  

### 🎯 Upgrade Pool
- **Power +1 (23%)** — Increases projectile damage  
- **Move Speed +5% (23%)** — Increases player movement speed  
- **Heal +1 Heart (23%)** — Restores one heart  
- **Projectiles +1 (10%)** — Adds one more projectile (targets additional enemy)  
- **Full Heal (10%)** — Restores all HP to max  
- **Big Hit Radius +0.2 (10%)** — Increases projectile area effect radius  

---

## 11. Audio & Visual Enhancements
- Added **infinite background scroll** system for seamless camera movement.  
- Implemented **background music** for both Main Menu and In-Game scenes using `MusicManager`.  
- Created **persistent SFXManager** (with `DontDestroyOnLoad`) to handle all sound effects globally.  
- Integrated SFX triggers across gameplay:  
  - Shooting → `PlayShoot()`  
  - Enemy death → `PlayEnemyDie()`  
  - Player hit → `PlayHit()`  
  - Heal / Full Heal → `PlayHeal()`  
  - Upgrade panel open → `PlayUpgradeOpen()`  
  - Button click → `PlayClick()`  
- Adjusted UI sound balance and verified playback across scene transitions.  
- Confirmed that audio works correctly with time scale pauses (upgrade and pause menus).  
- Finalized **Upgrade Panel sound effect** (triggered once per panel open).

---
## 12. UI & Gameplay Enhancements

- **Main Menu UI Overhaul**
  - Replaced all image-based buttons with standard Unity TMP Buttons for visual consistency.
  - Added hover and pressed color transitions:
    - Normal: `#2D6BFF`
    - Highlighted: `#5C8CFF`
    - Pressed: `#1E4FCC`
  - Re-linked all MainMenuController button events:
    - **Start** → loads `Scene_Entry`
    - **Settings** → opens Settings Panel
    - **Quit** → exits the application

- **Settings Panel Implementation**
  - Created new UI panel containing:
    - Music Volume Slider → connected to `MusicManager.SetVolume()`
    - SFX Volume Slider → connected to `SFXManager.SetVolume()`
    - Mute Toggle → controls both music and SFX
    - Back Button → returns to main menu
  - Adjusted text alignment and toggle positioning for better visibility.

- **Upgrade Status Panel (In-Game HUD)**
  - Added top-left panel displaying real-time upgrade information:
    - Projectiles
    - Power
    - Fire Rate
    - Move Speed
    - Hit Radius
    - Pierce
  - Implemented `UpgradeStatusPanel.cs` for automatic data updates every 0.2s.
  - Added support for `ProjectilePierce` and `FireInterval` in `PlayerAutoFire`.
  - Added `CurrentSpeed` property to `PlayerMovement` for live MoveSpeed display.

- **UI & Gameplay Integration**
  - Verified all upgrade stats correctly sync between UI and gameplay.
  - Adjusted font sizes and formatting for improved readability.
  - Tested full gameplay flow to ensure no data delay or desync between upgrades and display.

## 13. Game Over System

Implemented a complete Game Over system triggered when the player's HP reaches zero.

### Features
- Added GameOverPanel UI with dark overlay, “You Died” message, and two buttons:
  - **Restart**
  - **Main Menu**
- Gameplay now pauses on death (Time.timeScale = 0).
- Mouse cursor becomes visible and unlocked on death.
- Restart button:
  - Restores time scale
  - Reloads Scene_Entry
  - Resets all HP and player stats
  - Recreates a fresh UIManager instance
- Main Menu button returns to Scene_MainMenu with proper cursor handling.
- Fixed issue where hearts did not reset on restart.
---
## 14. Strong Enemy System (WIP) + Stability Updates

### Added
- Introduced **Strong Enemy (mid-tier)**:
  - Larger sprite and adjusted local scale
  - Custom HP and custom contact damage value
  - Spawns ~1 second after normal enemies are finished spawning
  - Number increases per wave
- Adjusted Strong Enemy hitbox so collision detection fits the large sprite

### Fixed
- Restart now correctly restores player HP and resets upgrade stats
- Cleaned up UI references to prevent destroyed Image exceptions

### Known Issues
- Strong Enemy still deals **1 damage** regardless of its assigned damage stat  
  → The damage value is not passing correctly to `DamagePlayer()`  
  → This will be addressed in the next development session.

### Status
The feature is partially implemented and functional, game runs normally.
Only the damage logic for the Strong Enemy is incomplete.

