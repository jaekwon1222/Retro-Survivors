## 🎮 Retro Survivors

Retro Survivors is a minimalist, action packed survival game inspired by the rapidly growing auto combat genre. Built as a capstone project, the game explores how simple controls can support deep and engaging gameplay through enemy swarms, different combat builds, diverse upgrades, and progressively scaling waves. The goal is to create a tight gameplay loop where strategic progression and moment to moment survival are equally important.

---


![Gameplay GIF](Assets/Sprites/retrogameshow.gif)


## ▶️ How to Play (Unity)

To play **Retro Survivors**, you’ll need to open and run the project in **Unity**.

### Prerequisites
- **Unity Hub** installed  
- **Unity Editor version:** *(6000.2.6f2)*  
- **Git** (or download ZIP from GitHub)

---

### Setup Instructions

#### 1. Clone the Repository
```bash
git clone https://github.com/jaekwon1222/Retro-Survivors.git
```
---

### 2. Open the Project in Unity
- Launch **Unity Hub**
- Click **Open**
- Select the root folder of the cloned repository
- Open the project using the recommended Unity version

---

### 3. Load the Main Scene
- In the **Project** window, navigate to:
- Open the main gameplay scene (e.g. `MainMenuScene`)

---

### 4. Run the Game
- Press the **Play ▶️** button at the top of the Unity Editor
- The game will start in the editor

---

### Basic Controls
- **Movement:** WASD / Arrow Keys  
- **Attacks:** Manual Fire/Hold Fire on Cursor  
- **Upgrades:** Choose upgrades when prompted between waves  
- **Goal:** Survive as long as possible against increasingly difficult enemy waves


## 📖 How to Get the Latest Code

### 0) (First time only) Clone + set up Git LFS
```bash
git clone https://github.com/jaekwon1222/Retro-Survivors.git
cd Retro-Survivors
brew install git-lfs    # if first time on macOS
git lfs install
git lfs pull
```

### 1) Regular update from `main`
```bash
git checkout main
git pull --rebase origin main
git lfs pull
```

### 2) Check a feature branch (before merge)
```bash
git fetch --all --prune
git switch -c feature/branch-name --track origin/feature/branch-name
```

### 3) If you have local changes but need to pull latest
**Option A – Commit first**
```bash
git add .
git commit -m "wip: my local changes"
git pull --rebase origin main
```

**Option B – Stash temporarily**
```bash
git stash
git pull --rebase origin main
git stash pop
```

### 4) Resolve conflicts
- Open conflicting files in VS Code → choose **Accept Current / Incoming / Both**
```bash
git add .
git rebase --continue   # if you were rebasing
# or
git commit              # if you did a merge
```

---

✅ In short:  
- **Always pull from `main` before starting new work**  
- **Use feature branches for tasks**  
- **Commit or stash local changes before pulling**  
- **Resolve conflicts in VS Code, then continue**  
