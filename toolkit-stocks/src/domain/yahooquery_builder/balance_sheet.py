import traceback
from yahooquery import Ticker

from functions.db_types import get_period_type
from lib.module.printf import printf, level as lv


def build_balance_sheet(ticker: Ticker, symbol: str, stock_id: int, period: str = "a") -> list[dict]:
	'''! Currently only annual periods work !'''
	try:
		if period.lower() not in ["a", "q"]:
			raise TypeError("period has wrong value")
		if not isinstance(symbol, str):
			raise TypeError(f"symbol has wrong data type: {type(symbol)}")
		if stock_id is None:
			raise TypeError("stock_id is None")

		data_list = []
		statements = ticker.balance_sheet(frequency=period).loc[symbol]

		if statements.empty:
			raise ValueError("Statements empty")

		for symbol, row in statements.iterrows():
			data_obj = {
				"stock_id": stock_id,
				"fiscal_date": row.get("asOfDate").date(),
				"period": get_period_type(period, row.get("asOfDate")),
				
				"total_assets": row.get("TotalAssets"),
				"current_assets": row.get("CurrentAssets"),
				"total_liabilities_net_minority_interest": row.get("TotalLiabilitiesNetMinorityInterest"),
				"current_liabilities": row.get("CurrentLiabilities"),
				"net_tangible_assets": row.get("NetTangibleAssets"),
				"stockholders_equity": row.get("StockholdersEquity"),
				"retained_earnings": row.get("RetainedEarnings"),
				"share_issued": row.get("ShareIssued"),
				"cash_cash_equivalents_and_short_term_investments": row.get("CashCashEquivalentsAndShortTermInvestments"),
				"total_debt": row.get("TotalDebt"),
			}

			data_list.append(data_obj)

		return data_list
	except Exception as err:
		tb = traceback.extract_tb(err.__traceback__)[-1]
		printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)
		return []
