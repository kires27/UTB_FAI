import traceback
from lib.module.printf import printf, level as lv


def gross_margin(gross_profit: float, total_revenue: float) -> float | None:
	"""
	income_statement => gross_profit, total_revenue
	"""
	try:
		res = (float(gross_profit) / float(total_revenue))
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def operating_margin(operating_income: float, total_revenue: float) -> float | None:
	"""
	income_statement => operating_income, total_revenue
	"""
	try:
		res = (float(operating_income) / float(total_revenue))
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def net_margin(net_income: int, total_revenue: int) -> float | None:
	"""
	income_statement => net_income, revenues
	"""
	try:
		res = (int(net_income) / int(total_revenue))
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def return_on_equity(net_income: float, stockholders_equity: float) -> float | None:
	"""
	income_statement => net_income
	balance_sheet => stockholders_equity (total_shareholders_equity)
	"""
	try:
		res = (float(net_income) / float(stockholders_equity))
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def earning_per_share(net_income: float, share_issued: float, dividends_on_preferred_stocks: float | None) -> float | None:
	"""
	income_statement => net_income, dividends_on_preferred_stocks (otherunder_preferred_stock_dividend)
	balance_sheet => share_issued (number of outstanding shares)
	"""
	try:
		dops = float(dividends_on_preferred_stocks or 0)
		res = (float(net_income) - dops) / float(share_issued)
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def debt_to_equity(total_liabilities: float, stockholders_equity: float) -> float | None:
	"""
	balance_sheet => total_liabilities (Total Liabilities Net Minority Interest),
		stockholders_equity
	"""
	try:
		res = (float(total_liabilities) / float(stockholders_equity))
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def current_ratio(current_assets: float, current_liabilities: float) -> float | None:
	"""
	balance_sheet => current_assets, current_liabilities
	"""
	try:
		res = float(current_assets) / float(current_liabilities)
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def revenues_per_share(total_revenue: float, share_issued: float) -> float | None:
	"""
	income_statement => total_revenue
	balance_sheet => number of outstanding shares/share_issued
	"""
	try:
		res = float(total_revenue) / float(share_issued)
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def capital_index(net_income: float, capital_expenditure: float) -> float | None:
	"""
	income_statement => net_income
	cash_flow => capital_expenditure
	"""
	try:
		res = (-float(capital_expenditure) / float(net_income))
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def payback_period(total_debt: float, free_cash_flow: float):
	"""
	balance_sheet => total_debt
	cash_flow => free_cash_flow
	"""
	try:
		res = float(total_debt) / float(free_cash_flow)
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def assets_price(net_tangible_assets: float, total_liabilities: float, market_cap: float) -> float | None:
	"""
	balance_sheet => net_tangible_assets, total_liabilities (total_liabilities_net_minority_interest)
	"""
	try:
		res = ((float(net_tangible_assets) - float(total_liabilities)) * 1.5) / market_cap
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def pe_ratio(price: float, earnings_per_share: float) -> float | None:
	"""
	overview => price
	? => earnings_per_share
	"""
	try:
		res = float(price) / float(earnings_per_share)
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def pb_ratio(market_cap: float, total_equity: float) -> float | None:
	"""
	overview => market_cap
	balance_sheet => total_equity (previously was total_assets)
	"""
	try:
		res = float(market_cap) / float(total_equity)
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def dividend_yield(dividend_yield) -> float | None:
	"""get dividend yield from yahoo finance and check whether its None type"""
	try:
		if dividend_yield is None:
			return None
		
		res = float(dividend_yield) * 100
		return round(res, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None


def growth_calculation(statements: list[dict], key: str) -> float | None:
	"""
	Calculates growth for a given key over a period of years.
	Example: net income growth for 5 years, revenues growth for 5 years, debt growth for 5 years.

	Used for: debt growth, revenue growth, income growth, retained earnings

	:param statements: list of dicts, each representing a statement for a year (most recent first)
	:param key: key in the statement dict to calculate growth for
	:return: average annual growth percentage or None if data missing/invalid
	"""
	try:
		newest = float(statements[0][key])
		oldest = float(statements[-1][key])

		if newest * oldest < 0:
			# TODO add another growth calc method

			return None
		else:
			growth = (pow((newest / oldest), 1 / len(statements)) - 1)

		return round(growth, 4)
	except Exception as error:
		tb = traceback.extract_tb(error.__traceback__)[-1]
		printf(f"{error} (File '{tb.filename}', line {tb.lineno})", lv.WARNING)
		return None
