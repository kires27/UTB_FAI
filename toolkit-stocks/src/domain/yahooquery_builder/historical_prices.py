import traceback
from yahooquery import Ticker

from functions.data_types import to_date, map_latest_yahoo_historical
from lib.supabase.sql_functions import get_latest_date
from lib.module.printf import printf, level as lv
import constants.db_tables as dbt


def build_historical_prices(ticker: Ticker, symbol: str, stock_id: int) -> list[dict]:
	try:
		if not isinstance(symbol, str):
			raise TypeError(f"symbol has wrong data type: {type(symbol)}")
		if stock_id is None:
			raise TypeError("stock_id is None")

		latest_statement_date = get_latest_date(dbt.historical_price, stock_id)
		relevant_period = map_latest_yahoo_historical(latest_statement_date)

		data_list = []
		statements = ticker.history(period=relevant_period, interval="1d").loc[symbol]

		if statements.empty:
			raise ValueError("Statements empty")

		# statement date: datetime.date
		for s_date, row in statements.iterrows():
			if to_date(s_date) < latest_statement_date: continue

			data_obj = {
				"stock_id": stock_id,
				"date": s_date,
				"open": float(row.get("open")),
				"high": float(row.get("high")),
				"low": float(row.get("low")),
				"close": float(row.get("close")),
				"volume": float(row.get("volume")),
				"adj_close": float(row.get("adjclose")),
			}

			data_list.append(data_obj)
		
		return data_list
	except Exception as err:
		tb = traceback.extract_tb(err.__traceback__)[-1]
		printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)
		return []
