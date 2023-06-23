namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        //This does not account for leap years, but OK for demo purposes
        public static int CalculateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var age = today.Year - dob.Year;

            if (dob > today.AddYears(-age)) --age;

            return age;
        }
    }
}