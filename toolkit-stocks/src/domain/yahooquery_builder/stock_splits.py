import traceback
from yahooquery import Ticker

from functions.data_types import round_number
from lib.module.printf import printf, level as lv


def build_stock_splits(ticker: Ticker, symbol: str, stock_id: int) -> list[dict]:
	try:
		if not isinstance(symbol, str):
			raise TypeError(f"symbol has wrong data type: {type(symbol)}")
		if stock_id is None:
			raise TypeError("stock_id is None")

		data_list = []
		statements = ticker.history(period="max").loc[symbol]

		if statements.empty:
			raise ValueError("Statements empty")

		# if not isinstance(history_df.index, pd.MultiIndex):
		#     history_df = history_df.reset_index()

		split_rows = statements[statements["splits"] != 0]

		for date, row in split_rows.iterrows():
			data_obj = {
				"stock_id": stock_id,
				"date": date,
				"factor": round_number(row["splits"]),
			}
			data_list.append(data_obj)

		return data_list
	except Exception as err:
		tb = traceback.extract_tb(err.__traceback__)[-1]
		printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)
		return []
