import argparse, sys, os
from lib.module.printf import printf, level as lv
from dotenv import load_dotenv


if os.getenv("GITHUB_ACTIONS") != "true":
	printf.open("log", create_log=True, console_print=True)
	load_dotenv()

	from data.symbols import manual_symbols
else:
	printf.open("log", create_log=False, console_print=True)
	manual_symbols = None


parser = argparse.ArgumentParser(description="MICE script to insert/update data to database")
parser.add_argument("--overview", action="store_true", help="Insert overview data")
parser.add_argument("--balance", action="store_true", help="Insert balance sheet data")
parser.add_argument("--cash", action="store_true", help="Insert cash flow data")
parser.add_argument("--income", action="store_true", help="Insert income statement data")
parser.add_argument("--dividends", action="store_true", help="Insert dividends data")
parser.add_argument("--prices", action="store_true", help="Insert prices data")
parser.add_argument("--splits", action="store_true", help="Insert stock splits data")
parser.add_argument("--metrics", action="store_true", help="Insert metrics data")
parser.add_argument("--fa", action="store_true", help="Insert financial analysis data")
args = parser.parse_args()

if not (
	args.overview or
	args.balance or
	args.cash or
	args.income or
	args.dividends or
	args.prices or
	args.splits or
	args.metrics or
	args.fa
):
	printf("Nothing to update. Missing arguments!", lv.ERROR)
	printf.close()
	sys.exit(1)


import time
from yahooquery import Ticker
from domain.yahooquery_builder import \
	build_overview, \
	build_balance_sheet, \
	build_cash_flow_statement, \
	build_income_statement, \
	build_dividends, \
	build_historical_prices, \
	build_stock_splits, \
	build_financial_analysis, \
	build_quality_score, \
	build_metrics
from functions.helper import batch_tickers
from lib.supabase.sql_functions import get_stock_id, get_symbols, insert_data_statement
from lib.supabase.init import get_connection


conn = get_connection()

symbols: list = manual_symbols if manual_symbols else get_symbols()
symbol_index = 0

printf(f"-- All symbols: \n{symbols}\n--", lv.INFO)

for batch in batch_tickers(symbols):
	ticker = Ticker(batch, asynchronous=False)

	printf(f"-- Batch ready: {batch} --", lv.INFO)

	for symbol in batch:
		symbol_index += 1
		printf(f"---- ({symbol_index}.) {symbol} ----", lv.INFO)

		stock_id = get_stock_id(symbol)

		if args.overview:
			printf("-- OVERVIEW --", lv.INFO)
			mode_condition = {} if stock_id is None else {"symbol": symbol}
			
			insert_data_statement(
				build_overview(ticker, symbol),
				"stock",
				update_condition=mode_condition
			)

			if stock_id is None:
				stock_id = get_stock_id(symbol)
		
		if stock_id is None:
			printf("Stock id is None", lv.ERROR)
			continue

		if args.balance:
			printf("-- BALANCE SHEET --", lv.INFO)
			insert_data_statement(
				build_balance_sheet(ticker, symbol, stock_id, "a"),
				"stock_balance_sheet",
				null_tolerance=0.6
			)

		if args.cash:
			printf("-- CASH FLOW --", lv.INFO)
			insert_data_statement(
				build_cash_flow_statement(ticker, symbol, stock_id, "a"),
				"stock_cash_flow",
				null_tolerance=0.17
			)

		if args.income:
			printf("-- INCOME STATEMENT --", lv.INFO)
			insert_data_statement(
				build_income_statement(ticker, symbol, stock_id, "a"),
				"stock_income_statement",
				null_tolerance=0.4
			)

		if args.dividends:
			printf("-- DIVIDENDS --", lv.INFO)
			insert_data_statement(
				build_dividends(ticker, symbol, stock_id),
				"stock_dividend",
				null_tolerance=0.0
			)

		if args.prices:
			printf("-- HISTORICAL PRICES --", lv.INFO)
			insert_data_statement(
				build_historical_prices(ticker, symbol, stock_id),
				"stock_historical_price",
			)

		if args.splits:
			printf("-- STOCK SPLITS --", lv.INFO)
			insert_data_statement(
				build_stock_splits(ticker, symbol, stock_id),
				"stock_split",
			)

		if args.metrics:
			printf("-- METRICS --", lv.INFO)
			insert_data_statement(
				build_metrics(ticker, symbol, stock_id),
				"stock_metric",
				update_condition={"stock_id": stock_id}
			)

		if args.fa:
			printf("-- FINANCIAL ANALYSIS --", lv.INFO)
			fa = build_financial_analysis(ticker, symbol, stock_id)
			insert_data_statement(
				fa["data"],
				"stock_financial_analysis",
				update_condition={
					"stock_id": stock_id,
					"fiscal_date": fa["fiscal_date"],
					"period": fa["period"]
				}
			)

			fqs = build_quality_score(stock_id)
			insert_data_statement(
				fqs["data"],
				"stock_financial_analysis",
				update_condition={
					"stock_id": stock_id,
					"fiscal_date": fa["fiscal_date"],
					"period": fa["period"]
				}
			)

	time.sleep(1)


conn.cursor().close()
conn.close()
printf("Disconnected from the database", lv.INFO)
printf.close()
