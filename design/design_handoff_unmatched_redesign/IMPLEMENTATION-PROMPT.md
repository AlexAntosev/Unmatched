# Implementation order — Unmatched tracker, revision 3

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

## 5. Add match — two entry points, one sheet component (`14a`, `12a`, `12b`)

**Add match no longer opens a form directly.** The button opens a small modal, `New match`, with two
cards: **Manual entry** (the game is played — go straight to the sheet) and **Draft session** (not
played yet — ban and pick first, the sheet fills itself). Footer of the modal: `Draft pools come from
owned sets only` + Cancel. The word "quiz" is not used anywhere; the flow is a **draft**.

Both entries end on the **same sheet component** — build it once (`MatchSheet`) and mount it in three
places: Add match, Edit match (header `Edit match`, footer `Save changes` + `Delete match`), and
step 5 of the draft (with `prefilled` on and its own footer suppressed, because the draft has one).

### 5.1 Map banner (`14a`, unchanged from `9c`)
Full-width strip under the header: the map image as a **backdrop at opacity .3** plus
`linear-gradient(90deg,rgba(10,10,16,.92),rgba(10,10,16,.6))`; on top a 58px map thumbnail with a
fullscreen glyph overlay, the caption `BATTLEFIELD`, the map name in 700 18px Chakra Petch uppercase,
the violet dice **randomize** button and a chevron for the picker; `Shuffle turn order` at the right
end (hidden in co-op, see 5.4). In the draft-prefilled sheet the banner also carries a `DRAFTED` badge.

**Clicking the thumbnail opens the map full screen** (`9g`): dark overlay, image `object-fit:contain`
with 12px radius and a soft shadow, map name + meta top-left, `Randomize` + close (`×`) top-right, and
a filmstrip of other maps at the bottom (active one ringed in `#8b7dff`). Esc and a backdrop click
close it; picking a strip thumbnail changes the match's map.

### 5.2 Participant card — vertical, 180px (`14a`, replaces the horizontal plate of `9c`–`9f`)
The plate is a **column**, not a row, so cards stand side by side and every control is the same width:

1. **Hero art** — full card width, 118px tall, radius 12, 1.5px border in the result colour.
   - a **drag handle strip** across the top of the art: 22px tall, `bi-grip-horizontal` centred on a
     `linear-gradient(180deg,rgba(8,8,12,.8),transparent)`, `cursor:grab`. **This strip is the only
     place a card can be grabbed** — the rest of the card stays clickable.
   - **turn badge** bottom-left, 24px, radius 8, `#6c5cff`, white 12px Chakra Petch.
   - **place medal** (FFA only) bottom-right: `1ST` gold / `2ND` silver / `3RD` bronze / `4TH` grey on
     `rgba(10,10,16,.9)` with a 1.5px border in the medal colour; **clicking it cycles the place**.
   - trash (FFA, >3 players) top-right on the art.
   - losers: `grayscale(.55)`, a `rgba(242,80,107,.2)` veil and a 44px red `bi-x-lg`.
2. **Hero dropdown**, 34px, with the violet dice (randomize) — directly under the art.
3. **Player dropdown**, 32px, avatar 19px + name (absent on the villain card).
4. **HP LEFT / CARDS / SIDEKICK** — one full-width **stepper** each, 31px:
   label on the left, then `[−] n [+]` (20×24 hit areas). A field that does not apply shows `—` and no
   buttons (villain: cards and sidekick; heroes without a sidekick). Zero HP on a loser paints the
   whole stepper red (`rgba(242,80,107,.1)` / border `rgba(242,80,107,.3)` / `#f2506b`).

Cards wrap **centred** inside their side box, gap 11px. `box-sizing:border-box` — the card is 180px
including padding and border, otherwise two cards no longer fit one side at 940px.

**Minions** (co-op) are the same card at **132px**, in a column to the **right of the villain card**:
art 78px tall with a `MINION` badge top-left and trash top-right, name dropdown, then `COUNT` and `HP`
steppers. Under the last one sits a dashed **Add minion** card of the same width.

### 5.3 Sides
A side is a box (`Fighter`/`Opponent` · `Team 1`/`Team 2` · `Players` · `Heroes`/`Villain`), tinted by
result — green `rgba(52,217,155,.07)` / border `rgba(52,217,155,.4)` for the winner, red for the loser,
blue for the villain, neutral in FFA — with `Vs.png` (28px, `invert(1)`) in a 32px gutter between them.
The **winner is marked on the side header** in every mode except FFA (`Mark winner` outlined →
filled `#34d99b` chip); in FFA the medals on the cards decide it and no winner control is shown.
Marking a winner flips every loser card at once (5.2), and editing a loser's HP by hand clears the
auto-zero for that fighter only.

### 5.4 Turn order — drag only, by the handle
- **Co-op has no turn order at all**: no badges, no `Shuffle turn order`, and the header line reads
  `No turn order in co-op`. Do not write `Turn` for co-op fighters.
- Everywhere else the order is changed **only by dragging a card by the handle strip** (5.2) — the
  card body is not draggable, so steppers and dropdowns keep working. Dragged card at .45 opacity,
  drop indicator = 2px `#8b7dff` between cards.
