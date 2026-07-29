# Implementation order — Unmatched tracker, revision 2

Use this together with `README.md` (tokens, screen specs) and the prototype
`Unmatched Redesign.dc.html`. The prototype is a **design reference**, not code to copy: port values
and structure into the existing Blazor Server app (`Unmatched.UI.BlazorServer`), using Razor
components and the project's own CSS. `support.js` never enters the codebase.

The first release is live. This document lists **only what must change or be added**, in the order it
should be done. Everything not mentioned stays exactly as built.

Reference ids point at badges in the prototype canvas (newest turn is at the top) and at PNGs in
`screens/`.

---

## 0. Global control sizing — do this first (`8h`, visible in every turn-9 screen)

Every control in the app grew one step. This is a token change, so it lands before the screen work.

| Control | Was | Now |
|---|---|---|
| Form field, dropdown trigger, primary/secondary button | 32px h, 12px text | **36px h, 13–13.5px** Barlow 500 |
| Nav rail item | 38px | **40px**, label 13.5px |
| Segmented control item | 28px | **30–32px**, 12.5px text, 3px track padding |
| Dropdown panel row | 32px | **38px**, 13px text, 26px token, radius 9px |
| Filter chip (applied value) | 26px | **32px**, 12.5px text, 14px `×` |
| Pagination button | 30px | **36px** (square) / 36px h for Prev / Next |
| Icon-only button | 24px | **28px** |
| Epic stars (editable) | 17px | **19–20px** |
| Radius | — | fields/rows 9–10px, cards 12–14px (unchanged elsewhere) |

Nothing else about the visual language changes: same palette, same fonts, same 8px rhythm, one
gradient action per screen.

---

## 1. Grids: bigger identity + List/Tiles switch + per-page footer (`9a`, `9a-heroes-list-tiles.png`)

Applies to **every** list screen (Heroes, Players, Maps, Minions, Villains, Titles, Tournaments).

**List view (default).**
- Row grid `44px 1fr 78px 70px 118px 70px`, gap 14px, padding 11px 16px, radius 12px.
- Hero token **46×46, radius 13**, 1px `rgba(255,255,255,.14)` border, `#14141c` behind it (art is a
  round token on transparent).
- Name **600 15px Barlow**; second line = set name, 400 10.5px IBM Plex Mono `#6b6c7e`.
- Melee/ranged icon 15px, `filter:invert(1)`, opacity .55, pushed to the right of the name cell.
- Rank 700 17px Chakra Petch (gold/silver/bronze for the top three), points 700 17px, mono values 13px.
- Left edge `3px solid` result colour + the existing 4–7% wash — unchanged.

**Tiles view.**
- `repeat(5, 1fr)`, gap 14px; tile padding 16px 14px 14px, radius 14px, background `#101018`
  (top three: `linear-gradient(170deg,rgba(108,92,255,.12),#101018 62%)`).
- Round art **88px** with a 2px ring (`rgba(139,125,255,.4)` for the top three, else
  `rgba(255,255,255,.1)`); rank badge top-left, attack-type icon top-right.
- Then name 14.5px, set in mono, points, and the W/L bar.

**Switch**: segmented `List | Tiles` (icons `bi-list-ul` / `bi-grid-3x3-gap-fill`) at the right end of
the filter row. Persist the choice per screen (user setting or localStorage) — it is a preference, not
a session flag.

**Per-page footer — required on every grid, including the small ones inside pages**
(hero-page match log, collection shelf, player screens):
caption `ROWS PER PAGE` / `TILES PER PAGE` (10px mono, uppercase) · segmented **10 / 20 / 50 / 100**
(tiles: 10 / 20 / 50 / All; collection: 10 / 20 / All) · range label `1–20 of 42` · pagination on the
right. Default comes from Settings → *Matches per page*; the footer overrides it for the session.

---

## 2. Filters: real multi-select (`8h`, `8h-controls-filters.png`)

Every filter dropdown (players, heroes, maps, tournaments, sets, attack type, date range) becomes
multi-select. The native `<select>` is not used anywhere.

- **Trigger**: 40px h, icon + label + selection-count badge (`#6c5cff`, white, min 20px, radius 6px)
  + chevron. Zero selected → label in `#8b8c9e`, no badge.
- **Panel**: `#14141c`, 1px `rgba(139,125,255,.4)` while open, radius 12px, shadow
  `0 18px 44px rgba(0,0,0,.6)`, 8px padding, 6px below the trigger.
- **Rows**: 38px, radius 9px, 19px checkbox (checked = `#6c5cff` + white `bi-check-lg`; unchecked =
  1.5px `rgba(255,255,255,.18)`), optional 26px token, label 13px, count on the right.
  Selected row background `rgba(108,92,255,.12)`; hover `rgba(255,255,255,.05)`.
- Lists over ~8 entries: 34px search field pinned at the top, scroll at ~212px.
- **Footer inside the panel**: `Clear (n)` on the left (turns `#f2506b` on hover) and a filled
  **Apply** button on the right. Selections apply on Apply, not per click.
