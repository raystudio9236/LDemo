namespace RayStudio.UtilScripts.Task
{
    public delegate void TaskHandler();
    public delegate void TaskUpdateHandler(float deltaTime);
    public delegate void TaskFinishHandler(bool isStopped);

    public class Task
    {
        enum TaskState
        {
            Init,
            Running,
            Paused,
            Stopped,
            Finished,
        }

        private event TaskHandler _onStartCb;
        private event TaskHandler _onPauseCb;
        private event TaskHandler _onResumeCb;
        private event TaskHandler _onStoppedCb;
        private event TaskUpdateHandler _onUpdateCb;
        private event TaskFinishHandler _onFinishedCb;

        private uint _id;
        public uint ID => _id;
        private TaskState _state;

        public bool IsRunning
        {
            get { return _state == TaskState.Running; }
        }

        public bool IsPaused
        {
            get { return _state == TaskState.Paused; }
        }

        public bool IsStopped
        {
            get { return _state == TaskState.Stopped; }
        }

        public bool IsOver
        {
            get
            {
                return _state == TaskState.Stopped
                    || _state == TaskState.Finished;
            }
        }

        #region API

        public Task()
        {
            _id = TaskManager.AllocateId();
            _state = TaskState.Init;
        }

        public Task Start()
        {
            if (_state != TaskState.Init)
            {
                RDebug.LogError("[TaskManager] 任务不能重复开始");
                return this;
            }

            if (!CheckState(TaskState.Running))
                return this;

            TaskManager.Instance.RegisterTask(this);
            _state = TaskState.Running;
            _InnerOnStart();

            return this;
        }

        public Task Pause()
        {
            if (!CheckState(TaskState.Paused))
                return this;
            
            _state = TaskState.Paused;
            _InnerOnPause();
            
            return this;
        }

        public Task Resume()
        {
            if (!CheckState(TaskState.Running))
                return this;

            _state = TaskState.Running;
            _InnerOnResume();

            return this;
        }

        public Task Stop()
        {
            if (!CheckState(TaskState.Stopped))
                return this;

            var lastState = _state;
            _state = TaskState.Stopped;

            _InnerOnStopped();
            if (lastState == TaskState.Running)
                _InnerOnFinished(false);

            TaskManager.Instance.UnRegisterTask(this);
            return this;
        }

        public Task Finish()
        {
            if (!CheckState(TaskState.Finished))
                return this;

            _state = TaskState.Finished;
            _InnerOnFinished(true);
            
            TaskManager.Instance.UnRegisterTask(this);

            return this;
        }

        public void Update(float deltaTime)
        {
            if (IsOver || IsPaused)
                return;

            _InnerOnUpdate(deltaTime);
        }

        #endregion

        #region Event

        public Task OnStart(TaskHandler cb)
        {
            _onStartCb += cb;
            return this;
        }

        public Task OnUpdate(TaskUpdateHandler cb)
        {
            _onUpdateCb += cb;
            return this;
        }

        public Task OnPause(TaskHandler cb)
        {
            _onPauseCb += cb;
            return this;
        }

        public Task OnResume(TaskHandler cb)
        {
            _onResumeCb += cb;
            return this;
        }

        public Task OnStopped(TaskHandler cb)
        {
            _onStoppedCb += cb;
            return this;
        }

        public Task OnFinished(TaskFinishHandler cb)
        {
            _onFinishedCb += cb;
            return this;
        }

        #endregion

        #region virtual

        protected virtual void _InnerOnStart()
        {
            if (_onStartCb != null)
                _onStartCb();
        }

        protected virtual void _InnerOnPause()
        {
            if (_onPauseCb != null)
                _onPauseCb();
        }

        protected virtual void _InnerOnResume()
        {
            if (_onResumeCb != null)
                _onResumeCb();
        }

        protected virtual void _InnerOnStopped()
        {
            if (_onStoppedCb != null)
                _onStoppedCb();
        }

        protected virtual void _InnerOnUpdate(float deltaTime)
        {
            if (_onUpdateCb != null)
                _onUpdateCb(deltaTime);
        }

        protected virtual void _InnerOnFinished(bool isStopped)
        {
            if (_onFinishedCb != null)
                _onFinishedCb(isStopped);
        }

        #endregion

        #region Private

        private bool CheckState(TaskState toState)
        {
            switch (toState)
            {
                case TaskState.Init:
                {
                    if (!IsOver)
                    {
                        RDebug.LogErrorFormat("[TaskManager] {0} -> {1} 当前状态不能切换到初始状态", _state, toState);
                        return false;
                    }
                    return true;
                }

                case TaskState.Running:
                {
                    if (_state != TaskState.Init
                        && _state != TaskState.Paused)
                        return false;
                    return true;
                }

                case TaskState.Paused:
                {
                    if (!IsRunning)
                        return false;
                    return true;
                }

                case TaskState.Stopped:
                {
                    if (_state == TaskState.Running
                        || _state == TaskState.Paused)
                        return true;
                    return false;
                }

                case TaskState.Finished:
                {
                    if (_state == TaskState.Init
                        || _state == TaskState.Running
                        || _state == TaskState.Paused
                        || _state == TaskState.Stopped)
                        return true;
                    return false;
                }
                
                default:
                    return false;
            }
        }

        #endregion
    }
}