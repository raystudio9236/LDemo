using System.Collections.Generic;
using RayStudio.UtilScripts.Singleton;
using UnityEngine;

namespace RayStudio.UtilScripts.Task
{
    public class TaskManager : MonoSingleton<TaskManager>
    {
        private static uint _id = 0;

        private bool _hasInit = false;
        private List<Task> _tasks = new List<Task>();

        #region LifeCycle

        protected override void InitSingleton()
        {
            if (_hasInit) return;
            _hasInit = true;
            OnInit();
        }

        protected override void FreeSingleton()
        {
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            int lastIndex = _tasks.Count - 1;
            for (int i = lastIndex; i >= 0; i--)
            {
                var task = _tasks[i];
                if (task == null || task.IsOver)
                {
                    _tasks[i] = _tasks[lastIndex];
                    _tasks.RemoveAt(lastIndex);
                    lastIndex--;
                    continue;
                }

                task.Update(deltaTime);
            }
        }

        #endregion

        #region API

        public static uint AllocateId()
        {
            return ++_id;
        }

        public T CreateTask<T>() where T : Task, new()
        {
            T t = new T();
            return t;
        }

        public void RegisterTask(Task task)
        {
            _tasks.Add(task);
        }

        public void UnRegisterTask(Task task)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].ID == task.ID)
                {
                    _tasks.RemoveAt(i);
                    return;
                }
            }
        }

        public void StopTask(uint id)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].ID == id)
                {
                    _tasks[i].Stop();
                    return;
                }
            }
        }

        public void PauseTask(uint id)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].ID == id)
                {
                    _tasks[i].Pause();
                    return;
                }
            }
        }

        public void ResumeTask(uint id)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].ID == id)
                {
                    _tasks[i].Resume();
                    return;
                }
            }
        }

        #endregion

        #region Private

        private void OnInit()
        {
            RDebug.Log("[TaskManager] Init");
        }

        #endregion
    }
}