- **Applied values render as chips** under / beside the triggers: 32px, `rgba(108,92,255,.14)`,
  border `rgba(139,125,255,.4)`, text `#c4bcff`, trailing `×` that removes just that value.
- **`Clear all filters`** — outlined red-ish chip at the end of the chip row, only when ≥1 filter is
  active; resets every dropdown and the mode segmented filter.

---

## 3. Hero page rebuild (`9b`, `9b-hero-page.png`)

Keep the `2a` dossier order (banner → 4 stat cards → analysis row → rating line → antagonist +
matchups → match log → against villains / minions). Changes:

**3.1 Named prev / next bar** — new sticky sub-bar above the banner, `#0c0c14`, 12px 26px, hairline
bottom. Left button: chevron + **34px token** + two lines (`PREVIOUS HERO` 8.5px mono / name 13.5px
Barlow 600). Centre: breadcrumb plus `Hero 01 of 42 · sorted by points` in 10.5px mono. Right button
mirrored (name, token, chevron). Buttons follow the **current list order and filters**, wrap around at
the ends, and keyboard `←` / `→` moves between entities.
**Add the same bar to Player, Map, Villain and Minion detail pages.**

**3.2 Play style becomes editable, with the radar beside it.** The analysis row is now
`1.15fr 210px 200px`: play-style card · radar card · win-rate donut (+ title chips beneath it).
- Each axis (Attack / Defence / Trickery / Difficulty) is a row: label 13px + **three 26px squares**,
  radius 8px, gap 7px. Filled = `#a855f7` with a `rgba(196,188,255,.55)` border; empty =
  `rgba(255,255,255,.07)` with `rgba(255,255,255,.12)`.
- Clicking square *n* sets the value to *n*; clicking the highest lit square clears it to 0.
  No ± stepper, no number — the squares *are* the control. Hint `click a square to set` in 9.5px mono.
- Persist on change (optimistic UI, no dialog, no Save button); values are 0–3 ints on the hero.
- The radar polygon must be **scaled to its own grid**: radius `4 + 42·v/3` on a 120-unit box whose
  outer ring is at 46 — v=3 lands on the ring, never outside it. Caption under the radar:
  `A 3 · D 2 · T 1 · Dif 2`.

**3.3 Sidekick without artwork.** There is no sidekick art in the data, so drop the image entirely:
a single row inside the play-style card — `SIDEKICK` label on the left, `Roach` + `7 HP` on the right
(`—` when the hero has none). Same rule anywhere else a sidekick appears.

**3.4 Match log inside the page** gets the per-page footer from §1.

---

## 4. Collection (`8c`, `8c-collection-big-tiles.png`)

- Shelf becomes **3 columns**; tile padding 13px, radius 14px.
- Cover **full tile width, 200px tall**, radius 10px; real art where it exists, otherwise the dashed
  placeholder with the inverted logo and the mono caption `box art`.
- Owned checkbox stays in the **cover's top-right corner**, now **24px**, radius 7px, with
  `0 2px 8px rgba(0,0,0,.6)`; clicking it must not change the selected set.
- Below the cover: name 14px, `year · STATUS` line (`OWNED` green / `ADD` violet / `UNSAVED` gold),
  a row of **40px hero tokens** (radius 11px, `+N` tile when they overflow) and **46px map thumbs**.
- Unowned tiles: dashed border, cover at opacity .6, names in `#8b8c9e`.
- **Right panel is sticky** (`position:sticky; top:16px`) so it stays visible while the shelf scrolls;
  it keeps its content (cover, name, own/unown CTA, heroes, maps, villain + minions) and gains a
  `pinned while scrolling` caption / pin icon in its header. Width 300–326px.
- Shelf footer gets the per-page control (10 / 20 / All).

---

## 5. Add match — rebuilt sheet, all four modes (`9c`–`9g`)

One screen, no stepper. **Add and Edit use the same component**; in edit mode the header reads
`Edit match` and the footer shows `Save changes` + `Delete match`.

### 5.1 Map banner (`9c`, replaces the right-column map preview)
Full-width strip under the header: the map image as a **backdrop at opacity .3** plus
`linear-gradient(90deg,rgba(10,10,16,.92),rgba(10,10,16,.6))`; on top a 58px map thumbnail with a
fullscreen glyph overlay, the caption `BATTLEFIELD`, the map name in 700 18px Chakra Petch uppercase,
the violet dice **randomize** button and a chevron for the picker; `Shuffle turn order` at the right end.

**Clicking the thumbnail opens the map full screen** (`9g`, `9g-map-fullscreen.png`): dark overlay,
image `object-fit:contain` with 12px radius and a soft shadow, map name + meta top-left,
`Randomize` + close (`×`) top-right, and a filmstrip of other maps at the bottom (active one ringed
in `#8b7dff`). Esc and a backdrop click close it; picking a strip thumbnail changes the match's map.

