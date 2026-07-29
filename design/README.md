# Handoff: Unmatched tracker — full redesign

## Overview

Unmatched is a personal hub for logging board-game matches (.NET 9 + Blazor Server, existing repo
`Unmatched.UI.BlazorServer`). This package redesigns the whole front end: navigation, hero
statistics, hero detail, match log, add match, and collection. The app is **not per-user** — one
owner logs everybody's games — so no screen is written from a single player's point of view.

## About the design files

The two `.dc.html` files in this bundle are **design references written in HTML** — living
prototypes of the intended look and behaviour, not production code to copy. Implement them inside
the existing Blazor Server app using its own patterns (Razor components, scoped CSS or the existing
`wwwroot/css` theme). Port the *values and structure*, not the file format: the prototypes use a
small in-house template runtime (`support.js`) that has no place in the target codebase.

Open them by serving the folder over any static server (they need `support.js` and `assets/`
side by side) — `python3 -m http.server` in this folder, then open the file in a browser.

- `Unmatched Design System.dc.html` — tokens + component library. **Read this first.**
- `Unmatched Redesign.dc.html` — a design canvas with all approved screens, newest turn at the top.
  Each option has a visible id badge (`1c`, `2a`, `3b`, `4b`, `5a`, `6a`) referenced throughout
  this README.

## Revision 2 — what changed after the first implementation

The first build shipped and exposed ten problems; they are all designed now and **override the
matching parts of the sections below**. Full change order in `IMPLEMENTATION-PROMPT.md`.

| # | Fix | Reference |
|---|---|---|
| 1 | Hero / player art was too small in grids: list rows now carry a **46px** hero token with a 15px name, and every list has a **List ⇄ Tiles** switch (88px round art tiles) | `9a` |
| 2 | **Every** grid gets a per-page control in its footer (10 / 20 / 50 / 100) | `9a`, `9b` |
| 3 | Play style on the hero page is **editable** — three clickable squares per axis — with the radar beside it, updating live | `9b` |
| 4 | Sidekick has no artwork, so it renders as **text only** (name + HP) | `9b` |
| 5 | Hero / villain / minion / map / player pages carry **named prev / next** at the top (token + entity name + position "01 of 42") | `9b` |
| 6 | Collection tiles enlarged: 3-up, 200px-tall covers, hero tokens 40px, map thumbs 46px | `8c` |
| 7 | The collection detail panel is **sticky** — stays in view while the shelf scrolls | `8c` |
| 8 | Filter dropdowns are **multi-select** — checkbox rows, selection count on the trigger, chips with ×, "Clear (n)" inside and "Clear all filters" outside | `8h` |
| 9 | All controls step up one size: fields / buttons **36px** (nav items 40px), text 13–13.5px, dropdown rows 38px, page buttons 36px | `8h`, everywhere |
| 10 | Add match rebuilt: map banner with full-screen view, draggable fighter plates that set turn order, winner marked on the fighter/team, losers auto 0 HP + red cross | `9c`–`9g` |

## Fidelity

**High fidelity.** Colours, type, spacing, radii and interactions are final. Recreate pixel-close
at ~1240px desktop width. Every interaction described below is live in the prototype — click it
before implementing. Mobile is explicitly **out of scope** for now.

### Screens that exist vs. screens that don't

The screens documented here (`1c`, `2a`, `3b`, `4b`, `5a`, `6a`, `7a`) are **designed and approved — build
them exactly as they are**, down to the values in this README; do not improvise alternatives.

The app also has screens that were **not** designed yet: Players list, Player detail, Maps, Minions,
Villains, Titles, Tournaments, settings and any dialogs. Build those **in the same visual language**,
assembled only from the design-system parts: same nav rail, same page header (breadcrumb + search +
controls + one gradient primary action), same KPI strip, same panel/card/row shapes, same type and
colour tokens. Concretely — a list screen (Players, Maps, Minions, Villains) follows the `1c` ladder
pattern; a detail screen (Player, Map, Villain) follows the `2a` dossier pattern; anything showing
matches reuses the `3b` row verbatim. Do not invent new colours, fonts, radii, chart types or layout
idioms for them.

## Design tokens

### Colour

