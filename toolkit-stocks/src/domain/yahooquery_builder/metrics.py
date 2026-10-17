import traceback
from datetime import datetime, timezone
from yahooquery import Ticker

from functions.data_types import time_to_timestamptz
from lib.module.printf import printf, level as lv


def build_metrics(ticker: Ticker, symbol: str, stock_id: int) -> dict:
	try:
		if not isinstance(symbol, str):
			raise TypeError(f"symbol has wrong data type: {type(symbol)}")
		if stock_id is None:
			raise TypeError("stock_id is None")

		price_data = ticker.price.get(symbol, {})
		if len(price_data) == 0:
			raise ValueError("Price statements is empty")
		
		price = price_data.get("regularMarketPrice")
		market_cap = price_data.get("marketCap")
		regular_market_change_percent = price_data.get("regularMarketChangePercent")
		regular_market_change = price_data.get("regularMarketChange")
		regular_market_time = time_to_timestamptz(price_data.get("regularMarketTime"), "%Y-%m-%d %H:%M:%S")
		
		return {
			"stock_id": stock_id,
			"maintenance_update": datetime.now(timezone.utc),
			"stock_price": price,
			"market_cap": market_cap,
			"regular_market_change_percent": regular_market_change_percent,
			"regular_market_change": regular_market_change,
			"regular_market_time": regular_market_time,
		}
	except Exception as err:
		tb = traceback.extract_tb(err.__traceback__)[-1]
		printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)
		return {}
