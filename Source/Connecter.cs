using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAGEMP_TsVoice_Connecter
{
    public class Connecter : Script
    {
        public Connecter()
        {
            Console.WriteLine("Teamspeak Connecter Initialization...");
        }

		
		// Char Select Remote Event
        [RemoteEvent("authCharacter")]
        public void ChangeVoiceRange(Client player)
        {
            RAGEMP_TsVoice.Teamspeak.Connect(player, RAGEMP_TsVoice.Teamspeak.GetRandomString());
        }

    }
}