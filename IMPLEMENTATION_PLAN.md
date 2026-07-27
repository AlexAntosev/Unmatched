# Редизайн Unmatched.UI.BlazorServer — план реалізації

## Контекст

`Unmatched.UI.BlazorServer` — робочий фронт застосунку (.NET 9, Blazor Server), побудований на
Syncfusion-гридах, Bootstrap-картках і червоній темі `#e63946`. Хендоф
`Unmatched дизайн система/design_handoff_unmatched_redesign/` замінює його цілком: нова темна
палітра (фіолетовий акцент `#6c5cff`), три нові шрифти, колапсибельний нав-рейл, щільні рядки
замість гридів, і шість надизайнених екранів (`1c`, `2a`, `3b`, `4b`, `5a`, `6a`).

Застосунок **не per-user** — один власник логує чужі ігри, тому жоден екран не пишеться з точки зору
одного гравця. Мобільна адаптація свідомо поза скоупом. `Unmatched.UI.Angular` цей план не зачіпає.

Частина того, що є на макетах, бекендом не покрита: вміст доповнення (герої/мапи/віллени/мініони в
сеті), обкладинки коробок, статистика в розрізі сету, тайтли героя (зашито заглушкою `//TODO`),
метрики матчлогу. Розділ 2 розписує кожен такий розрив.

---

## Ухвалені рішення

| # | Питання | Рішення |
|---|---|---|
| 1 | Key art 3:4 у банері 2a | Перевикористовуємо наявний круглий токен `Hero.ImageFileName`. Нових полів і міграцій немає |
| 2 | Час під датою в рядку матчу | Додаємо time-input у Add match; у рядку час рендериться **лише коли ≠ 00:00**. Міграції не треба — `Matches.Date` уже `DateTime` |
| 3 | Syncfusion | Прибираємо **повністю** — пакет, реєстрація, `_Imports`, `_Host`, `syncfusion-overrides.css` |
| 4 | `/admin` та `/initialize` | Один пункт `bi-gear` «Settings» біля кнопки Collapse → сторінка з обома діями. Нав-рейл лишається на 10 пунктах макета |
| 5 | Обкладинки коробок | Нове поле `Expansion.ImageFileName` + міграція + категорія `expansions` у MinIO + аплоад через `EditableImage` |
| 6 | Кількість вмісту сету | Рахуємо рядки за `ExpansionId` (герої/мапи/віллени/мініони). Поля `CountInBox` не додаємо, `×N` біля мініона не показуємо |
| 7 | KPI матчлогу | `sessions` = кількість різних дат у поточному місяці. `Longest streak` = найдовша серія перемог **героя** (та сама семантика, що в наявному `StreakTitleHandler`), підпис — ім'я героя + місяць |
| 8 | Кнопка Export | Присутня візуально, `disabled`, без дії |
| 9 | `Turn` у Add match | Лишається як компактний контрол у боксі бійця (стилізований під поля 4b). `ItemsUsed`/`ActionsMade`/`TimeSpentInSeconds` більше не збираємо |
| 10 | Редагування Epic | У згорнутому рядку — read-only. Клікабельні зірки — в розгорнутому скорборді |
| 11 | Фільтрація матчлогу | В пам'яті, як зараз (`GetMatchLogAsync()` + LINQ у компоненті). Match API не змінюється |
| 12 | Player detail | `Favorites` → рядки в стилі 1c; кругова діаграма → donut win-rate дизайн-системи |
| 13 | Шрифти | Google Fonts CDN у `_Host.cshtml` |
| 14 | Лічильники нав-рейла | Новий scoped `NavCountsService` із кешем на циркуїт, інвалідація після збереження матчу/колекції |
| 15 | Статистика сету (PLAYED / BEST HERO / WIN RATE) | Новий ендпойнт у Statistics-сервісі |

---

## 1. Інвентаризація

### Надизайнені екрани (робимо один-в-один)

