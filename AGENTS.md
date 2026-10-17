# About
This is NOT a managed monorepo. No root `package.json`, no root workspace.
Each directory is independently runnable.

## Rules
- Always activate skill /caveman
- Keep this file under 300 lines.

## Repo Layout

`TASK.md` is the anchor for the active task. Keep it human-readable, step-by-step, and simply formatted.
`PHILOSOPHY.md` provides background context when relevant.

| Directory | What | Coverage |
|-----------|------|----------|
| `web/` | Nuxt 4 app (main deliverable) | this file |
| `toolkit-stocks/` | Python data pipeline (yahooquery → Supabase) | this file |
| `toolkit-congress/` | empty — placeholder |
| `concept/` | design artifacts, prototypes, calculations — NOT code |


# web

## Commands (run from `web/`)
Package manager is **pnpm**.
NO lint command — never run `pnpm exec eslint` (user forbade it; config also broken)

## Key Config (nuxt.config.ts)
- Nuxt 4: `app/` is root, `~` → `app/`, `~~` → web root.
- Components auto-imported from `~/ui` (no prefix, no `app/components/`).
- Global CSS: only `~/styles/app.css` (design tokens).
- `supabase.redirect: false` — route protection via `app/middleware/auth.js`.
- `.env` vars: `SUPABASE_URL`, `SUPABASE_KEY`.

## Shared code (`#shared`) — GOTCHA
Code in `web/shared/` is cross-app/server, imported via `#shared` alias.
- Must be pure JS — no Vue, no Nitro APIs.
- **Imports BETWEEN files inside `shared/` must be relative WITH `.js` extension** (e.g. `./financial-analysis-shape.js`). Build aliases fail at runtime because Nitro externalizes shared `.js`.

## Shared Code Conventions
- **DB references** — Use `TABLES`/`COLUMNS` from `#shared/constants/database` over hardcoded names.
- **Paths** — `URI` for frontend routes, `API` for endpoints, both from `#shared/constants/routes`. Never hardcode URL strings.
- **Globals** — Store app-wide constants in `#shared/constants/variables`.
- **Reusable queries** — Put composable Supabase query functions in `#shared/query/*.js`. They accept a client, return raw data — no logic.
- **Server endpoints** — Foldered by feature (`server/api/stock/`, `server/api/watchlist/`). One file per usage, named by action (`search-history.get.js`, `insert-row.post.js`).

## Client Utils (`web/app/utils/`)
Shared client-side code, **explicit imports required** (no auto-import).
- `cache.js` — `cached(key, fn)` in-memory fetch store (MAX 100 entries), `invalidate` / `invalidatePrefix` / `clearCache`. Use for client fetches keyed by symbol.
- `toast.js` — global toast system: `useToast()` → `{ toasts, showToast, removeToast }`, `redirectWithToast(url, msg, { theme })`. Toasts render in `app.vue` stack (newest at bottom). Toast-triggering redirects must be CLIENT-only (server redirect loses in-memory toast state).
- Others: `validation.js`, `checkData.js`, `formatData.js`, `functions.js`, `getUrlSubpage.js`.

## Key Architecture Facts
- **`app/features/watchlist/`** — Provider pattern with auth-aware dual storage (Supabase for authed, localStorage for anon). Never call Supabase `.from()` directly in pages.
- **`[symbol].vue` shell** — Fetches stock header once. Child pages use `definePageMeta({ keepalive: true })`, fetch own data keyed by symbol.
- **SSR quirk** — Data differing between SSR (empty) and client: use `isHydrated` ref + `onMounted()`, wrap UI in `v-if="isHydrated"`.

## Server Endpoints
```
stock/
  GET  [symbol].get.js                      stock overview
  GET  metric/[symbol].get.js               real-time price/metrics
  GET  financial-analysis/[symbol].get.js   FA metrics + industry medians
  GET  search-history.get.js                batch stock name + price by symbol list
  GET  search/[input].get.js                search by symbol/name
  GET  stock-list/[limit].get.js            list stocks
  POST intrinsic-value/[symbol].post.js     DCF analysis
watchlist/
  GET  display-watchlist.get.js             combined FA + price row for table
  GET  list-of-watchlists.get.js
  GET  watchlist-items.get.js
  GET  watchlist-has-stock/[symbol].get.js
  POST create-watchlist.post.js
  POST insert-row.post.js
  POST remove-row.post.js
global-index/
  GET  display-global-index.js
user/
  POST create.post.js
```

### Financial-analysis response shape
```js
{ maintenance_update, quality_score, median_comparison, groups, result: {
  [key]: { value, median, group, isPercentage, valueAboveMedianIsGood }
} }
```

## Server Auth
```js
import { supabaseClientAuth } from "~~/server/utils/supabase/auth"
const { client, user } = await supabaseClientAuth(event, false) // false = public/SSR-safe
```
- Auth defaults to true (401 if no user). Public endpoints pass `false`.
- `useSupabaseAdmin()` bypasses RLS.
- Client-side: `useSupabaseUser()` / `useSupabaseClient()` — only in feature providers and middleware.

## Caching
- `setResponseHeader(event, 'Cache-Control', 'public, max-age=86400, stale-while-revalidate=3600')` for public endpoints.
- NEVER on personal user data — shared cache leaks data between users.

## Design System
Tokens in `app/styles/app.css` CSS variables. Never hardcode colors.
- Backgrounds: `--bg`, `--bg-2`, `--bg-card`, `--bg-hover`
- Borders: `--border`, `--border-hi`
- Foregrounds: `--fg`, `--fg-2`, `--fg-3`
- Accents: `--teal`, `--teal-dim`, `--teal-border`, `--purple`, `--gold`, `--rose`, `--rose-dim`, `--blue`
- Fonts: `--font-main`, `--font-sans`
- Layout: `--container-max` (1180px), `--header-height` (62px)