- Renumbering after a drop:
  - **1v1 / FFA** — plain sequence, `1-2-3-4` left to right.
  - **2v2** — teams alternate: Team 1 gets `1` and `3`, Team 2 gets `2` and `4`. Dragging inside a team
    swaps that team's two numbers; a card never changes team by dragging.
- `Shuffle turn order` in the map banner re-rolls the same sequence (numbers only).
- The header line shows the result as text: `Order: Andrii → Vados → Ksuha → Zheka`.
- Persist into the existing `Turn` field on the fighter.

### 5.5 Details row and footer
Under the participants, one row of three: **Match type** — a segmented `Ranked | Casual` (Ranked =
filled `#34d99b`, `bi-graph-up-arrow`; Casual neutral) — **Date** (always with the year) and **Epic**
(three bare 20px stars). Below it a full-width **Comment** field.

**There is no Tournament or Stage field on this screen, in any mode.** A match is tied to a tournament
and a stage only from the tournament bracket, server-side; the sheet carries the hint
`Tournament and stage come from the bracket — never set on this screen.` Ranked/Casual is the only
thing the player chooses here that affects rating.

Footer: result summary (`Team 1 won · counts for the ladder` / `… casual, no rating change`) +
`Save & add another` + `Save match`. **Suppressed when the sheet is embedded in the draft.**
Never collect items used, actions made or time spent.

### 5.6 Mode differences in one place
| | 1v1 | 2v2 | FFA | Co-op |
|---|---|---|---|---|
| Sides | Fighter / Opponent | Team 1 / Team 2 | one `Players` box | Heroes / Villain |
| Cards per row | 1 per side | 2 per side | 4 in one box | 2 heroes + villain (+ minion column) |
| Winner | side header | side header | 1st-place medal | side header |
| Turn badges | 1-2 | 1,3 / 2,4 | 1-2-3-4 | none |
| Extra | — | — | Place medal, min 3 / max 4 players | villain has no player, cards, sidekick or badge; `Add minion` |

---

## 5A. Draft session (`12b`) — five steps

A full-screen flow, not a modal: header (`Draft session` + mode chip + `Restart` + `Step n of 5`),
a progress rail, the step body, and a **footer that always says who acts now** (avatar + line, e.g.
`Ksuha bans a map`) with `Back` / `Next`. `Next` is disabled until the step is complete; the rail
allows going back only to steps already passed. Every random pool has a **Reroll pool** button.

1. **Players** — mode cards (1v1 / 2v2 / FFA / Co-op) + a grid of player cards. For 2v2 each selected
   card shows a `T1`/`T2` badge that toggles on click, plus **Split teams randomly**. Counts are
   enforced per mode (1v1 = 2, 2v2 = 4, FFA = 3–4, co-op = 2–4).
2. **Map** — a pool of **N + 1** random maps from the collection. Every player bans exactly one, in a
   random order shown as a `Ban order` ribbon (current player highlighted, done players ticked).
   A banned map is crossed out, greyed and stamped with the banner's avatar; the survivor gets a
   `CHOSEN MAP` badge and `Next` unlocks.
3. **Heroes** — a pool of **N × 3** random heroes. First one ban each in a random order, then a
   **snake pick**: forward through that order, then back, so everyone ends with two heroes. A picked
   hero shows its owner's avatar; a banned one a `BAN` tag and the banner's avatar.
   **3.1 Final pick** — a block under the pool: each player has their two cards, and **clicking the
   one they keep marks it `FINAL` and automatically crosses the other as `CUT`**. Everyone decides in
   silence first; the flow does not enforce simultaneity.
4. **Turn order** — a `Roll turn order` button; 1v1 and FFA get a plain random order, 2v2 alternates
   teams (T1, T2, T1, T2). Result is a list of rows with the number, hero art, player and team badge.
   **For co-op this step is replaced by `Villain`**: pick one of three villains and any number of
   minions by checkbox, or **Random from collection**.
5. **Sheet** — the section-5 component with `prefilled`: a green `Prefilled from draft` banner, the
   map with the `DRAFTED` badge, a `Draft log` strip (the bans), fighters sorted by turn order, and
   every field still editable. Its own footer is hidden; the draft footer saves.

Open questions to settle before implementation: whether a finished draft can be saved as a
**pending match** (drafted now, result filled in after the game), and whether pools are limited to
owned sets or all printed content.

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
7. Add match opens the **New match** modal (Manual entry / Draft session); both paths end on the same
   sheet component, also reused by Edit match.
8. Sheet: map banner + fullscreen map, 180px vertical participant cards with steppers, winner on the
   side (medals in FFA), losers auto-zeroed and crossed out, Ranked/Casual instead of Tournament,
   drag-by-handle turn order (1-2-3 / team-alternating / none in co-op), all four modes.
9. Draft session: five steps, N+1 map bans, N×3 hero pool with snake pick and final pick, turn-order
   roll or villain step for co-op, prefilled sheet at the end.
8. No `#e63946`, no `Unmatched.otf`, no native `<select>`, no new hues, fonts or radii.
