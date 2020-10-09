namespace RayStudio.UtilScripts.Task
{
    public class DelaySecondsTask : Task
    {
        private float _seconds;

        public DelaySecondsTask SetDelaySeconds(float seconds)
        {
            _seconds = seconds;
            return this;
        }

        protected override void _InnerOnStart()
        {
            base._InnerOnStart();

            if (_seconds <= 0)
                Finish();
        }

        protected override void _InnerOnUpdate(float deltaTime)
        {
            base._InnerOnUpdate(deltaTime);

            _seconds -= deltaTime;
            if (_seconds <= 0)
                Finish();
        }
    }
}