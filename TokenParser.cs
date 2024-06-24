using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Discord_HoneyPot
{
    // Handles different methods of populating the Discord Bot token used for StoveTop.

    public record TokenFile
    {
        public string token { get; set; }
        public string server { get; set; }
    }

    internal class TokenParser
    {
        // Local file
        public TokenFile tokenFile;

        public bool TryParseTokenFile()
        {
            if (!File.Exists("token.json")) 
            {
                Console.WriteLine("[X] Could not find token.json");
                return false; 
            }

            var tokenStuff = File.ReadAllText("token.json");
            tokenFile = JsonSerializer.Deserialize<TokenFile>(tokenStuff);
            return true;
        }

    }
}