| Екран | Наявні файли | Що робимо |
|---|---|---|
| **1c** Heroes ladder | `Pages/Statistics/Hero/HeroesStatistics.razor` (SfGrid + inline `<style>` із `Colors.*`) | Переписати повністю: щільні рядки, таби Points/Win rate/K-D/Recently played, права колонка (spotlight + matchups + collection), пагінація. Логіка `FilterByCollectionAsync` зберігається |
| **2a** Hero detail | `Pages/Statistics/Hero/HeroStatistics.razor`, `HeroSummary.razor` (+ `.css`), `Shared/Statistics/EntityDetailHeader.razor`, `StatCard.razor`, `Pages/StatisticsRecord.razor`, `TotalMatchesRecord.razor`, `Pages/Statistics/MonsterMatchupTable.razor` | Новий лейаут-досьє. `EntityDetailHeader`/`StatCard`/`StatisticsRecord`/`TotalMatchesRecord` **видаляються** (їхню роль беруть `HeroBanner`, `StatTile`, `AntagonistCard`). `MonsterMatchupTable` + `MonsterMatchupRow` переписуються під сітку `1fr 40px 40px 40px 52px`. Обчислення антагоніста/матчапів виносяться з `.razor` у сервіс |
| **3b** Рядок матчу | `Shared/MatchLog/MatchLogGrid.razor` (+ `.css`), `MatchLogRow.cs` | **Видаляються.** Нові `MatchRow.razor`, `MatchScoreboard.razor`, `MatchList.razor` |
| **4b** Add match | `Pages/AddMatch/AddMatch.razor` (+ `.css`), `FighterInfo.razor`, `VillainInfo.razor`, `MatchSavedPopup.razor` | Переписати розмітку в одну сторінку (сегментований перемикач + side-бокси + права колонка 280px + футер). Уся `@code`-логіка (`GameModeChanged`, `EnsureFighterCount`, `AssignTeams`, `SetTeamWinner`, `SetPlayersWon`, `ValidateMatch`, `GenerateDefaultMatch`, `FilterByCollectionAsync`, рандомайзер через `NotificationService`) **зберігається**, змінюється лише UI-шар |
| **5a** Collection | `Pages/Collection/Collection.razor` (+ `.css`) | Переписати: KPI-стрічка, полиця карток 2 колонки, права панель 396px, draft-стан + `Save collection`. `CollectionService.Get/SetOwnedExpansionIdsAsync` зберігається |
| **6a** Match log | `Pages/MatchLog/MatchLog.razor` | Переписати: шапка, 4 KPI, сегментований фільтр режимів + чипи-дропдауни + діапазон дат, пошук, панель рядків, пагінація. Query-параметр `?mode=` з нав-меню зникає (фільтр тепер на сторінці) |

### Ненадизайнені екрани (збираємо з компонентів за патернами 1c/2a/3b)

