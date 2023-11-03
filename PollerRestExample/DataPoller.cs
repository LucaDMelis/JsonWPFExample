using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace PollerRestExample
{
    public enum EnumStatus
    {
        Idle, Error, InWork
    }

    public class DataPoller
    {
        private string filePath = "status.xml";
        private CancellationTokenSource cancellationTokenSource;
        private Timer pollTimer;

        public DataPoller()
        {
            cancellationTokenSource = new CancellationTokenSource();
            pollTimer = new Timer(async _ => await PollAndSaveDataAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        // To get the Current element from the Api

        /*    TO CALL and update all the time I set a timer as well
         *    Timer statusUpdateTimer = new Timer();
            statusUpdateTimer.Interval = 300; // Adjust the interval as needed (e.g., 10 seconds)
            statusUpdateTimer.Elapsed += (sender, e) =>
            {
                Status = poller.GetCurrentStatus();
            };
            statusUpdateTimer.Start();       */
        public EnumStatus GetCurrentStatus()
        {
            return LoadStatusFromFile(filePath);
        }

        // When you want to stop the process
        public void Stop()
        {
            cancellationTokenSource.Cancel();
            pollTimer.Dispose();
        }

        // Poll the information from get and save it in an external file
        private async Task PollAndSaveDataAsync()
        {
            try
            {
                string result = await PollApiAsync("https://localhost:7188/api/supervisor/GetStatus");
                string decodedXml = HttpUtility.HtmlDecode(result);

                int startIndex = decodedXml.IndexOf("<EnumStatus>");
                int endIndex = decodedXml.IndexOf("</EnumStatus>") + "</EnumStatus>".Length;
                string innerXml = decodedXml.Substring(startIndex, endIndex - startIndex);

                XmlSerializer serializer = new XmlSerializer(typeof(EnumStatus));
                EnumStatus status;

                using (StringReader reader = new StringReader(innerXml))
                {
                    status = (EnumStatus)serializer.Deserialize(reader);
                }

                SaveStatusToFile(status, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during polling: {ex.Message}");
            }
        }

        private EnumStatus LoadStatusFromFile(string filePath)
        {
            EnumStatus status = EnumStatus.InWork;

            if (File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EnumStatus));
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        status = (EnumStatus)serializer.Deserialize(reader);
                    }
                }
                catch
                {
                    Console.WriteLine("Empty file");
                }

            }

            return status;
        }

        public static void SaveStatusToFile(EnumStatus status, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EnumStatus));

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, status);
            }
        }

        static async Task<string> PollApiAsync(string apiUrl)
        {
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl); // maybe take out the configure false

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Errore durante la richiesta HTTP. Codice di stato: {response.StatusCode}");
                }
            }
        }

        // This is to call a post request
        public async Task<string> SendPostRequestAsync(string cmd)
        {
            string endpoint = "PostStatus";
            string requestBody = $"\"{cmd}\""; // Wrap the cmd in double quotes to make it a JSON string

            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:7188/api/supervisor/");

                // Prepare the content with the request body
                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"POST request failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                throw new Exception(ex.Message);
            }
        }
    }
}