| Token | Value | Use |
|---|---|---|
| `bg-app` | `#0a0a10` | page background |
| `bg-panel` | `#101018` (also `#12121a`) | cards, panels, tables |
| `bg-nav` | `#0c0c14` | nav rail, sticky sub-bars |
| `bg-inset` | `rgba(255,255,255,.045)` | fields, chips, inset rows |
| `border` | `rgba(255,255,255,.07–.09)` | all hairlines |
| `border-strong` | `rgba(255,255,255,.14)` | medals, small badges |
| `text` | `#ecedf2` | primary |
| `text-muted` | `#8b8c9e` | secondary |
| `text-dim` | `#6b6c7e` | labels |
| `text-faint` | `#5c5d70` | captions, mono meta |
| `accent` | `#6c5cff` | primary action, charts |
| `accent-2` | `#a855f7` | gradient end |
| `accent-soft` | `#8b7dff` | icons, 1v1 mode |
| `accent-text` | `#c4bcff` | links, active values |
| `win` | `#34d99b` | wins, positive deltas |
| `loss` | `#f2506b` | losses, destructive |
| `info` | `#38bdf8` | co-op mode, minions, maps |
| `gold` / `silver` / `bronze` | `#f0b429` / `#c9cad6` / `#cd7f32` | 1st/2nd/3rd, epic stars, FFA, villain |
| `mode-2v2` | `#c084fc` | team matches |

Primary action = `linear-gradient(150deg,#6c5cff,#a855f7)`, white text, **once per screen**.
Selected surface = `rgba(139,125,255,.1)` + border `rgba(139,125,255,.55)` +
`box-shadow:0 10px 30px rgba(108,92,255,.22)`.
The old red `#e63946` theme is retired — do not reintroduce it.

### Typography

Google Fonts: **Chakra Petch** (600/700), **Barlow** (400–600), **IBM Plex Mono** (400/500).
The brand `Unmatched.otf` is no longer used.

| Token | Spec |
|---|---|
| display | 700 34px Chakra Petch, ls .03em, uppercase |
| page-title | 700 24px Chakra Petch, ls .02em, uppercase |
| card-title | 600 15px Chakra Petch, ls .06em, uppercase |
| section-caption | 500–600 10.5px IBM Plex Mono, ls .14–.18em, uppercase, `#6b6c7e` |
| body | 400 13px/1.6 Barlow |
| ui-label | 500–600 12.5px Barlow |
| metric | 700 19px Chakra Petch (large metrics 22–34px) |
| mono-meta | 400 11px IBM Plex Mono |
| mono-caption | 500 9.5px IBM Plex Mono, ls .12–.14em, uppercase |

### Spacing, radius, elevation

- Spacing step 8: gaps 8 / 12 / 14 / 16 / 18 / 22 / 26. Page padding 26px horizontal, 16–20px vertical.
- Radius: 6–9px chips and small controls · 10–11px rows and inputs · 12–13px cards · 14–16px panels.
- Control heights: 28px filter chip, 32px header control, 36px form field / primary button.
- Shadow only on the selected card and the design-canvas frames; panels are flat with hairline borders.

## Screens

### Nav rail (all screens)

Fixed left column, `#0c0c14`, right border hairline, padding 16px/12px, gap 16px.
Width **216px expanded / 76px collapsed**, transition `width .18s ease`. Collapsed shows icons only,
centred, with `title` tooltips; header swaps the 112px inverted `Unmatched-logo.png` for a 32px
gradient "U" tile. A **Collapse** button pinned at the bottom (34px, outlined, `bi-chevron-double-left/right`).

Groups and items (Bootstrap Icons 1.11.3, 15px):
- **Statistics** — Heroes `bi-shield-fill` 48 · Players `bi-people` 6 · Maps `bi-map` 20 ·
  Minions `bi-bug` 6 · Villains `bi-eye` 2
- **Play** — Add match `bi-plus-circle` · Match log `bi-list-ul` 1284 · Collection `bi-box-seam` 7/10 ·
  Titles `bi-award` 9 · Tournaments `bi-flag` 3

Item: height 38px, radius 9px, gap 11px, label 500 13px Barlow, trailing count 400 10px mono `#5c5d70`.
Active = gradient `linear-gradient(150deg,rgba(108,92,255,.9),rgba(168,85,247,.7))`, white text.
Hover = `background:rgba(255,255,255,.05)`, text `#ecedf2`.

### 1c — Heroes / ladder (landing screen)

Dense ranked list of heroes with a right-hand analysis column.
Row: `border-left:3px solid` result colour + a 4–7% wash of the same hue; rank number in Chakra Petch
(gold/silver/bronze for top three), 32px rounded hero token, name + set, melee/ranged icon
(`assets/Melee.png` / `assets/Ranged.png`, `filter:invert(1)`, opacity .5), points (`#c4bcff`, red when
negative), matches, W/L, K/D (green ≥ 1, red below), win %.
Hovering a row updates the right column (spotlight hero: donut win-rate, radar play style, form
sparkline, matchup heatmap, mini stats); clicking opens the hero page. Tabs above the list:
Points / Win rate / K/D / Recently played. Pagination at the bottom.

