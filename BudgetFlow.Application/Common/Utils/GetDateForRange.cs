namespace BudgetFlow.Application.Common.Utils
{
    public static class GetDateForRange
    {
        public static DateTime GetStartDateForRange(string range)
        {
            var currentDate = DateTime.UtcNow;
            switch (range)
            {
                case "1w":
                    return currentDate.AddDays(-7);
                case "1m":
                    return currentDate.AddMonths(-1);
                case "3m":
                    return currentDate.AddMonths(-3);
                case "6m":
                    return currentDate.AddMonths(-6);
                case "12m":
                    return currentDate.AddYears(-1);
                default:
                    return currentDate;
            }
        }
        public static DateTime GetPreviousStartDateForRange(string range)
        {
            var currentDate = DateTime.UtcNow;
            switch (range)
            {
                case "1w":
                    return currentDate.AddDays(-14);
                case "1m":
                    return currentDate.AddMonths(-2);
                case "3m":
                    return currentDate.AddMonths(-6);
                case "6m":
                    return currentDate.AddMonths(-12);
                case "12m":
                    return currentDate.AddYears(-2);
                default:
                    return currentDate;
            }
        }

    }
}
