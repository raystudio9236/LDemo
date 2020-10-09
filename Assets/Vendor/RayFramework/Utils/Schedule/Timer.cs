namespace RayStudio.UtilScripts.Schedule
{
    public class Timer
    {
        private float _defaultTime;
        private float _timer;

        public Timer()
        {
        }

        public Timer(float defaultTime)
        {
            Init(defaultTime);
        }

        public void Init(float defaultTime, float startOffset = 0)
        {
            _defaultTime = defaultTime;
            _timer = startOffset;
        }

        public void Reset(bool toStart = true)
        {
            if (toStart)
                _timer = 0;
            else
                _timer = _defaultTime;
        }

        public bool IsReady(bool reset = true)
        {
            var ret = _timer <= 0;
            if (ret && reset)
                _timer = _defaultTime;
            return ret;
        }

        public bool Update(float dt, bool autoReset = false)
        {
            _timer -= dt;
            if (_timer <= 0)
            {
                if (autoReset)
                    Reset(false);

                return true;
            }

            return false;
        }
    }
}