### 5.2 Fighter plates (from `8e`, now everywhere)
A side is a box (`Team 1` / `Team 2` / `Heroes` / `Villain`, colour-coded as in the README) holding
plates. One plate = drag handle · **64px hero art** (radius 14) · a 2×3 field block:
`Player` + `Hero` (with the violet dice) on the first line, `HP` / `CARDS` / `SIDEKICK` on the second.
Fields are 34px / 32px tall. This is what removes the empty space of the old horizontal rows: a 2v2
sheet now fits in ~700px with details and comment visible.

### 5.3 Turn order (`9c`, and every mode)
- Every plate carries an **order badge** — 23px, radius 7px, `#6c5cff`, white 11.5px Chakra Petch —
  pinned to the art's top-left corner (FFA: top-left badge for order, bottom-right badge for place).
- **Drag a plate to reorder**: HTML5 drag or pointer-based sorting, drop indicator = 2px `#8b7dff`
  line between plates. Dragging across sides moves the fighter to that side **and** re-numbers.
  Handle = `bi-grip-vertical`, `cursor:grab` / `grabbing`.
- **Shuffle turn order** randomises the numbers of all fighters (and only the numbers).
- The header line shows the resulting order as text: `Order: Geralt → Arthur → Medusa → Bigfoot`.
- Store into the existing `Turn` field on the fighter.

### 5.4 Winner on the fighter / team, losers auto-zero (`9c`, `9d`, `9f`)
- 1v1 and FFA: the winner control lives **on the fighter plate**; 2v2 and co-op: **on the side header**.
- Unset state = outlined `Mark winner` (`bi-trophy`, `#6b6c7e`); set state = filled chip
  (`#34d99b`, text `#062a1c`, `bi-trophy-fill`, uppercase Chakra Petch). Only one winner at a time —
  choosing another moves it.
- On win selection **every loser plate flips at once**: HP field forced to `0` and painted
  `rgba(242,80,107,.1)` / border `rgba(242,80,107,.3)` / text `#f2506b`; sidekick HP → 0 where present;
  the hero art goes `grayscale(.55)`, gets a `rgba(242,80,107,.22)` veil, a red `bi-x-lg` cross
  (30px on 64px art, 44px on the 96px 1v1 art) and a `rgba(242,80,107,.45)` border.
  Editing a loser's HP by hand is still allowed and clears the auto-zero for that fighter only.
- Footer summary states it plainly: `Team 1 won · losers set to 0 HP`.

### 5.5 Modes
- **1v1** (`9d`): two plates joined by `Vs.png`, art **96px**, fields stacked; drag swaps who is first.
- **2v2** (`9c`): two side boxes, two plates each, `VS` between them.
- **FFA** (`9e`): one plate per fighter, full width, art 72px; grid
  `1.1fr 1.1fr 78px 78px 96px` = Player · Hero · HP · Cards · **Place** (medal dropdown).
  Order badge top-left, place badge (`1ST` gold / `2ND` silver / `3RD` bronze / `4TH` grey)
  bottom-right on the art. Min 3, max 4 players: `Add player` disappears at 4, the trash returns to 3.
  Drag order and place are independent — a first-turn player can finish 3rd.
- **Co-op** (`9f`): `Heroes` side (green) vs `Villain` side (`#38bdf8`). The villain plate has
  **no player avatar, no sidekick, no cards**, uses `assets/Unknown.png`, and carries `Add minion`.
  A minion row is indented 16px: 30px art, name, `×N on the board`, `HP` field, trash. Villain acts
  last (fixed), so its plate has no order badge.
- Details below the plates in one row — `Tournament`, `Date` (always with the year), `Epic`
  (three bare 20px stars) — then a full-width `Comment` field. Footer: state summary +
  `Save & add another` + gradient `Save match`.
- Never collect items used, actions made or time spent.

---

## 6. Backend / data touchpoints

- `Turn` per fighter — written by the drag/shuffle order (already in `UiFighterDto`).
- Hero play-style values (Attack / Defence / Trickery / Difficulty, int 0–3) must be **writable** from
  the hero page.
- Per-user UI preferences to persist: nav collapsed, list view (list/tiles) per screen, per-page size,
  active filters per screen (session is enough for filters).
- Prev/next needs the current ordered, filtered id list on the server side (or a cached query) so the
  neighbours match what the user saw in the list.

---

## 7. Definition of done

1. No control smaller than the §0 table anywhere in the app.
2. Every grid has a working per-page control and paging label.
3. Every list has a List/Tiles switch, and hero/player art is legible in both.
4. Every filter dropdown is multi-select, with chips, per-value removal and Clear all.
5. Hero page: named prev/next, editable play-style squares with a live radar inside its grid,
   sidekick as text.
6. Collection: 3-up tiles with 200px covers, sticky detail panel.
7. Add match / Edit match: map banner + fullscreen map, draggable plates with turn numbers,
   Shuffle working, winner on fighter/team, losers auto-zeroed and crossed out, all four modes.
8. No `#e63946`, no `Unmatched.otf`, no native `<select>`, no new hues, fonts or radii.
