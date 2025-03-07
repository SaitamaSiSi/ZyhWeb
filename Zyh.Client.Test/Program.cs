using Newtonsoft.Json;
using ProtoBuf;
using System.Text;

namespace Zyh.Client.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Protobuf

            if (true)
            {
                var person = new Person { Name = "Alice", Id = 123, Emails = { "alice@example.com" } };

                byte[] jsonData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(person, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));

                Person parsedJsonPerson;
                parsedJsonPerson = JsonConvert.DeserializeObject<Person>(Encoding.UTF8.GetString(jsonData));


                // 序列化
                byte[] data;
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, person);
                    data = stream.ToArray();
                }

                // 反序列化
                Person parsedPerson;
                using (var stream = new MemoryStream(data))
                {
                    parsedPerson = Serializer.Deserialize<Person>(stream);
                }
            }

            #endregion

            #region Ws通信

            if (false)
            {
                WsClient wsClient = new WsClient("ws://127.0.0.1:5000/api/Ws/entry");
                wsClient.ConnectAsync().Wait();
                wsClient.SendMessageAsync("Client send").Wait();

                while (true)
                {
                    Thread.Sleep(1000);
                    if (Console.ReadLine() == "Q")
                    {
                        break;
                    }
                }
            }

            #endregion
        }
    }
}