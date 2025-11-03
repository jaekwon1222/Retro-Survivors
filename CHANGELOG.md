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

📘 **Next Version Goal (v0.8.0 - Boss & Advanced Waves)**  
- Add boss spawn every 5 waves.  
- Implement special attack patterns.  
- Add visual damage feedback & screen shake.  
- Introduce achievements and weapon variety.