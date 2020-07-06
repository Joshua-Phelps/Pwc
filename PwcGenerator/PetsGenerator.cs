using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.FormattableString;

namespace PwcGenerator
{
    internal sealed class PetsGenerator : IDisposable
    {
        private const string AUTH_URI = @"https://api.petfinder.com/v2/oauth2/token";
        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly HttpClient _httpClient;

        private PetFinderToken _token;

        public PetsGenerator(string client_id, string client_secret)
        {
            _clientId = client_id;
            _clientSecret = client_secret;

            _httpClient = new HttpClient();
            _token = GetTokenAsync().Result;
        }

        public async Task GetAnimalsAsync()
        {
            if (_token == null)
            {
                _token = await GetTokenAsync();
            }
            if (_token.Access_Token == null)
            {
                Console.WriteLine("Could not authenticate. Please verify client id and client secret were passed correctly.");
            } else {
                var allAnimals = await GetAnimalsFromOrganizationsAsync();
                await DownloadImagesAsync(allAnimals);
            }
        }

        private async Task DownloadImagesAsync(List<PetFinderAnimal> animals)
        {
            Directory.CreateDirectory(ConfigurationSettings.WriteLocation);

            await ForEachAsync(animals, ConfigurationSettings.NumberOfParallelDownloads, async (animal) =>
            {
                Console.WriteLine(string.Format("Fetching animal {0}", animal.ID));
                for (int i = 0; i < animal.Photos.Length; i++)
                {
                    var photo = animal.Photos[i];
                    string extension = Path.GetExtension(photo.Full);
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = ".jpg";
                    }
                    string imageDestination = Path.Combine(ConfigurationSettings.WriteLocation, Invariant($"{animal.ID}_{i}{extension}"));
                    var response = await _httpClient.GetAsync(photo.Full);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var file = new FileStream(imageDestination, FileMode.CreateNew, FileAccess.Write))
                        {
                            await stream.CopyToAsync(file);
                        }
                    }

                    animal.Photos[i].LocalPath = imageDestination;
                }
            });

            string jsonOutput = JsonConvert.SerializeObject(animals);
            string outputDestination = String.Format("{0}output.txt", ConfigurationSettings.WriteLocation);
            File.WriteAllText(outputDestination, jsonOutput);
        }

        // curl -H "Authorization: Bearer <insert bearer>" https://api.petfinder.com/v2/animals?limit=500 > dogs.txt
        private async Task<List<PetFinderAnimal>> GetAnimalsFromOrganizationsAsync()
        {
            // form organizations query
            StringBuilder orgSB = new StringBuilder();

            foreach (string orgID in ConfigurationSettings.OrganizationIDs)
            {
                orgSB.Append(orgID);
                orgSB.Append(",");
            }
            orgSB.Remove(orgSB.Length - 1, 1);

            string url = String.Format(
                @"https://api.petfinder.com/v2/animals?limit={0}&organizations={1}",
                100,
                orgSB.ToString());

            PetFinderAnimals allAnimals;
            List<PetFinderAnimal> animalsList = new List<PetFinderAnimal>();
            while (animalsList.Count < ConfigurationSettings.TotalResults && url != null)
            {
                using (var client = new HttpClient())
                {
                    // Set authorization token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._token.Access_Token);
                    var request = await client.GetAsync(url);
                    var response = await request.Content.ReadAsStringAsync();

                    allAnimals = JsonConvert.DeserializeObject<PetFinderAnimals>(response);
                    animalsList.AddRange(allAnimals.animals);
                }

                if (allAnimals.pagination != null && allAnimals.pagination._links != null && allAnimals.pagination._links.next != null && allAnimals.pagination._links.next.href != null)
                {
                    url = string.Format(@"https://api.petfinder.com{0}", allAnimals.pagination._links.next.href);
                } else
                {
                    url = null;
                }
            }

            return animalsList;
        }

        // Get authorization token to be used in subsequent requests
        // The authorization http post should look like the following:
        // curl -d "grant_type =client_credentials&client_id=<client id>&client_secret=<secret>" https://api.petfinder.com/v2/oauth2/token > token.txt
        private async Task<PetFinderToken> GetTokenAsync()
        {
            //Prepare Request Body
            List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
            requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            requestData.Add(new KeyValuePair<string, string>("client_id", (_clientId)));
            requestData.Add(new KeyValuePair<string, string>("client_secret", _clientSecret));

            FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

            //Request Token
            var request = await _httpClient.PostAsync(AUTH_URI, requestBody);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PetFinderToken>(response);
        }

        private static Task ForEachAsync<T>(IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async () => {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            await body(partition.Current);
                        }
                    }
                }));
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
