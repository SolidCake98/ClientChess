using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessClient;

namespace ConsoleChess
{
    class Program
    {

        public const string HOST = "http://localhost:57286/api/";
        public const string USER = "Donil";

        static void Main(string[] args)
        {
            Program program = new Program();
            program.client = new ChessClient.ChessClient(HOST, USER);
            program.Start();
            Console.ReadKey();
        }

        ChessClient.ChessClient client;

        async void Start()
        {
            client = new ChessClient.ChessClient(HOST, USER);
            try
            {
                Task<GameInfo> task = client.AcceptDraw();
   
                task.Wait();
                var g = task.Result;
                Task<UserInfo> task1 = client.GetUserById(2);
                var c = task1.Result;
                Console.WriteLine(g.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Write("Your move: ");
            string move = Console.ReadLine();
            Console.WriteLine(client.SendMove(move));
        }
    }
}
