using System;
using System.Collections.Generic;
using System.Linq;
using RAGE;

namespace RAGEMP_TsVoiceClient
{
    internal class KeyHandler : Events.Script
    {
        internal Dictionary<ConsoleKey, bool> KeyList = new Dictionary<ConsoleKey, bool>();

        internal delegate void EventKeyDelegate(ConsoleKey key);
        internal EventKeyDelegate KeyEvent { get; set; }


        internal KeyHandler()
        {
            foreach (ConsoleKey key in (ConsoleKey[])Enum.GetValues(typeof(ConsoleKey)))
                KeyList.Add(key, false);
            Events.Tick += OnUpdate;
            KeyEvent += OnKeyPressed;
        }

        private void OnKeyPressed(ConsoleKey key)
        {
            if (RAGE.Game.Ui.IsPauseMenuActive())
                return;

            switch (key)
            {
                case ConsoleKey.F10:
                    Events.CallRemote("ChangeVoiceRange");
                    break;

                    // if you want more KeyBind
            }
        }

        private void OnUpdate(List<Events.TickNametagData> nametags)
        {
            foreach (KeyValuePair<ConsoleKey, bool> key in KeyList.ToList())
            {
                if (KeyList[key.Key] && !Input.IsDown((int)key.Key))
                {
                    KeyList[key.Key] = false;
                }
                else if (!KeyList[key.Key] && RAGE.Input.IsDown((int)key.Key))
                {
                    KeyEvent?.Invoke(key.Key);
                    KeyList[key.Key] = true;
                }
            }
        }
    }
}
