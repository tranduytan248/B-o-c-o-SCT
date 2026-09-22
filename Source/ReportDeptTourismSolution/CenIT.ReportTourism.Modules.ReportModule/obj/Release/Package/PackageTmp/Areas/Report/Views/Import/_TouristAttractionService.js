function ValidForm() {
    var isValid =
        ValidMinValue("#PerformPreviousPeriod", "2;3;4;5", 1, true) &
            ValidMinValue("#PerformInPeriod", "2;3;4;5", 1, true) &
            ValidMinValue("#CompareSamePeriodLastYear", "2;3;4;5", 1, true) &
            ValidTotalWithChild("#PerformPreviousPeriod", "2", "3;4;5") &
            ValidTotalWithChild("#PerformPreviousPeriod", "6", "7;8;9;10;11") &
            ValidTotalWithChild("#PerformInPeriod", "2", "3;4;5") &
            ValidTotalWithChild("#PerformInPeriod", "6", "7;8;9;10;11") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "2", "3;4;5") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "6", "7;8;9;10;11");
    return isValid;
}