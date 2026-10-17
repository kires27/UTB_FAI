import traceback
from yahooquery import Ticker

from lib.supabase.sql_functions import get_latest_date
from lib.module.printf import printf, level as lv
import constants.db_tables as dbt


def build_dividends(ticker: Ticker, symbol: str, stock_id: int) -> list[dict]:
	try:
		if not isinstance(symbol, str):
			raise TypeError(f"symbol has wrong data type: {type(symbol)}")
		if stock_id is None:
			raise TypeError("stock_id is None")

		latest_statement_date = get_latest_date(dbt.dividend, stock_id)

		data_list = []
		statements = ticker.dividend_history(latest_statement_date).loc[symbol]

		if statements.empty: raise ValueError("Statements empty")

		for date, row in statements.iterrows():
			data_obj = {
				"stock_id": stock_id,
				"date": date,
				"dividend": float(row.get("dividends"))
			}

			data_list.append(data_obj)
		
		return data_list
	except Exception as err:
		tb = traceback.extract_tb(err.__traceback__)[-1]
		printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)
		return []
