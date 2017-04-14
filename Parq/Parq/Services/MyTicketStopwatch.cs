#region Copyright
/*Copyright (c) 2016 Javus Software (Pty) Ltd

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion
using System;
using System.Collections.Generic;

namespace Parq.Services
{
    public class MyTicketStopwatch
    {
        public DateTime date;
        public interface GetTime
        {
            long now();
        }

        private DateTime SystemTime = System.DateTime.Now;

        public enum State { PAUSED, RUNNING};

        private DateTime m_time;
        private long m_startTime;
        private long m_stopTime;
        private long m_pauseOffset;
        private List<long> m_laps = new List<long>();
        private State m_state;
        private long ticketElapsedTime;

        public MyTicketStopwatch(long ticketElapsedTime)
        {
            this.ticketElapsedTime = ticketElapsedTime;
            m_time = SystemTime;
            reset();
        }

        public MyTicketStopwatch(DateTime time)
        {
            m_time = time;
            reset();
        }

        public void start(long ticketElapsedTime)
        {
            if(m_state == State.PAUSED)
            {
                m_pauseOffset = getElapsedTime();
                m_stopTime = 0;
                m_startTime = m_time.Millisecond;
                m_state = State.RUNNING;
            }
        }

        public void pause()
        {
            if(m_state == State.RUNNING)
            {
                m_stopTime = m_time.Millisecond;
                m_state = State.PAUSED;
            }
        }

        public void reset()
        {
            m_state = State.PAUSED;
            m_startTime = 0;
            m_stopTime = 0;
            m_pauseOffset = ticketElapsedTime;
            m_laps.Clear();
        }

        public void lap()
        {
            m_laps.Add(getElapsedTime());
        }

        public long getElapsedTime()
        {
            if(m_state == State.PAUSED)
            {
                return (m_stopTime - m_startTime) + m_pauseOffset;
            }else
            {
                return (m_time.Millisecond - m_startTime) + m_pauseOffset;
            }
        }

        public List<long> getLaps()
        {
            return m_laps;
        }

        public bool isRunning()
        {
            return (m_state == State.RUNNING);
        }
    }
}