### 2a — Hero detail (dossier)

Order, top to bottom:
1. Full-width hero banner (art, name in display type, set, melee/ranged, HP, deck size).
2. Four stat cards (matches, win rate, K/D, points).
3. Play-style radar (Attack / Defence / Trickery / Difficulty, 0–3), titles + sidekick, donut win-rate.
4. Full-width rating line chart.
5. Antagonist card + Matchups card, two equal columns.
6. Match log excerpt (row style 3b).
7. Against villains / Against minions.

**Matchups is paged, not truncated.** The card header carries a range label (`1–5 of 12`) and ‹ ›
buttons (24×24px, radius 7, border hairline, arrow colour `#8b8c9e`, `#3c3d4c` when the end is
reached). Five rows per page, **sorted by win rate against that opponent, descending**, each row:
hero token, name, `N met`, 56px bar, win %. Caption under the header: "Sorted by win rate against".

### 3b — Match row (shared component)

The single row shape used by the match log and any "recent matches" block.

Grid `82px 84px 1fr 116px 60px 24px`, gap 12px, padding 12px 16px, hairline bottom border,
`border-left:3px solid`.
- **Date** — always with the year (`12 Jul 2026`) plus time in 9.5px mono below.
- **Mode badge** — 1v1 `#8b7dff` · 2v2 `#c084fc` · FFA `#f0b429` · CO-OP `#38bdf8`;
  the badge only *confirms* the shape the row already shows.
- **Participants** — sides. A side is a group of fighter chips; sides are joined by `VS`.
  A chip = 30px rounded hero art (green ring + full opacity when it won, `rgba(255,255,255,.09)`
  ring + 50% opacity when it lost) with the player's 15px round avatar pinned bottom-right.
  In FFA the separator is `›` and each chip carries a place badge top-left (gold/silver/bronze/grey).
  Villain and minion chips carry **no** player avatar.
- **Map**, **comment indicator**, **Epic**, chevron.

**Epic stars.** Filled = `bi-star-fill` gold `#f0b429`; empty = the **outline** glyph `bi-star` at
`rgba(255,255,255,.16)`. Never a dimmed gold fill — at row size it reads as half-lit. 11px in rows,
15px in detail panels, 17px when editable.

