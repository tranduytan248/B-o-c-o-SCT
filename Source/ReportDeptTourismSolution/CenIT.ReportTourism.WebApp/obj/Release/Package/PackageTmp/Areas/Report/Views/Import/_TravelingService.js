function ValidForm() {
    var isValid =
        ValidMinValue("#PerformPreviousPeriod", "17;18;19;20", 1, true) &
            ValidMinValue("#PerformInPeriod", "17;18;19;20", 1, true) &
            ValidMinValue("#CompareSamePeriodLastYear", "17;18;19;20", 1, true) &
            ValidTotalWithChild("#PerformPreviousPeriod", "1", "2;3;4") &
            ValidTotalWithChild("#PerformPreviousPeriod", "5", "6;7;8;9;10") &
            ValidTotalWithChild("#PerformPreviousPeriod", "11", "12;13;14;15;16") &
            ValidTotalWithChild("#PerformPreviousPeriod", "17", "18;19;20") &
            ValidTotalWithChild("#PerformPreviousPeriod", "21", "22;23;24;25;26;27") &
            ValidTotalWithChild("#PerformInPeriod", "1", "2;3;4") &
            ValidTotalWithChild("#PerformInPeriod", "5", "6;7;8;9;10") &
            ValidTotalWithChild("#PerformInPeriod", "11", "12;13;14;15;16") &
            ValidTotalWithChild("#PerformInPeriod", "17", "18;19;20") &
            ValidTotalWithChild("#PerformInPeriod", "21", "22;23;24;25;26;27") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "1", "2;3;4") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "5", "6;7;8;9;10") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "11", "12;13;14;15;16") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "17", "18;19;20") &
            ValidTotalWithChild("#CompareSamePeriodLastYear", "21", "22;23;24;25;26;27");
    return isValid;
}