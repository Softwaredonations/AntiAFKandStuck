using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media;

using Styx;
using Styx.Helpers;
using Styx.Plugins;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace AntiAFK
{
    public class AntiAFK : HBPlugin
    {
        public override string Name { get { return "Anti AFK + Stuck"; } }
        public override string Author { get { return "Rozman + Hackernoob"; } }
        public override Version Version { get { return new Version(1, 1); } }
        public override bool WantButton { get { return false; } }

        private readonly List<DateTime> Stuck = new List<DateTime>();
        private static readonly Stopwatch timerAccept = new Stopwatch();
        private int AcceptingTime = 0;

        public override void OnEnable()
        {
            base.OnEnable();

            Logging.Write("[Anti AFK  + Stuck" + Version + "] Enabled");
            Logging.OnLogMessage += Logging_OnLogMessage;
        }

        public override void OnDisable()
        {
            Logging.Write("[Anti AFK " + Version + "] Disabled");

            timerAccept.Stop();
            timerAccept.Reset();
            AcceptingTime = 0;

            base.OnDisable();
        }

        public override void Pulse()
        {
            if (!StyxWoW.IsInGame) return;

			if (!timerAccept.IsRunning) {
                Random r = new Random();

                AcceptingTime = r.Next(240000, 420000);
                //AcceptingTime = r.Next(10000, 20000);

			    timerAccept.Start();
			}

			if (timerAccept.ElapsedMilliseconds < AcceptingTime)
			{
				return;
			}

            WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend, TimeSpan.FromSeconds(1));

            timerAccept.Stop();
            timerAccept.Reset();
            AcceptingTime = 0;
        }


        public void Logging_OnLogMessage(System.Collections.ObjectModel.ReadOnlyCollection<Logging.LogMessage> messages)
        {
            foreach (Logging.LogMessage logMessage in messages)
            {
                ParseLogMessage(logMessage.Message);
            }

        }

        public static void WriteLog(string format, params object[] args)
        {
            Logging.Write("[AFK AntiStuck!]: " + format, args);
        }


        public void ParseLogMessage(string message)
        {
    
                if (message.Contains("We are stuck!"))
                {
                    Stuck.Add(DateTime.Now);
                    WriteLog("Stuck ({0}/10 in the latest 5 minutes)", Stuck.Count);
                }

                Stuck.RemoveAll(er => er.AddMinutes(5) < DateTime.Now);
                if (Stuck.Count >= 10)
                {
       
                 
                    Lua.DoString("ForceQuit()");
                   /*
                    int intProcessId = StyxWoW.Memory.Process.Id;
                    int intTickCount = Environment.TickCount;
                    while (Environment.TickCount - intTickCount < 7000 && Process.GetProcessById(intProcessId) != null)
                        Thread.Sleep(200);
                    if (Process.GetProcessById(intProcessId) != null)
                        Process.GetProcessById(intProcessId).Kill();*/
            }
            
        }


    }
}