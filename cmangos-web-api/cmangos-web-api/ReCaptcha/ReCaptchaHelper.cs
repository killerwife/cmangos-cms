using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cmangos_web_api.ReCaptcha
{
    public class ReCaptchaHelper
    {
        class RecaptchaResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }
            [JsonPropertyName("error-codes")]
            public List<string> ErrorCodes { get; set; } = new();
            public decimal? Score { get; set; }
            public DateTime? challenge_ts { get; set; }
            public string? Hostname { get; set; }
            public string? Action { get; set; }
        }

        public static bool ReCaptchaPassed(string? gRecaptchaResponse, string secretKey)
        {
            if (gRecaptchaResponse == null)
                return false;

            HttpClient httpClient = new HttpClient();

            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={gRecaptchaResponse}").Result;

            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            string JSONres = res.Content.ReadAsStringAsync().Result;
            RecaptchaResponse? JSONdata = JsonSerializer.Deserialize<RecaptchaResponse>(JSONres);

            if (JSONdata!.Success != true || JSONdata.Score <= 0.5m)
            {
                return false;
            }

            return true;
        }
    }
}
