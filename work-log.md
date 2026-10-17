# Pracovní deník

## 16.09.2026
Plán pro přepis získavání dat z yahoo finance v rámci optimalizace pipeline.

## 09.09.2026
Deployment na Netlify. Oprava Netlify konfigurace a prostředí. 

## 02.09.2026
Testy: vitest unit setup a Playwright e2e pro auth flow.

## 26.08.2026
Finalizace API tvaru pro financial-analysis: hodnota, medián, skupina, isPercentage, směr nad mediánem.

## 15.08.2026
Opravy SSR hydratace: pattern `isHydrated` + `onMounted` pro data, která se liší mezi serverem a klientem.

## 08.08.2026
Globální toast renderovaný client-only.

## 01.08.2026
POST endpoint napojující DCF analýzu na free cash flow statements.

## 25.07.2026
Začátek intrinsic value, validátory vstupů.

## 18.07.2026
Watchlist tabulka s řazením. batch endpoint `search-history` vrací názvy a ceny akcií pro seznam symbolů v jednom requestu.

## 15.07.2026
Endpoint registrace uživatele a auth stránky. Auth helper na serveru `supabaseClientAuth` jako jednotný průsečík.

## 08.07.2026
Ůdržba kódu, konstanty tabulek/sloupců DB, komponovatelné query funkce.

## 27.06.2026
Caching: optimalizace veřejných dat. Data uživatelů jsou bez cache, aby nedocházelo k úniku dat mezi uživateli.

## 24.06.2026
Quality score: vážené skóre normalizované podle peer skupiny. Metriky min-max normalizované 0-100 v rámci oboru, rozdělené do 6 kategorií.

## 17.06.2026
Financial-analysis UI: skupiny metrik v kartách, porovnání s mediánem, indikátory nad/pod mediánem podle směru metriky.

## 13.06.2026
Financial-analysis endpoint rozšířen o mediány oboru, aby každá metrika šla porovnat s peer akcemi.

## 10.06.2026
Supabase watchlist provider: více watchlistů na uživatele, přidávání/mazání řádků.

## 03.06.2026
Začátek funkce watchlist. Provider interface pro anonymní uživatele (localStorage) i přihlášené uživatele (Supabase).

## 23.05.2026
Základ stránky akce načítá overview a real-time metriky.

## 20.05.2026
Vyhledávání akcií: endpoint `/api/stock/search/[input]` a komponenta `Searchbar` s výsledky podle symbolu/názvu.

## 16.05.2026
Integrace Supabase klienta a auth middleware s ochranou pro linky.

## 09.05.2026
Základí Nuxt 4 aplikace. CSS proměnné pro dizajn, základní UI komponenty (`Text`, `Button`, `Grid`, `Section`, `Card`).

## 06.05.2026
První verze financial-analysis pipeline: základní poměry (ROE, marže, EPS, dluh/vlastní kapitál) počítané per akcie a ukládané do `stock_financial_analysis` s fiskálním datem a obdobím (FY/Q1-Q4).

## 02.05.2026
Přidány pipeline `--prices`, `--splits` a `--dividends` pro historická data.

## 25.04.2026
Přidán pipeline `--metrics` pro real-time cenu a tržní kapitalizaci do tabulky `stock_metric`.

## 22.04.2026
První pipeline: `--overview`. Načítá metadata akcií přes yahooquery a plní tabulku `stock` obory a sektory.

## 18.04.2026
Založen repozitář se dvěma nezávislými částmi: `toolkit-stocks` (Python datový pipeline) a `web` (frontend). Vytvoření projektu.
