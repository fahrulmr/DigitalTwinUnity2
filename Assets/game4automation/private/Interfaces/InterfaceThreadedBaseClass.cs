// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using System.Threading;
using System;

namespace game4automation
{
    public class InterfaceThreadedBaseClass : InterfaceBaseClass
    {
        public int MinCommCycleMs = 0;
        [ReadOnly] public int CommCycleNr;
        [ReadOnly] public int CommCycleMs;
        [ReadOnly] public string ThreadStatus; 
        private Thread CommThread;
        private DateTime ThreadTime;
        private bool run;
        
        protected virtual void CommunicationThreadUpdate()
        {
        }
        
        
        protected virtual void CommunicationThreadClose()
        {
        }
        
        public override void OpenInterface()
        {
            ThreadStatus = "running";
            CommThread = new Thread(CommunicationThread);
            run = true;
            CommThread.Start();
        }
        
        public override void CloseInterface()
        {
            run = false;
            if (CommThread!=null)
                   CommThread.Abort();
        }
        
        void CommunicationThread()
        {
            DateTime start,end;
            do
            {
                start = DateTime.Now;
                CommunicationThreadUpdate();
                ThreadTime = start;
                CommCycleNr++;
                end = DateTime.Now;
                TimeSpan span = end - start;
                CommCycleMs = (int) span.TotalMilliseconds;
                if (MinCommCycleMs-CommCycleMs>0)
                    Thread.Sleep(MinCommCycleMs-CommCycleMs);
            } while (run == true);
            CommunicationThreadClose();
            
        }

    }
}