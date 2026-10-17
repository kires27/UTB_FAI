def calculate_weighted_average(score: float, weight: float) -> float:
    if not (0 <= weight <= 1):
        raise ValueError("Weight must be between 0 and 1")
    return round(score * weight, 2)