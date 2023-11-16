

using System.Net;
using System.Text;
using System.Text.Json;

using PusherClient;

namespace Avalon
{

    public class ValkyrieWebConnectionController
    {

        private static readonly string PUSHER_APP_KEY = "944a01cf9e93fcbc302a";
        private static readonly string PUSHER_APP_CLUSTER = "us2";

        private string _authToken = "";

        private Pusher? _pusher;

        private HttpAuthorizer? _httpAuthorizer = null;

        /// <summary>
        /// the pin code where the client is listening to the web app
        /// </summary>
        private string _pin;
        private string _userName;

        private string _channelName => $"private-channel-{_pin}";

        private bool _authenticated = false;

        public ValkyrieWebConnectionController(string pin, string userName)
        {
            _pin = pin;
            _userName = userName;
            // _pusher = await CreateNewPusherInstance();
        }


        public async Task Connect()
        {

            await CreateNewPusherInstance();

            // if (_pusher == null)
            // {
            //    _pusher = await CreateNewPusherInstance();
            // }

            // try
            // {
            //    await _pusher.ConnectAsync().ConfigureAwait(false);
            //    await _pusher.
            //    // await _httpAuthorizer.AuthorizeAsync(_channelName, _pusher.SocketID);
            // }
            // catch (Exception e)
            // {
            //    Console.WriteLine("Pusher connection failed: " + e.Message);
            // }
        }

        private async Task CreateNewPusherInstance()
        {

            //var Authorizer = new HttpAuthorizer(StateMachineInfo.AuthEndpoint)
            //{
            //    AuthenticationHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken)
            //};
            //_httpAuthorizer = Authorizer;


            var pusherInstance = new Pusher(PUSHER_APP_KEY, new PusherOptions()
            {
                Cluster = PUSHER_APP_CLUSTER,
                Encrypted = true,
                TraceLogger = new TraceLogger()
            });



            pusherInstance.ConnectionStateChanged += StateChanged;
            pusherInstance.Error += Error;
            pusherInstance.Connected += Connected;
            pusherInstance.Disconnected += Disconnected;


            await pusherInstance.ConnectAsync().ConfigureAwait(false);



            _pusher = pusherInstance;
        }

        public async Task Authenticate()
        {

            Console.WriteLine("Authenticating...");

            if (_pusher == null)
                throw new NullReferenceException("the pusher object is null");

            var clientData = new ClientData
            {
                socketId = _pusher.SocketID,
                channelName = _channelName,
                userName = _userName,
                pin = _pin,
                deviceID = StateMachineInfo.Id,
                deviceName = StateMachineInfo.Name,
                deviceDescription = StateMachineInfo.Description,
                deviceType = StateMachineInfo.Type
            };

            var json = JsonSerializer.Serialize(clientData);


            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var response = await client.PostAsync(StateMachineInfo.AuthEndpoint, content);
                Console.WriteLine("response: " + response.StatusCode);
                string data = await response.Content.ReadAsStringAsync();
                Console.WriteLine("response: " + data);

                var jsonResultRoot = JsonDocument.Parse(data).RootElement;

                var authKey = jsonResultRoot.GetProperty("auth").GetString();

                if (authKey != null)
                {
                    _authToken = authKey;
                    Console.WriteLine("Authenticated!");
                }
                else
                {
                    throw new Exception("No auth key returned");
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _authenticated = true;
                }
                else
                {
                    _authenticated = false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Pusher authentication failed: " + ex.Message);
            }
        }

        /// <summary>
        /// subscribe to the pusher channel
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeToChannel()
        {

            // if (!_authenticated)
            // {
            //    Console.WriteLine("Not authenticated, cannot subscribe to channel");
            //    return;
            // }

            // await Authenticate();

            if (_pusher == null)
                throw new NullReferenceException("the pusher object is null");

            try
            {

                var channels = _pusher.GetAllChannels();
                foreach (var c in channels)
                {
                    Console.WriteLine("channel: " + c);
                }

                var channel = await _pusher.SubscribeAsync($"{_channelName}", (x) => { Console.WriteLine("connected??" + x.ToString()); });
                channel.Bind($"my-event", (string data) =>
                {

                    var root = JsonDocument.Parse(data).RootElement;
                    var message = root.GetProperty("data").GetString();

                    Console.WriteLine(message);
                    Console.WriteLine(data);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Pusher subscription failed: " + e.Message);
            }
        }

        /// <summary>
        /// send a message to the pusher channel
        /// </summary>
        /// <param name="message">the message</param>
        public void SendMessage(string message)
        {

            if (_pusher == null)
                throw new NullReferenceException("the pusher object is null");

            try
            {
                var channel = _pusher.GetChannel("public-channel-1");
                channel.Trigger("my-event", new { message = message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pusher message failed: " + ex.Message);
            }
        }

        public async void Disconnect()
        {
            if (_pusher != null)
            {
                await _pusher.DisconnectAsync().ConfigureAwait(false);
            }
        }




        #region events

        void Connected(object sender)
        {
            Console.WriteLine("Connected to Pusher");
        }

        void Disconnected(object sender)
        {
            Console.WriteLine("Disconnected from Pusher");
        }


        /// <summary>
        /// handle the connection state change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        void StateChanged(object sender, ConnectionState state)
        {
            Console.WriteLine("Connection state changed to " + state.ToString());
        }

        /// <summary>
        /// Pusher error handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param> <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        void Error(object sender, PusherException error)
        {
            Console.WriteLine("Pusher Error: " + error.ToString());
        }

        #endregion

    }

    public class ClientData
    {
        public string socketId { get; set; } = "";
        public string channelName { get; set; } = "";
        public string userName { get; set; } = "";
        public string pin { get; set; } = "";

        public string deviceID { get; set; } = "";
        public string deviceName { get; set; } = "";
        public string deviceDescription { get; set; } = "";
        public string deviceType { get; set; } = "";

    }
}
