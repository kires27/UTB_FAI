import pytest
from dummy import calculate_weighted_average


def test_calculate_weighted_average_success():
    # Test normal behavior
    result = calculate_weighted_average(85.0, 0.4)
    assert result == 34.0


def test_calculate_weighted_average_invalid_weight():
    # Test that it correctly throws an error for bad input
    with pytest.raises(ValueError):
        calculate_weighted_average(85.0, 1.5)