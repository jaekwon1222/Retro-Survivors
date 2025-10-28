# Retro Survivors

A 2D survival game project built with Unity.

---

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
