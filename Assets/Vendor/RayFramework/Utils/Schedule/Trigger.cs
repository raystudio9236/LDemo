using System;
using System.Collections.Generic;

namespace RayStudio.UtilScripts.Schedule
{
    public class Trigger
    {
        private static uint _id = 0;
        private static uint id => _id++;

        private readonly List<TimerData> _delayList = new List<TimerData>(1);

        public uint DelaySeconds(float seconds, Action<string> cb, string param = "")
        {
            var data = new TimerData {Id = id, Seconds = seconds, Cb = cb, Param = param};
            _delayList.Add(data);
            return data.Id;
        }

        public uint DelayFrame(int frame, Action<string> cb, string param = "")
        {
            var data = new TimerData {Id = id, Frame = frame, Cb = cb, Param = param};
            _delayList.Add(data);
            return data.Id;
        }

        public void Cancel(uint id)
        {
            foreach (var delayData in _delayList)
            {
                if (delayData.Id == id)
                {
                    delayData.Cancel();
                    break;
                }
            }
        }

        public void CancelAll()
        {
            foreach (var delayData in _delayList)
            {
                delayData.Cancel();
            }
        }

        public void Tick(float delta)
        {
            if (_delayList.Count <= 0)
                return;

            for (var i = _delayList.Count - 1; i >= 0; i--)
            {
                var ret = _delayList[i].OnTick(delta);
                if (ret)
                {
                    _delayList.RemoveAt(i);
                }
            }
        }

        private class TimerData
        {
            public uint Id;
            public float Seconds;
            public int Frame;
            public string Param;
            public Action<string> Cb;

            public void Cancel()
            {
                Seconds = 0f;
                Frame = 0;
                Cb = null;
            }

            public bool OnTick(float delta)
            {
                var finish = false;
                if (Seconds > 0)
                {
                    Seconds -= delta;
                    if (Seconds <= 0)
                        finish = true;
                }
                else if (Frame > 0)
                {
                    Frame--;
                    if (Frame <= 0)
                        finish = true;
                }
                else
                {
                    finish = true;
                }

                if (finish)
                {
                    Cb?.Invoke(Param);
                    return true;
                }

                return false;
            }
        }
    }
}