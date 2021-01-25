using System;
using System.Collections.Generic;
using RAGE;
using RAGE.Ui;

namespace RAGEMP_TsVoiceClient
{
    public class TS_Connector : Events.Script
    {
        #region Variables
        private HtmlWindow tsBrowser;
        private string tsname;
		HtmlWindow gta5voice_ui_cef = null;
        #endregion

        #region Constructor
        public TS_Connector()
        {
			gta5voice_ui_cef = new HtmlWindow("package://GTA5_Voice/mic.html") {Active = true};
            RAGE.Events.Add("ConnectTeamspeak", ConnectTeamspeak);
            RAGE.Events.Add("DisconnectTeamspeak", DisconnectTeamspeak);
            RAGE.Events.Add("Teamspeak_LipSync", Teamspeak_LipSync);           
        }
        #endregion

        #region Events
        private void ConnectTeamspeak(object[] args)
        {
            tsname = args[0].ToString();

            Events.Tick += OnTick;
        }

        private void DisconnectTeamspeak(object[] args)
        {
            tsBrowser.Destroy();
        }

        private void Teamspeak_LipSync(object[] args)
        {
            RAGE.Elements.Player player = RAGE.Elements.Entities.Players.GetAtRemote(Convert.ToUInt16(args[0]));
            if (player != null)
            {
                bool speak = Convert.ToBoolean(args[1].ToString());

                if (speak)
                    player.PlayFacialAnim("mic_chatter", "mp_facial");
                else
                    player.PlayFacialAnim("mood_normal_1", "facials@gen_male@variations@normal");
            }
        }

        DateTime oldDateTime = DateTime.Now.AddMilliseconds(500);
        private void OnTick(List<Events.TickNametagData> nametags)
        {
            if (DateTime.Now > oldDateTime)
            {
                
                var player = RAGE.Elements.Player.LocalPlayer;
                var rotation = Math.PI / 180 * (player.GetRotation(0).Z * -1);
                var streamedPlayers = RAGE.Elements.Entities.Players.All;
                var playerNames = new List<string>();
                
                if (player.GetSharedData("CALLING_PLAYER_NAME") != null && player.GetSharedData("CALL_IS_STARTED") != null && player.GetSharedData("CALL_IS_STARTED").ToString() == "1")
                {
                    var callingPlayerName = player.GetSharedData("CALLING_PLAYER_NAME").ToString();
                    if (callingPlayerName != "")
                        playerNames.Add(callingPlayerName + "~10~0~0~3");
                }
                
                for (var i = 0; i < streamedPlayers.Count; i++)
                {
                    if (streamedPlayers[i] != null && !streamedPlayers[i].Exists)
                        continue;

                    if (streamedPlayers[i].GetSharedData("TsName") == null)
                        continue;

                    var streamedPlayerPos = streamedPlayers[i].Position;
                    var distance = player.Position.DistanceTo2D(streamedPlayerPos);

                    var voiceRange = "Normal";
                    if (streamedPlayers[i].GetSharedData("VOICE_RANGE") != null)
                        voiceRange = streamedPlayers[i].GetSharedData("VOICE_RANGE").ToString();

                    float volumeModifier = 0;
                    var range = 12;

                    if (voiceRange == "Weit")
                    {
                        range = 50;
                        gta5voice_ui_cef.ExecuteJs("(document.getElementById('voiceColor')).style.backgroundColor = 'rgba(255, 0, 0, 0.72)';");
                    }
                    else if (voiceRange == "Normal")
                    {
                        range = 15;
                        gta5voice_ui_cef.ExecuteJs("(document.getElementById('voiceColor')).style.backgroundColor = 'rgba(255, 0, 0, 0.39)';");
                    }
                    else if (voiceRange == "Kurz")
                    {
                        gta5voice_ui_cef.ExecuteJs("(document.getElementById('voiceColor')).style.backgroundColor = 'rgba(255, 0, 0, 0.21)';");
                        range = 5;
                    }
                    else if (voiceRange == "Stumm")
                    {
                        gta5voice_ui_cef.ExecuteJs("(document.getElementById('voiceColor')).style.backgroundColor = 'rgba(255, 0, 0, 0)';");
                        range = 0;
                    }

                    if (distance > 5)
                        volumeModifier = (distance * -8 / 10);

                    if (volumeModifier > 0)
                        volumeModifier = 0;

                    if (distance < range)
                    {
                        var subPos = streamedPlayerPos.Subtract(player.Position);

                        var x = subPos.X * Math.Cos(rotation) - subPos.Y * Math.Sin(rotation);
                        var y = subPos.X * Math.Sin(rotation) + subPos.Y * Math.Cos(rotation);

                        x = x * 10 / range;
                        y = y * 10 / range;

                        playerNames.Add(streamedPlayers[i].GetSharedData("TsName").ToString() + "~" + (Math.Round(x * 1000) / 1000) + "~" + (Math.Round(y * 1000) / 1000) + "~0~" + (Math.Round(volumeModifier * 1000) / 1000));
                    }
                }

                if (tsBrowser == null)
                    tsBrowser = new HtmlWindow($"http://localhost:15555/players/{tsname}/{string.Join(";", playerNames)}/");
                else
                    tsBrowser.Url = $"http://localhost:15555/players/{tsname}/{string.Join(";", playerNames)}/";

                oldDateTime = DateTime.Now.AddMilliseconds(500);
            }
        }
        #endregion
        #region Methods
        #endregion
    }
}
