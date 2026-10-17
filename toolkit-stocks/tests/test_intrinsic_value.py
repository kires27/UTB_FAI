from domain.intrinsic_value.intrinsic_value import calculate_intrinsic_value


def test_calculate_intrinsic_value():

	iv1 = calculate_intrinsic_value(
		market_cap=3829276000000,
		stock_price=255.2,
		cash_and_equivalents=54_697_000_000,
		share_issued=15005000000,
		cash_flow=[
			{
				"fiscal_date": "2025-9-30",
				"free_cash_flow": 98767000000
			},
			{
				"fiscal_date": "2024-9-30",
				"free_cash_flow": 108807000000
				
			},
			{
				"fiscal_date": "2023-9-30",
				"free_cash_flow": 99584000000

			},
			{
				"fiscal_date": "2022-9-30",
				"free_cash_flow": 111443000000

			},
			{
				"fiscal_date": "2021-9-30",
				"free_cash_flow": 92953000000

			}
		],
		growth_rate_discount=15,
		future_discount_rate=10,
		safety_margin=25,
		projection=3
	)
	result1 = round(iv1["intrinsic_value_per_share"][0], 2)

	ivps = {
		"aapl-2025": 54.56,
		"msft-2025": 504.06
	}

	# result2 = calculate_intrinsic_value(

	# )
	
	assert result1 == ivps["aapl-2025"]
	# assert result2 == ivps[2]