**Comment.** When a match has a comment, the row shows a 10.5px `bi-chat-left-text-fill` in
`#6b6c7e` (own 22px column between Map and Epic) with the text as its `title`. The full text lives in
the expanded panel footer: a 11px-radius inset strip holding `EPIC` + 15px stars on the left, a
hairline divider, then `COMMENT` + the text at 400 12px/1.55 Barlow (`#c9cad6`; when empty, "No
comment on this match." in `#5c5d70`), and an outlined **Edit match** button on the right.

Click toggles a **scoreboard** listing *every* participant:
columns `Participant · Side · HP left · Cards · Sidekick`.
Side column appears **only** for 2v2 (Team 1 / Team 2) and co-op (Heroes / Villains).
Minions are indented 20px under their villain with a dash for cards. Sidekick shows `Name · Nhp` or a dash.
Never show items used, actions made, or time spent — the app does not track them.

### 6a — Match log (page)

Nav rail · header (breadcrumb Play › Match log, search, Export, primary **Add match**) ·
four KPI cards (Matches 1 284 / This month / Avg. epic / Longest streak) ·
filter bar: a segmented mode filter (All · 1v1 · 2v2 · FFA · Co-op — **actually filters the list**)
plus dropdown chips for players, heroes, maps, tournaments, date range, with the result count on the right ·
the list of 3b rows in a rounded panel · pagination.

**Rows per page.** The footer carries the page label, a divider, the caption `PER PAGE` and a
segmented 10 / 20 / 50 / 100 control (same styling as any segmented control: 3px padding track,
7px radius items, active `rgba(139,125,255,.18)` + `#c4bcff`). The default comes from
Settings → *Matches per page*; the footer control overrides it for the current session only.

**Rows are not coloured win/loss here.** The app is a hub for everybody's matches, so the left edge
carries the *mode* colour and the result is only visible inside the expanded scoreboard.

### 4b — Add match (one sheet)

No stepper — everything on one screen.
Header with a segmented mode switch (1v1 / 2v2 / FFA / Co-op) that rebuilds the participants area.
Left: participants as side boxes matching 3b. Fighter fields: Player, Hero (with a violet dice
randomize button — present **everywhere** a hero is chosen), HP, Cards, Sidekick.
Right column (280px): map preview, Map with dice, Tournament, Stage, Date, Comment, Epic
(three bare stars, no box). Footer: summary chips + **Save match** and **Save & add another**.
- FFA: minimum 3, maximum 4 players ("Add player" disappears at 4; the trash icon returns to 3);
  every fighter has a Place dropdown with a medal.
- Co-op: the villain box has "Add minion"; a minion row = art + count + HP left + trash.
  The villain has no sidekick and no cards.

### 5a — Collection (page)

Header: breadcrumb, search, **Owned only** toggle, unsaved-changes counter, **Save collection**.
KPI strip: Sets · Heroes · Maps · Villains · minions (owned / total).
Left: a two-column shelf of expansion cards — 76×97 box cover (real art where available, otherwise a
dashed placeholder with the inverted logo and the mono caption "box art"), name, year, content chips
(`4 heroes`, `2 maps`, `1 villain`, `5 minions`), overlapping hero tokens, and a **20px checkbox in the
top-right corner of the cover** that adds/removes the set. Checkbox click must not change the selected
set. Status caption next to the year: `OWNED` (green) · `ADD` (grey) · `UNSAVED` (gold, with a gold
checkbox border). Unowned sets are dimmed (opacity .5) with a dashed cover frame.
Right panel (396px): the selected set's contents — cover, name, publisher/year, own/unown CTA,
three mini stats, then sections **Heroes** (token, deck size, type, HP), **Maps** (44px thumbnail),
**Villain & minions** (villain row plus indented minion rows with counts).
**Save collection** commits all pending checkbox changes at once.

### 7a — Player profile

A player is not a hero — the page is about **who they beat and what they play**.

1. Header: breadcrumb Statistics › Players › Name, prev/next player buttons, primary **Add match**.
2. Identity band on a soft violet gradient (`linear-gradient(105deg,rgba(108,92,255,.14),rgba(168,85,247,.05) 45%,transparent 78%)`):
   92px round avatar with a `rgba(139,125,255,.5)` ring, name in display type, a gold rank pill
   (`#2` + `of 6 players`), mono meta line, and title chips.
3. **Three** KPI cards only: Matches, Win rate, K/D. **No Points and no Form card.**
4. Rating changes line chart (2fr) + By game mode bars (1fr, one bar per mode in the mode colour).
5. **Heroes played** — full-width panel listing **every hero the player has played**, internal scroll
   (`max-height:430px`), sort segmented control (played / win % / k/d / my rating), filter field, and a
   count. Columns: `Hero | Matches | W/L | Win % | K/D | Main | My rating`.
   - **No rank numbers and no gold/silver/bronze highlighting** in this list — it is not a ladder.
   - **Main** is a bookmark toggle, 26px, **exactly one hero per player**: active =
     `bi-bookmark-star-fill` gold on `rgba(240,180,41,.14)` with a `rgba(240,180,41,.45)` border, the
     hero token gets a gold ring and the row a 5% gold wash; the panel header shows `MAIN · <HERO>`.
   - **My rating** is a subjective **1–5 star** control in accent violet `#c4bcff` (empty = outline at
     `rgba(255,255,255,.16)`), clickable per hero. It never feeds the ladder or any computed stat.
6. Rivals (head-to-head W/L bar per player) + Best maps, two equal columns.
7. Titles earned — four cards.
8. **`<Name>`'s matches** — own section, and here rows **are** coloured by result: 3px left edge
   `rgba(52,217,155,.7)` / `rgba(242,80,107,.7)` with a 5% wash of the same hue, a WIN/LOSS badge,
   the hero the player played, `vs` opponent chips, map, `HP / opponent HP`, epic stars. Header shows
   W and L tallies and a link to the full match log; footer has "Showing 6 of 168" + Load more.

New backend needs for this screen: `IsMain` (one per player) and a per-player, per-hero subjective
rating 1–5.

## Interactions & state

| State | Scope | Notes |
|---|---|---|
| `navOpen` | global | 216 ⇄ 76px rail, persists across screens |
| `openMatch` | match log | id of the expanded row, or null; only one open at a time |
| `logMode` | match log | `all \| 1v1 \| team \| ffa \| coop` |
| `addMode` | add match | rebuilds the participants area |
| `colSet` | collection | index of the set shown in the right panel |
| `colDraft` | collection | pending owned-flag changes, keyed by set; drives the gold UNSAVED state |
| `colSaved` | collection | committed owned flags, written by Save collection |
| `logSize` | match log | rows per page (10/20/50/100), seeded from Settings |
| `muPage` | hero page | Matchups carousel page |
| `plMain` | player page | the player's single main hero |
| `plRatings` | player page | per-hero subjective 1–5 rating |
| `plSort` | player page | hero list sort: played / win % / k/d / my rating |
| ladder hover | heroes | hovered hero drives the right analysis column (not yet built) |

Transitions are short and functional: `width .18s ease` on the rail, background/border colour on hover.
No decorative motion.

## Data model (from the existing repo)

`GameMode` = OneVsOne · TeamVsTeam · FreeForAll · Cooperative.
`UiFighterDto`: Hero, Player, Team, Placement, HpLeft, CardsLeft, SidekickHpLeft, ActionsMade, Turn, MatchPoints.
`UiMatchLogDto`: Fighters, MapName, Epic, Comment, TournamentName, optional `Villain` (`UiMatchVillainDto`)
with a `Minions` collection.
Any match list must therefore tolerate a variable number of participants — that is exactly what the
sides + scoreboard shape in 3b is for. There is no art for villains and minions: use `assets/Unknown.png`.

## Assets

In `assets/`: `heroes/*.png` (200×200 round tokens), `players/*.png`, `maps/*.png`, `sets/*.png`
(two real box covers: Cobble & Fog, Battle of Legends III), `Melee.png`, `Ranged.png`, `Vs.png`,
`Unknown.png`, `Unmatched-logo.png` (used inverted).
Large 3:4 hero key art does not exist yet — the prototypes use placeholders.
Icons: Bootstrap Icons 1.11.3 (already used by the current `NavMenu.razor`).

## Do / don't

**Do** — show hero and map art as tiles or tokens; keep one gradient action per screen; use mono for
anything numeric or temporal (dates always with the year); let the game mode reshape the row or form;
keep villains and minions avatar-less with minions indented.

**Don't** — reintroduce `#e63946` or invent hues outside the palette; record items used / actions made /
time spent; colour match-log rows win/loss; use `Unmatched.otf`; show a side column for 1v1 or FFA.

## Files in this bundle

- `Unmatched Design System.dc.html` — tokens and component library (start here)
- `Unmatched Redesign.dc.html` — all screens. **Turn 9 at the top supersedes earlier turns**
  for Heroes (`9a`), Hero page (`9b`) and Add match (`9c`–`9g`); `8c` supersedes the collection
  shelf, `8h` defines the new control sizes and filter dropdowns. `3b`, `6a`, `7a` stand as before.
- `screens/` — PNG reference shots:
  `00-design-system.png`, `1c-heroes-ladder.png`, `2a-hero-detail.png`, `3b-match-row.png`,
  `4b-add-match.png`, `5a-collection.png`, `6a-match-log.png`, `7a-player-profile.png`,
  `8c-collection-big-tiles.png`, `8h-controls-filters.png`, `9a-heroes-list-tiles.png`,
  `9b-hero-page.png`, `9c-add-match-2v2.png`, `9d-add-match-1v1.png`, `9e-add-match-ffa.png`,
  `9f-add-match-coop.png`, `9g-map-fullscreen.png`
- `IMPLEMENTATION-PROMPT.md` — the change order for the current build: what to rework, what to add
- `support.js` — runtime the prototypes need to render locally; **not** for the target codebase
- `assets/` — every image referenced above

## Menus and settings

**Dropdowns are custom panels, never the browser's native popup.** Surface `#14141c`, 1px
`rgba(255,255,255,.1)`, radius 11px, shadow `0 18px 44px rgba(0,0,0,.6)`, 6px padding, 6px below the
trigger. Items are 32px rows, radius 8px, 500 12px Barlow, optional 20px token on the left and a
count on the right; selected = `rgba(139,125,255,.16)` + `#c4bcff` + check icon; hover =
`rgba(255,255,255,.06)`. Lists longer than ~8 entries get a filter field at the top and scroll at
212px. The trigger keeps its focused look while open (accent border, chevron flipped).

**Settings screen** uses the settings row: label + hint on the left, control on the right, hairline
between rows. It owns at least *Matches per page* (10 / 20 / 50 / 100 — the default for the match-log
footer), *Row density*, *Date format*. See section 07 of the design system.

## Still missing (design side)

Players **list** · hover preview in the ladder right column · Maps / Minions / Villains / Titles /
Tournaments / Settings screens · mobile adaptation (deliberately postponed).
Build these — and every other undesigned screen — from the design-system parts as described under
**Fidelity** above.
