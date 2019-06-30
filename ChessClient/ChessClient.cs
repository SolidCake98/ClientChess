using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessClient
{
    public class ChessClient
    {
        public const string userApi = "Users/";
        public const string gameApi = "Chess/";
        public const string getUserApi = "Games/";
        public const string connectApi = "ConnectToGame/";
        public const string createOnce = "CreateOnce/";
        public const string createGameApi = "CreateGame/";
        public const string OfferDrawApi = "Draw/";
        public const string SetDrawApi = "Drawset/";
        public const string DeclineDrawApi = "DeclineDraw/";

        public string host { get; private set; }
        public string user { get; private set; }

        int CurrentGameID;

        public ChessClient(string host, string user)
        {
            this.host = host;
            this.user = user;
        }

        public Task<string> RegisterUser(string host, string username, string password, string email)
        {
            return Task.Run(() =>
            {
                var cli = new WebClient();
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                return cli.UploadString(host, $"{{\"Name\":\"{username}\", \"Password\":\"{password}\", \"Email\":\"{email}\"}}");
            });
        }

        public string GetNameByToken(string host, string token)
        {
            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";
            cli.Headers[HttpRequestHeader.Authorization] = token;
            return cli.DownloadString(host);
        }

        public Task<string> Authentication(string host, string username, string password)
        {
            return Task.Run(() =>
            {
                var cli = new WebClient();
                string host1 = host + username + "/" + password;
                return cli.DownloadString(host1);
            });
        }

        public Task<List<UserInfo>> GetRating(string host)
        {
            return Task.Run(() =>
            {
                var cli = new WebClient();
                string host1 = host + userApi ;
                string s = cli.DownloadString(host1);
                var inf = ParseArr(s);
                List<UserInfo> u = new List<UserInfo>();
                foreach(var i in inf)
                {
                    UserInfo user = new UserInfo(ParseJson(i));
                    u.Add(user);
                }
                return u;
            });
        }

        public async Task<string> GetColorPlayer(GameInfo info)
        {
            UserInfo userInfo = await GetUser();
            if(userInfo.id == info.White)
            {
                return "white";
            }
            else
            {
                return "black";
            }
        }

        public Task<GameInfo> GetCurrentGame()
        {
            return Task.Run(() =>
            {
                string response = CallServer(gameApi);

                if (response == "null")
                {
                    throw new Exception("Игра закончилась. Сожалеем(");
                }
                GameInfo game = new GameInfo(ParseJson(response));
                CurrentGameID = game.id;
                return game;
            }); 
        }

        public Task<GameInfo> ConnectToGame(string toUser)
        {
            return Task.Run(() =>
            {
                string response = CallServer(connectApi, "/" + toUser);

                if (response == "null")
                {
                    throw new Exception("Не можем подключится к игре. Сожалеем(");
                }
                GameInfo game = new GameInfo(ParseJson(response));
                CurrentGameID = game.id;
                return game;
            });
        }

        public Task<List<GameInfo>> GetWaitingGames(string host)
        {
            return Task.Run(() =>
            {
                var cli = new WebClient();
                string host1 = host + connectApi;
                string s = cli.DownloadString(host1);
                var inf = ParseArr(s);
                List<GameInfo> u = new List<GameInfo>();
                foreach (var i in inf)
                {
                    GameInfo user = new GameInfo(ParseJson(i));
                    u.Add(user);
                }
                return u;
            });
        }

        public Task<GameInfo> CreateOnce()
        {
            return Task.Run(() =>
            {
                string response = CallServer(createOnce);

                if (response == "null")
                {
                    throw new Exception("Игра уже была создана. Сожалеем(");
                }
                GameInfo game = new GameInfo(ParseJson(response));
                CurrentGameID = game.id;
                return game;
            });
        }

        public Task<UserInfo> GetUser()
        {
            return Task.Run(() =>
            {
                string response = CallServer(userApi);
                UserInfo user = new UserInfo(ParseJson(response));
                return user;
            });
        }

        public Task<UserInfo> GetUserById(int id)
        {
            return Task.Run(() =>
            {
                var cli = new WebClient();
                string host1 = host + getUserApi + id;
                string s = cli.DownloadString(host1);
                var inf = new UserInfo(ParseJson(s));
                return inf;
            });
        }


        public Task<GameInfo> CreateGame()
        {
            return Task.Run(() =>
            {
                string response = CallServer(createGameApi);

                if(response == "null")
                {
                    throw new Exception("Игра уже была создана");
                }
                GameInfo game = new GameInfo(ParseJson(response));
                CurrentGameID = game.id;
                return game;
            });
        }

        public Task<GameInfo> SendMove(string move)
        {
            return Task.Run(() =>
            {
                string response = CallServer(gameApi, "/" + move);
                GameInfo info;
                try
                {
                    info = new GameInfo(ParseJson(response));
                }catch

                {
                    throw new Exception("");
                }
                
                return info;
            });
        }

        public Task<GameInfo> OfferDraw()
        {
            return Task.Run(() =>
            {
                string response = CallServer(OfferDrawApi);
                GameInfo info;
                try
                {
                    info = new GameInfo(ParseJson(response));
                }
                catch

                {
                    throw new Exception("");
                }

                return info;
            });
        }

        public Task<GameInfo> AcceptDraw()
        {
            return Task.Run(() =>
            {
                string response = CallServer(SetDrawApi);
                GameInfo info;
                try
                {
                    info = new GameInfo(ParseJson(response));
                }
                catch

                {
                    throw new Exception("");
                }

                return info;
            });
        }

        public Task<GameInfo> DeclineDraw()
        {
            return Task.Run(() =>
            {
                string response = CallServer(DeclineDrawApi);
                GameInfo info;
                try
                {
                    info = new GameInfo(ParseJson(response));
                }
                catch

                {
                    throw new Exception("");
                }

                return info;
            });
        }

        private string CallServer(string api, string param = "")
        {
            WebRequest request = WebRequest.Create(host + api + user + param);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string temp = reader.ReadToEnd();
                return temp;
            }
                
        }

        private NameValueCollection ParseJson (string json)
        {
            NameValueCollection list = new NameValueCollection();

            string pattern = @"""(\w+)\"":""?([^,""}]*)""?";
            foreach (Match m in Regex.Matches(json, pattern))
                if (m.Groups.Count == 3)
                    list[m.Groups[1].Value] = m.Groups[2].Value;
            return list;
        }

        private List<string> ParseArr (string json)
        {
            List<string> l = new List<string>();
            string pattern = @"({""\w+"":""?[^}]*})";
            foreach (Match m in Regex.Matches(json, pattern))
            {
                l.Add(m.Groups[1].Value);
            }
            return l;
        }
    }
}
