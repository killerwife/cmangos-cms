using System.Net;
using System.Text.Json;

namespace cmangos_web_api.ReCaptcha
{
    public class ReCaptchaHelper
    {
        public static bool ReCaptchaPassed(string? gRecaptchaResponse)
        {
            if (gRecaptchaResponse == null)
                return false;

            HttpClient httpClient = new HttpClient();

            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret=your secret key no quotes&response={gRecaptchaResponse}").Result;

            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic? JSONdata = JsonSerializer.Deserialize<dynamic>(JSONres);

            if (JSONdata!.success != "true" || JSONdata.score <= 0.5m)
            {
                return false;
            }

            return true;
        }
    }
}
