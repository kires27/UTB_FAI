import json, traceback
from datetime import datetime, timezone
from yahooquery import Ticker
from lib.module.printf import printf, level as lv


def build_overview(ticker: Ticker, symbol: str) -> dict:
	try:
		if not isinstance(symbol, str):
			raise TypeError(f"symbol has wrong data type: {type(symbol)}")

		asset_profile = ticker.asset_profile.get(symbol, {})
		summary_profile = ticker.summary_profile.get(symbol, {})
		price_info = ticker.price.get(symbol, {})
		key_executives = asset_profile.get("companyOfficers", [])
		
		name = price_info.get("longName") or price_info.get("shortName")

		data = {
			"maintenance_update": datetime.now(timezone.utc),
			"address": asset_profile.get("address1"),
			"description": asset_profile.get("longBusinessSummary"),
			"employees": summary_profile.get("fullTimeEmployees"),
			"sector": asset_profile.get("industry"),	# sector x industry -> is switched on purpose
			"industry": asset_profile.get("sector"),
			"key_executives": json.dumps(key_executives),
			"phone_number": asset_profile.get("phone"),
			"website": asset_profile.get("website"),
			"symbol": price_info.get("symbol"),
			"name": name,
			"logo": None,
			"exchange": price_info.get("exchangeName"),
			"currency": price_info.get("currency"),
			"city": asset_profile.get("city"),
			"country": asset_profile.get("country"),
			"zip": asset_profile.get("zip"),
		}

		return data
	except Exception as err:
		tb = traceback.extract_tb(err.__traceback__)[-1]
		printf(f"{err} (File '{tb.filename}', line {tb.lineno})", lv.ERROR)
		return []
