using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient
{
    public struct GameInfo
    {
        public int id;
        public string FEN;
        public string Statuse;
        public int White;
        public int? Black;
        public string ColorMove;

       public GameInfo (NameValueCollection list)
       {
            id = int.Parse(list["id"]);
            FEN = list["FEN"];
            Statuse = list["Statuse"];
            White = int.Parse(list["White"]);
            var j = list["Black"];
            if(list["Black"]!= "null")
                Black = int.Parse(list["Black"]);
            else
                Black = null;
            ColorMove = list["ColorMove"];
       }

        public override string ToString() =>
            "id = " + id +
            "\nFEN = " + FEN +
            "\nStatuse = " + Statuse +
            "\nWhite = " + White +
            "\nBlack = " + Black +
            "\nColorMove = " + ColorMove;
    }
}