## Components
- `<Text>`: variant `hero|h1|h2|h3|body|body-sm|label`, tone `muted|green|purple`.
- `<Button>`: variant `primary|secondary|danger`, size `sm|md|lg`.
- `<Searchbar />`: emits `@select(symbol)`, slot `#menuAction`. Hits `/api/stock/search/:input` at ≥2 chars.
- `<Section>`: `topMargin`/`bottomMargin` props (`''|sm|md|lg`).
- `<Grid>`: `cols`, `gap` (`na|sm|md|lg`), `bordered`, `alignContent` (`stretch|start|center|end|space-between|space-around|space-evenly`).
- `<Toast>`: global only — rendered by `app.vue` stack, never per-page.
- Icons: `<Icon name="material-symbols:NAME" />`. Charts: `echarts` / `vue-echarts`.

## Conventions
- Always use `;` at end of JS lines.
- Prettier: tabs (tabWidth 4), `printWidth: 70`, double quotes, `semi: true`, `trailingComma: all`, `bracketSameLine: true`.
- Use tabs (size 4) when editing files in the agent.
- Percent/ratio DB fields are raw 0–1 — multiply ×100 for display (`isPercentage` in shapes signals this).
- `NuxtLink` for routes, `@click` for SPA tab switching.


# toolkit-stocks

## Commands (run from `toolkit-stocks/`)

```bash
# Venv (fish shell)
source .venv-kyos/bin/activate.fish

# Run all data pipelines
python3 src/main.py --overview --balance --cash --income --dividends --prices --splits --fa

# Single pipeline
python3 src/main.py --fa            # financial analysis only
python3 src/main.py --overview      # stock overview (industry, sector, etc.)

# Lint
flake8 src/

# Tests
python -m pytest tests/
```

## CI

Runs `python src/main.py --prices --metrics --fa` daily at 08:00 UTC + on `workflow_dispatch`.

## Entrypoint & Flow

`src/main.py` — iterates all stock symbols, for each symbol dispatches to builder functions in `domain/yahooquery_builder/`. Builders fetch from yahooquery API, compute derived metrics, write to Supabase/PostgreSQL.

**Order matters** for some args: `--overview` must run first to populate `stock.industry` (used by other pipelines as FK).

## Source Layout

```
src/
  main.py                           entrypoint
  constants/db_tables.py            table name constants
  domain/
    yahooquery_builder/             per-stock data fetch + compute
      financial_analysis.py         calls fa_calculations, writes to stock_financial_analysis
      overview.py, metrics.py, etc.
    financial_analysis/
      fa_calculations.py            metric computation functions (ROE, margins, ratios, CAGR)
    financial_quality_score/        quality score (separate post-processing, not per-stock)
      quality_score.py              calculate_quality_score(target_stock_id, fiscal_date, period)
      utils/
        constants.py                METRIC_COLUMNS, HIGHER_IS_BETTER, GROUPS definitions
        sql.py                      DB queries for quality score
    intrinsic_value/                empty/placeholder
  lib/supabase/
    sql_functions.py                shared DB helpers (get_statements_by_stock, insert_data_statement, etc.)
    init.py                         psycopg2 connection singleton from DATABASE_URL
  functions/
    helper.py                       batch_tickers, evaluate_null_tolerance
    data_types.py                   time/date converters
```

## Financial Quality Score

`domain/financial_quality_score/quality_score.py:calculate_quality_score()` computes peer-normalized weighted score for a target stock.

Flow: industry → get peer IDs → fetch their financial analysis at same fiscal_date/period → min-max normalize 0–100 per metric → group into 6 weighted categories → sum → return `{quality_score, category_scores}`.

### Known Bugs in `utils/sql.py`

- **`get_stock_industry()`** — returns tuple `(industry,)`, not string. Caller must index `[0]`.
- **`get_stock_ids_within_industry()`** — uses `fetchone()` not `fetchall()`. Returns single peer ID, not all peers. Needs `fetchall()` + list comprehension.

## DB Schema Notes

- `stock_financial_analysis` table key columns: `stock_id`, `fiscal_date`, `period` (enum: `FY`/`Q1`–`Q4`/`TTM`), `quality_score`, plus 19 metric columns.
- All percent/ratio fields stored as raw values (0–1 or 0–100), not formatted strings.
- Metric columns: `gross_margin`, `operating_margin`, `net_margin`, `return_on_equity`, `earnings_per_share`, `debt_to_equity`, `debt_growth`, `current_ratio`, `revenue_growth`, `gross_income_growth`, `net_income_growth`, `retained_earning_growth`, `revenues_per_share`, `capital_index`, `payback_period`, `assets_price`, `pe_ratio`, `pb_ratio`, `dividend_yield`.
- `stock` table has `industry` + `sector` columns (populated by `--overview`).
- `stock_metric` table stores real-time price/market-cap (updated per run).

## Conventions

- Flake8 config in `.flake8`: ignores `E501,W191,W293,E401,E701,E402,W504,E242,E224,E201,E261`.
- `__init__.py` gets `# noqa: F401` per-file-ignore.
- Exception handling pattern: `except Exception as err: tb = traceback.extract_tb(err.__traceback__)[-1]; printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)`.
- `from lib.module.printf import printf, level as lv` — custom logger, don't use `print()`.
- Run tests with `python -m pytest tests/`. `test/` contains scratch/prototype scripts, not the test suite.
- No pyproject.toml. Dependencies in `requirements.txt`.
- Venv at `.venv-kyos/` (fish shell activation).
