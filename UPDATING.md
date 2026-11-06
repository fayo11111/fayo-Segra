# Updating this fork

Branches: `main` mirrors [Segergren/Segra](https://github.com/Segergren/Segra), `fayo` holds my changes and is the default branch.

## Normal flow (all in the browser)

1. **Upstream Sync** runs every Monday. When upstream has new commits it opens a pull request `Segergren:main → fayo`.
2. Open the PR (tab *Pull requests*), skim the commits.
3. Click **Merge pull request** with **Create a merge commit** (not squash, not rebase, so later syncs stay clean).
4. The merge triggers **Fayo Release**: it builds, then publishes `vX.Y.Z-fayo.N` with `Segra-win-Setup.exe` under *Releases* (~15 min).
5. The installed app finds the new release by itself, downloads it and installs it when idle (or via the update card in the sidebar).

Run a sync or a build by hand: *Actions* → *Upstream Sync* / *Fayo Release* → **Run workflow**.

## Merge conflicts

If GitHub says the PR has conflicts, don't resolve them in the web editor. Start a new Claude Code session on this repo and say:

> Merge the open upstream sync PR into `fayo`, resolve the conflicts keeping my fork changes (see UPDATING.md), push, and make sure Fayo Release succeeds.

## My changes and the files they touch

New files (never conflict): `Frontend/src/fayo/*`, `Backend/Games/GameResult.cs`, `Backend/App/ForkConfig.cs`, `.github/workflows/fayo-release.yml`, `.github/workflows/upstream-sync.yml`, `UPDATING.md`.

| Change | Upstream files touched |
|---|---|
| Release + sync workflows (unsigned build, version `-fayo.N`) | none |
| Auto-updater uses this fork's releases (`ForkConfig.cs`) | `Backend/App/UpdateService.cs` (2 lines) |
| Obsidian theme, Geist fonts (`fayo/theme.css`) | `Frontend/src/globals.css` (1 import line) |
| Logo, icons, splash, flat sidebar | `Frontend/src/menu.tsx` (~8 lines), `Frontend/src/Components/Button.tsx` (nav style, 1 line), `icon.ico`, `icon.png`, `Resources/icon.ico`, `Resources/iconRecording.ico`, `Resources/splash.png` |
| Game score/result for League + CS2 (`GameResult.cs`) | `Backend/Core/Models/Settings.cs` (1 property), `Backend/Media/ContentService.cs` (1 line), `Backend/Games/LeagueOfLegends/LeagueOfLegendsIntegration.cs` (1 line), `Backend/Games/CounterStrike2/CounterStrike2Integration.cs` (1 line) |
| Session banner rows, K/D/A, grid/list toggle (`fayo/SessionStats.tsx`) | `Frontend/src/Components/ContentPage.tsx` (~40 lines), `Frontend/src/Components/ContentCard.tsx` (3 lines) |

The banner rows are pure CSS on top of upstream's `ContentCard` markup (`.fayo-rows` in `fayo/theme.css`). If upstream restructures that card, the rows may look off after a merge: fix the selectors there.
