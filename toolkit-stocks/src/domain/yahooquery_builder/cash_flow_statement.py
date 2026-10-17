import traceback
from yahooquery import Ticker

from functions.db_types import get_period_type
from lib.module.printf import printf, level as lv


def build_cash_flow_statement(ticker: Ticker, symbol: str, stock_id: int, period: str = "a") -> list[dict]:
	try:
		if period.lower() not in ["a", "q"]:
			raise TypeError("period has wrong value")
		if not isinstance(symbol, str):
			raise TypeError(f"symbol has wrong data type: {type(symbol)}")
		if stock_id is None:
			raise TypeError("stock_id is None")

		data_list = []
		statements = ticker.cash_flow(frequency=period, trailing=False).loc[symbol]
		
		if statements.empty:
			printf("cash flow statements are empty", lv.ERROR)
			return []

		for symbol, row in statements.iterrows():
			data_obj = {
				"stock_id": stock_id,
				"fiscal_date": row.get("asOfDate").date(),
				"period": get_period_type(period, row.get("asOfDate")),
				
				"free_cash_flow": row.get("FreeCashFlow"),
				"capital_expenditure": row.get("CapitalExpenditure"),
				"operating_cash_flow": row.get("OperatingCashFlow"),
			}

			data_list.append(data_obj)

		return data_list
	except Exception as err:
		tb = traceback.extract_tb(err.__traceback__)[-1]
		printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)
		return []