| Екран | Наявні файли | Що робимо |
|---|---|---|
| Players list | `Pages/Statistics/Player/PlayersStatistics.razor` | Ладдер 1c (аватар, ім'я, matches, W/L, K/D) |
| Player detail | `PlayerStatistics.razor`, `PlayerSummary.razor` (+ `.css`), `Favorites.razor`, `HeroesMatchesPieChart.razor` | Досьє 2a. `Favorites` → рядки 1c із медаллю для Chosen one. `HeroesMatchesPieChart` **видаляється**, замість неї donut win-rate |
| Maps list / detail | `MapsStatistics.razor`, `MapStatistics.razor`, `MapSummary.razor` | Ладдер 1c із мініатюрою мапи / досьє 2a |
| Minions list / detail | `MinionsStatistics.razor`, `MinionStatistics.razor`, `MinionSummary.razor` | Те саме, токен `info` `#38bdf8` |
| Villains list / detail | `VillainsStatistics.razor`, `VillainStatistics.razor`, `VillainSummary.razor` | Те саме, токен `villain` `#f0b429` |
| Titles | `Pages/Titles/Titles.razor` (SfGrid + командні колонки + модалка з мультиселектом) | Панель рядків + діалог призначення героїв на власних чекбоксах |
| Tournaments | `Pages/Tournaments/Tournaments.razor`, `Tournament.razor`, `TournamentRecord.razor`, `TournamentStage.razor` | Список 1c / деталь 2a; матчі всередині — рядок `3b`; кнопки генерації стадій — `UmButton` |
| Settings | `Pages/Admin/Admin.razor`, `Pages/Initialize/Initialize.razor` | Об'єднати в `Pages/Settings/Settings.razor` (перерахунок рейтингів + ініціалізація даних), доступ із шестерні нав-рейла |
| Landing | `Pages/Main/Index.razor` (лого по центру) | `/` редіректить на Heroes ladder — README називає 1c landing screen |
| Оболонка | `Shared/MainLayout.razor` (+ `.css`), `NavMenu.razor` (+ `.css`), `NavSubMenu.razor` (+ `.css`) | `NavMenu`/`NavSubMenu` **видаляються** → `NavRail.razor`. `MainLayout` переписується |
| Спільне | `Shared/RatingRecalculationWarning.razor`, `Shared/EditableImage.razor`, `NotificationService.cs`, `Services/ImageUploadService.cs`, `Urls.cs` | Зберігаються; `RatingRecalculationWarning` і `EditableImage` перестилюються під токени |
| `Pages/Epic.razor` | зірки ★/☆ інлайн-стилями | Замінюється на `EpicStars.razor` (read-only + editable) |

---

## 2. Розрив «дизайн ↔ бекенд»

| Потреба макета | Що є зараз | Чого бракує |
|---|---|---|
| 5a: вміст сету — герої (deck, type, HP), мапи, віллен, мініони | `Expansion` має навігації `Heroes/Maps/Villains/Minions`, але `ExpansionRepository.GetAsync()` інклюдить лише `Heroes`+`Maps`, а `ExpansionDto` віддає `HeroNames: string[]` | Include вілленів/мініонів + повноцінні вкладені DTO |
| 5a: обкладинка коробки | нічого | `Expansion.ImageFileName` + міграція + аплоад |
| 5a: PLAYED / BEST HERO / WIN RATE по сету | нічого | Ендпойнт у Statistics |
| 5a: KPI Sets/Heroes/Maps/Villains·Minions (owned/total) | `OwnedExpansion` + каталог | Рахується в UI з (1) і `GetOwnedExpansionIdsAsync()` |
| 2a: тайтли героя із коментарем | `HeroTitleEntity`, `TitleEntity`, `TitleController`; але `HeroStatisticsService` віддає `new List<TitleDto>() //TODO` | `GET /title/hero/{heroId}` + прошивка клієнта |
| 2a: рейтингова лінія 120→208 | `GetRatingChangesAsync` уже віддає **накопичений** рейтинг (поле названо `RatingDelta`, але це running total) | Нічого. Потрібен лише SVG-рендер замість `BlazorBootstrap.LineChart` |
| 2a/1c: радар play style 0–3 | `PlayStyle` (Attack/Defence/Trickery/Difficulty) є | Нічого |
| 2a/1c: donut win rate, matchups, антагоніст | обчислюється інлайн у `HeroStatistics.razor` | Винести в сервіс, додати win% по опоненту |
| 6a: KPI Matches / This month / Avg. epic / Longest streak | `MatchLogDto` має `Date`, `Epic`, `Fighters` | Обчислення в UI-шарі (рішення #7, #11) |
| 6a: фільтри player/hero/map/tournament/date + пошук + пагінація | `GetMatchLogAsync()` тягне все | Обчислення в UI-шарі (рішення #11) |
| 3b: час матчу | `Matches.Date` = `DateTime`, але Add match пише через `InputDate` → 00:00 | time-input у формі (рішення #2) |
| 3b: арт вілленів/мініонів | `Villain.ImageFileName`, `Minion.ImageFileName` є | Фолбек `/Unknown.png` уже вбудований у `*Dto.ImageUrl` |
| Нав-рейл: лічильники | розкидані по 4 мікросервісах | `NavCountsService` (рішення #14) |

### A. Catalog — вміст сету та обкладинка

**Сутності / міграція**

- `Services/Catalog/Unmatched.CatalogService.Domain/Entities/Expansion.cs` — додати `public string? ImageFileName { get; set; }`
- нова міграція `AddExpansionImageFileName` у `Unmatched.CatalogService.EntityFramework/Migrations/`

**Репозиторій**

- `EntityFramework/Repositories/ExpansionRepository.cs` — `.Include(e => e.Villains).Include(e => e.Minions)`

**Контракти** (`Unmatched.CatalogService.Api/Dto/`)

```
ExpansionDto            + ImageFileName
                        − HeroNames: IEnumerable<string>
                        + Heroes:   IEnumerable<ExpansionHeroDto>
                        + Villains: IEnumerable<ExpansionVillainDto>
                        + Minions:  IEnumerable<ExpansionMinionDto>

ExpansionHeroDto     { Id, Name, Hp, DeckSize, IsRanged, ImageFileName }
ExpansionVillainDto  { Id, Name, Hp, DeckSize, IsRanged, ImageFileName }
ExpansionMinionDto   { Id, Name, Hp, DeckSize, IsRanged, ImageFileName }
```

- `Api/Mapping/ApiMapper.cs` — профілі для трьох нових DTO
- `Api/Controllers/ExpansionController.cs` — `PUT /expansion/{id}/image` (за зразком `HeroController` image-ендпойнта)
- `Domain/Services/ExpansionService.cs` + `IExpansionService` — `UpdateImageAsync(Guid id, string fileName)`

**Клієнтський шар** (`Unmatched/`)

- `Dtos/Catalog/CatalogExpansionDto.cs` — дзеркалить нову форму; нові `CatalogExpansionHeroDto`/`...VillainDto`/`...MinionDto`
- `Dtos/ExpansionDto.cs` — `ImageFileName`, `ImageUrl => "/images/expansions/{ImageFileName}"`, `Heroes`, `Villains`, `Minions`; computed `HeroCount`/`MapCount`/`VillainCount`/`MinionCount`
- `HttpClients/Contracts/ICatalogClient.cs` + `CatalogClient.cs` — `UpdateExpansionImageAsync`
- `Services/Contracts/IExpansionService.cs` + `ExpansionService.cs` — `UpdateImageAsync`
- `Mapping/UnmatchedMapper.cs` — нові профілі

**Асети** — дві обкладинки з хендофу (`assets/sets/cobble-and-fog.png`, `battle-of-legends-3.png`)
кладемо в `Unmatched.UI.BlazorServer/wwwroot/images/expansions/`; `SeedImagesFromWwwRootAsync` у
`Program.cs` заллє їх у MinIO автоматично.

**Тести** — `Unmatched.CatalogService.Tests`: репозиторій повертає вілленів/мініонів; маппінг;
`UpdateImageAsync` на неіснуючому Id.

### B. Statistics — статистика в розрізі доповнення

- `Domain/Communication/Catalog/Http/Dto/CatalogHeroDto.cs` — додати `Guid? ExpansionId` (Catalog API
  вже його віддає, поле просто не десеріалізується)
- новий `Domain/Services/ExpansionStatisticsService.cs` + `Contracts/IExpansionStatisticsService.cs`:
  групує `HeroStats` за `ExpansionId` героя і рахує `TotalMatches`, `TotalWins`, `TotalLooses`,
  `WinRate`, `BestHeroId`/`BestHeroName` (за `Points`, тайбрейк — win rate)
- новий `Domain/Models/ExpansionStats.cs`
- новий `Api/Controllers/ExpansionController.cs` — `GET /expansion/stats`
- новий `Api/Dto/ExpansionStatsDto.cs` + профіль у `Api/Mapping`
- реєстрація в `Domain/Registration/ServiceCollectionExtensions.cs`

Клієнт: `Unmatched/Dtos/Statistics/ExpansionStatisticsDto.cs`, `Dtos/UiExpansionStatisticsDto.cs`,
`IStatisticsClient.GetExpansionStatsAsync()`, `StatisticsClient`, `Services/Statistics/IExpansionStatisticsService.cs`.

**Тести** — `Unmatched.StatisticsService.Tests`: групування, герої без `ExpansionId`, порожній сет
(нуль матчів → win rate 0, best hero `null`).

### C. Match — тайтли героя та час матчу

- `Domain/Services/ITitleService.cs` + `TitleService.cs` — `GetByHeroAsync(Guid heroId)` через
  `IHeroTitleRepository`
- `Api/Controllers/TitleController.cs` — `GET /title/hero/{heroId}`
- `Unmatched/HttpClients/Contracts/IMatchClient.cs` + `MatchClient.cs` — `GetTitlesByHeroAsync`
- `Unmatched/Services/Statistics/HeroStatisticsService.cs` — прибрати `//TODO`, заповнити `uiModel.Titles`
- час матчу: змін у бекенді немає, лише time-input у `AddMatch.razor` (`_match.Date` комбінує дату й час)

**Тести** — `Unmatched.MatchService.Tests`: `GetByHeroAsync` для героя з кількома тайтлами й без них.

### D. UI-обчислення (без змін у мікросервісах)

Нові чисті класи в `Unmatched/Services/Statistics/`, покриті тестами в `Unmatched.MatchService.Tests`
або новому `Unmatched.Tests` (за наявності):

- `MatchLogMetrics` — `TotalMatches`, `AddedThisWeek`, `ThisMonth`, `SessionsThisMonth`
  (distinct `Date.Date`), `AverageEpic`, `LongestHeroStreak` (ім'я героя, довжина, місяць завершення)
- `HeroMatchupCalculator` — win% проти кожного опонента (для 1c spotlight і 2a Matchups), антагоніст
  (переїжджає з `HeroStatistics.razor`)
- `MatchLogFilter` — режим, гравець, герой, мапа, турнір, діапазон дат, текстовий пошук

---

## 3. Кроки 1 + 2 — спільний фундамент

### Токени і глобальні стилі

| Файл | Дія |
|---|---|
| `wwwroot/css/theme/variables.css` | Переписати повністю: `--um-bg-app/panel/nav/inset`, `--um-border`, `--um-border-strong`, `--um-text/-muted/-dim/-faint`, `--um-accent`, `--um-accent-2`, `--um-accent-soft`, `--um-accent-text`, `--um-win`, `--um-loss`, `--um-info`, `--um-gold/-silver/-bronze`, `--um-mode-2v2`, `--um-gradient-primary`, `--um-selected-*`, радіуси 6/9/11/13/16, висоти контролів 28/32/36, кроки спейсингу 8–26. Червона тема `#e63946` видаляється |
| `wwwroot/css/theme/typography.css` | Переписати: класи `.um-display`, `.um-page-title`, `.um-card-title`, `.um-section-caption`, `.um-body`, `.um-ui-label`, `.um-metric`, `.um-mono-meta`, `.um-mono-caption`. `@font-face` для `Unmatched.otf` видаляється |
| `wwwroot/css/theme/base.css` | **Новий**: reset, `body` на `--um-bg-app`, скролбари, focus-ring, утиліти `.um-panel`, `.um-inset`, `.um-hairline` |
| `wwwroot/css/theme/bootstrap-overrides.css` | Скоротити до мінімуму (модалки BlazorBootstrap, алерти) |
| `wwwroot/css/theme/syncfusion-overrides.css` | **Видалити** |
| `wwwroot/css/fonts/Unmatched.otf` | **Видалити** |
| `wwwroot/css/style.css`, `site.css` | Почистити від `.page-content`/`.page-title` (переїжджають у `base.css`/`typography.css`) |
| `Pages/_Host.cshtml` | Додати Google Fonts (`Chakra Petch` 500/600/700, `Barlow` 400–700, `IBM Plex Mono` 400/500); bootstrap-icons `1.10.3 → 1.11.3`; підключити `base.css`; прибрати Syncfusion CSS/JS |
| `Unmatched.UI.BlazorServer.csproj`, `Program.cs`, `_Imports.razor` | Прибрати `Syncfusion.Blazor` |

### Базові компоненти — `Shared/DesignSystem/`

Кожен компонент — `*.razor` + scoped `*.razor.css`; жодних інлайн-кольорів, лише `var(--um-*)`.

**Оболонка й навігація**
`NavRail.razor` (216⇄76, групи Statistics/Play, лічильники, Collapse, шестерня Settings) ·
`PageHeader.razor` (breadcrumb + слот пошуку + слот контролів + один градієнтний Action) ·
`Breadcrumb.razor` · `NavState.cs` (scoped, тримає `navOpen`)

**Поверхні й метрики**
`Panel.razor` (radius 15, hairline, слоти Title/Caption/Actions) · `KpiCard.razor` (іконка + label +
metric + sub) · `StatTile.razor` (inset-плитка PLACE/POINTS/HP/DECK) · `SectionCaption.razor`

**Домен**
`ModeBadge.razor` (`GameMode` → 1v1/2v2/FFA/CO-OP + колір) · `ResultBadge.razor` (WIN/LOSS) ·
`MedalBadge.razor` (1/2/3, gold/silver/bronze) · `FighterChip.razor` (30px арт, зелений ринг у
переможця, аватар гравця знизу-справа, place-бейдж зверху-зліва; villain/minion — без аватара) ·
`SideBox.razor` (група чипів + роздільник `VS` або `›`) · `EpicStars.razor` (read-only / editable) ·
`HeroToken.razor` (32px арт із фолбеком `/Unknown.png`) · `MeleeRangedIcon.razor`

**Контроли**
`UmButton.razor` (`primary` градієнт, `outline`, `ghost`, `danger`, `icon`, `dice`) ·
`UmField.razor` (mono-caption label + inset shell) · `UmSelect.razor` (заміна `InputSelect`/`SfComboBox`;
опційні аватар/арт + кнопка-кубик) · `UmNumberField.razor` · `UmSearchBox.razor` ·
`SegmentedControl.razor` · `FilterChip.razor` (дропдаун-чип) · `UmToggle.razor` · `UmCheckbox.razor` (20px) ·
`Pagination.razor` · `Tabs.razor`

**Списки й таблиці**
`DataPanel.razor` (панель + caption-рядок заголовків + слот рядків + слот пагінації) ·
`LadderRow.razor` (сітка 1c: ранг, токен, назва+підпис, type-іконка, значення, W/L-бар, K/D;
`border-left` за результатом) · `WinLossBar.razor` · `MatchupBar.razor`

**Графіки (власний SVG, без Chart.js)**
`DonutChart.razor` (win rate) · `RadarChart.razor` (4 осі, 0–3) · `LineChartSvg.razor` (рейтинг + area
+ підписи осей + перемикач 6M/1Y/All) · `Sparkline.razor` · `Heatmap.razor`

**Матчлог** — `Shared/MatchLog/`
`MatchRow.razor` (сітка `82px 84px 1fr 116px 60px 24px`) · `MatchScoreboard.razor` (колонка Side лише
для 2v2/co-op; мініони з відступом 20px і прочерком у Cards; редагування Epic) · `MatchList.razor`

**Демо-сторінка** — `Pages/DesignSystem/DesignSystem.razor` (`/design-system`), рендерить усі
компоненти в порядку `00-design-system.png`. Слугує критерієм готовності кроку 2 і живою
документацією.

---

## 4. Порядок робіт

Кожен крок — окремий PR. Критерій готовності всюди включає `dotnet build Unmatched.sln` без
ворнінгів nullable і зелені відповідні `*.Tests`.

| # | Крок | Критерій готовності |
|---|---|---|
| **1** | Токени, типографіка, шрифти, чистка CSS, bootstrap-icons 1.11.3 | Колірні й типографічні зразки збігаються з `00-design-system.png`; застосунок запускається (старі екрани тимчасово виглядають сиро — це очікувано) |
| **2** | Базові компоненти дизайн-системи + `/design-system` | Демо-сторінка візуально збігається з `00-design-system.png` секція за секцією |
| **3** | Оболонка: `NavRail`, `MainLayout`, `PageHeader`, `NavState`, `NavCountsService`, `Pages/Settings`, `/` → ladder | Рейл 216⇄76 з `width .18s ease`, стан тримається між сторінками, згорнутий показує іконки з `title`, лічильники живі, шестерня веде на Settings із перерахунком рейтингів та ініціалізацією |
| **4** | Catalog-бекенд (розділ 2A): міграція, Include, DTO, `PUT /expansion/{id}/image`, асети | `GET /expansion` віддає героїв/мапи/вілленів/мініонів + `imageFileName`; тести Catalog зелені |
| **5** | Statistics-бекенд (розділ 2B): `ExpansionId` у кеші, `ExpansionStatisticsService`, `GET /expansion/stats`, клієнт | Ендпойнт повертає PLAYED / WIN RATE / BEST HERO по кожному сету; тести Statistics зелені |
| **6** | Match-бекенд (розділ 2C): `GET /title/hero/{heroId}`, прошивка `Titles`, час матчу в Add match | 2a показує реальні тайтли з коментарем; новий матч зберігає час; тести Match зелені |
| **7** | **3b** — `MatchRow` + `MatchScoreboard` + `MatchList` | Один-в-один із `3b-match-row.png` для всіх 4 режимів. Клік розкриває рівно один рядок; колонка Side лише для 2v2 (Team 1/2) і co-op (Heroes/Villains); мініони з відступом і прочерком у Cards; villain/minion без аватара; FFA-роздільник `›` + медалі; жодних items/actions/time |
| **8** | **6a** — сторінка Match log | Один-в-один із `6a-match-log.png`. Сегментований фільтр реально фільтрує; чипи player/hero/map/tournament/date працюють; пошук; лічильник «8 of 1 284 matches»; пагінація; лівий край рядка — колір **режиму**, не результату; Export disabled |
| **9** | **1c** — Heroes ladder | Один-в-один із `1c-heroes-ladder.png`. Таби Points/Win rate/K-D/Recently played сортують; hover і клік по рядку оновлюють праву колонку (spotlight + radar, best/worst matchups, collection 6/11); клік відкриває сторінку героя; від'ємні очки червоні; медалі для топ-3 |
| **10** | **2a** — Hero detail | Один-в-один із `2a-hero-detail.png` у порядку: банер → 4 картки → play style + titles/sidekick + donut → рейтингова лінія → antagonist + matchups → матчлог → against villains/minions. Prev/Next hero працюють; аплоад арту й вибір кольору героя збережено; SVG замість Chart.js |
| **11** | **5a** — Collection | Один-в-один із `5a-collection.png`. Owned only фільтрує; чекбокс на обкладинці **не** змінює вибраний сет; статуси `OWNED`/`ADD`/`UNSAVED` (золотий) із лічильником незбережених; `Save collection` комітить усе разом; права панель показує героїв (deck, type, HP), мапи 44px, віллена з мініонами; аплоад обкладинки |
| **12** | **4b** — Add match | Один-в-один із `4b-add-match.png`. Сегментований перемикач перебудовує область учасників; FFA 3–4 гравці («Add player» зникає на 4, кошик повертається на 3) з Place-дропдауном і медаллю; co-op — «Add minion», віллен без сайдкіка й карт; кубик усюди, де обирається герой або мапа; футер зі зведенням + Save match / Save & add another |
| **13** | Ненадизайнені списки й деталі: Players, Maps, Minions, Villains | Побудовані виключно з компонентів кроку 2; жодного нового кольору, шрифту, радіуса, типу графіка. Player detail: Favorites у стилі 1c, donut замість пай-чарта |
| **14** | Titles, Tournaments, діалоги; **повне видалення Syncfusion** | `grep -ri syncfusion` по репозиторію порожній (крім історії міграцій); пакет прибрано з `.csproj`; `dotnet build Unmatched.sln` зелений; призначення тайтлів і генерація стадій турніру працюють як раніше |

---

## 5. Ризики й місця можливого розходження з макетом

1. **Банер 2a буде порожнішим за макет.** Рішення #1 — показуємо круглий токен 200×200 замість
   key art 3:4. Це збігається зі скріншотом (там теж кругла аватарка на штрихованому тлі), але
   якщо колись з'явиться справжній 3:4-арт, знадобиться окреме поле й міграція.
2. **Час у рядку з'явиться лише на нових матчах.** Уся історія має `00:00`, тож перші тижні
   більшість рядків буде без другого рядка дати — свідомий наслідок рішення #2.
3. **PLAYED / WIN RATE по сету — це «участі героїв», не «матчі».** Ми сумуємо `HeroStats` героїв
   сету; матч 2v2, де двоє героїв з одного сету, порахується двічі. Точний підрахунок вимагав би
   join матчів із каталогом на боці Match-сервісу.
4. **Чип «5 minions» може не збігтися з коробкою.** За рішенням #6 рахуємо рядки `Minion` за
   `ExpansionId`. Якщо в каталозі один рядок на тип мініона, а в коробці їх кілька копій, число
   буде меншим за макет.
5. **Матчлог у пам'яті.** На 1 284 матчах це прийнятно, але кожен вхід на сторінку тягне весь лог
   через HTTP. При зростанні до 5–10k доведеться повертатися до серверної фільтрації й пагінації.
6. **Нав-рейл у прототипі 1c має 64px іконкову колонку**, тоді як README задає 216/76. Беремо
   README — саме він описаний як специфікація «для всіх екранів»; скріншот 1c просто знято в
   згорнутому стані старішої ітерації.
7. **У шапці 2a праворуч є аватарка користувача.** Застосунок не per-user — прибираємо, лишаємо
   Prev/Next hero.
8. **Заміна Chart.js/BlazorBootstrap-графіків на власний SVG** позбавляє тултіпів і зуму. Макет їх
   не показує, але це втрата функціональності на 2a. `BlazorBootstrap` лишається заради `Preload`
   і `Modal`; CDN Chart.js із `_Host.cshtml` прибираємо, коли зникне останній `LineChart`.
9. **Видалення Syncfusion — найбільший обсяг «непомітної» роботи.** `Titles.razor` спирається на
   командні колонки, мультиселект і `SelectRowsAsync`; `Tournaments.razor` — на командні колонки.
   Це доведеться відтворити вручну (крок 14) і воно не має жодного макета.
10. **Ламаюча зміна контракту `ExpansionDto.HeroNames` → `Heroes`.** Єдиний споживач —
    `Collection.razor`, який усе одно переписується, але Catalog API і клієнтську бібліотеку треба
    задеплоїти разом.
11. **Мапи без `ImageFileName`** у правій панелі 5a і в превʼю 4b покажуть `/UnknownMap.png` —
    у макеті всі мапи з артом.
12. **Пошук у шапці 6a** трактуємо як фільтр по вже завантаженому логу (герой / гравець / мапа), не
    як глобальний пошук по застосунку — макет не показує сторінки результатів.

---

## 6. Верифікація

- `dotnet build Unmatched.sln`
- `dotnet test Unmatched.sln` (як мінімум `Unmatched.CatalogService.Tests`,
  `Unmatched.StatisticsService.Tests`, `Unmatched.MatchService.Tests` після кроків 4–6)
- `docker-compose up` (потрібен локальний SQL Server із налаштуваннями з `CLAUDE.md`), відкрити
  застосунок на ширині вікна **1240px** і звірити кожен екран із відповідним PNG у
  `design_handoff_unmatched_redesign/screens/`
- Прототип для звірки інтеракцій: `python3 -m http.server` у папці хендофу, далі
  `Unmatched Redesign.dc.html` — клікнути кожну інтеракцію з таблиці «Interactions & state» README
  (`navOpen`, `openMatch`, `logMode`, `addMode`, `colSet`, `colDraft`, `colSaved`) і повторити її в
  застосунку
- `/design-system` у застосунку звіряється з `00-design-system.png`
- Регресія даних: додати матч у кожному з 4 режимів і переконатися, що він коректно зʼявляється в
  матчлозі, на сторінці героя і в статистиці
