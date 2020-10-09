namespace RayStudio.UtilScripts.Task
{
    public class DelayFramesTask : Task
    {
        private uint _frames;

        public DelayFramesTask SetDelayFrames(uint frames)
        {
            _frames = frames;
            return this;
        }
    
        protected override void _InnerOnStart()
        {
            base._InnerOnStart();

            if (_frames == 0)
                Finish();
        }

        protected override void _InnerOnUpdate(float deltaTime)
        {
            base._InnerOnUpdate(deltaTime);

            _frames--;
            if (_frames == 0)
            {
                Finish();
            }
        }
    }
}