namespace WebAPI.Net6.BaseRepositoryGenerics.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToChinaStandardTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
        }

        public static bool IsValidDateTime(this string dateTimeString)
        {
            return DateTime.TryParse(dateTimeString, out _);
        }


        public static long GetTimeMilliseconds(this DateTime date)
        {
            DateTimeOffset dto = new DateTimeOffset(date);
            return dto.ToUnixTimeMilliseconds();
        }
    }
}
