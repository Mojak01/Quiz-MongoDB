using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labb3
{
    public class TriviaFetcher
    {
        HttpClient client = new HttpClient();

        public async Task<List<QuestionItem>> GetFromApi(int count, string catId, string diffLevel)
        {
            string address = "https://opentdb.com/api.php?amount=" + count;

            if (catId != "")
            {
                address = address + "&category=" + catId;
            }

            if (diffLevel != "")
            {
                address = address + "&difficulty=" + diffLevel;
            }

            address = address + "&type=multiple";

            var data = await client.GetFromJsonAsync<ApiRoot>(address);

            if (data != null)
            {
                return data.results;
            }

            return new List<QuestionItem>();
        }
    }

    public class ApiRoot
    {
        public int response_code { get; set; }
        public List<QuestionItem> results { get; set; }
    }

    public class QuestionItem
    {
        public string category { get; set; }
        public string difficulty { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
        public List<string> incorrect_answers { get; set; }
    }
}