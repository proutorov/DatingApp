using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Errors");
        }
        public static int CalculateAge(this DateTime inputDateTime)
        {
            var age = DateTime.Today.Year - inputDateTime.Year;
            if (inputDateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }
            return age;
        }
    }

}