import math


def free_cash_flows(cash_flow):
    dates = []
    values = []
    for i in range(len(cash_flow)):
        if i + 1 < len(cash_flow) and cash_flow[i]["fiscal_date"] <= cash_flow[i + 1]["fiscal_date"]:
            raise ValueError("Cash Flow statements corrupted")
        value = cash_flow[i]["free_cash_flow"]
        if isinstance(value, (int, float)) and not math.isnan(value):
            dates.append(cash_flow[i]["fiscal_date"])
            values.append(value)
    return {"dates": dates, "values": values}


def annual_growth_rate(fcfs):
    periods = len(fcfs)
    return (fcfs[0] / fcfs[-1]) ** (1 / periods) - 1


def normalized_free_cash_flow(fcfs):
    return sum(fcfs) / len(fcfs)


def future_free_cash_flows(fcf, length, dagr):
    holder = []
    current = fcf
    for _ in range(length):
        current = current * (1 + dagr)
        holder.append(current)
    return holder


def price_to_free_cash_flow(market_cap, fcf):
    return market_cap / fcf


def discounted_future_free_cash_flows(ffcfs, length, disc):
    holder = []
    for exp in range(1, length + 1):
        index = exp - 1
        holder.append(ffcfs[index] / disc ** exp)
    return holder


def calculate_intrinsic_value(
    market_cap,
    stock_price,
    cash_and_equivalents,
    share_issued,
    cash_flow,
    growth_rate_discount,
    future_discount_rate,
    safety_margin,
    projection,
):
    grd_decimal = 1 - growth_rate_discount / 100
    fdr_decimal = 1 + future_discount_rate / 100
    sm_decimal = 1 - safety_margin / 100

    result = free_cash_flows(cash_flow)
    fcfs = result["values"][:projection]
    projection = len(fcfs)

    agr = annual_growth_rate(fcfs)
    dagr = agr * grd_decimal

    nfcf = normalized_free_cash_flow(fcfs)
    ffcfs = future_free_cash_flows(nfcf, projection, dagr)

    pfcf = price_to_free_cash_flow(market_cap, fcfs[0])
    ter = ffcfs[-1] * pfcf

    dffcfs = discounted_future_free_cash_flows(ffcfs, projection, fdr_decimal)
    dffcf_sum = sum(dffcfs)
    dter = ter / (fdr_decimal ** projection)
    pv = dffcf_sum + dter

    iv = pv + cash_and_equivalents

    ivps = (iv * sm_decimal) / share_issued
    ivpspd = ((ivps - stock_price) / stock_price) * 100

    return {
        "stock_price": stock_price,
        "market_cap": [
            market_cap,
        ],
        "projection_horizon": projection,
        "annual_growth_rate": [
            agr * 100,
        ],
        "intrinsic_value": [
            iv,
        ],
        "intrinsic_value_per_share": [
            ivps,
        ],
        "iv_upside": [
            ivpspd,
        ],
    }
