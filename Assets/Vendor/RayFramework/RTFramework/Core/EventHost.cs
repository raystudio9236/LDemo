using RayStudio.UtilScripts.Event;
using UnityEngine;

namespace RayStudio.Framework.Core
{
    public class EventHost : MonoBehaviour
    {
        public EventDispatcher Dispatcher = new EventDispatcher();

        public void AddEventHandler(short type, EventHandler handler)
        {
            Dispatcher.AddEventHandler(type, handler);
        }

        public void RemoveEventHandler(short type, EventHandler handler)
        {
            Dispatcher.RemoveEventHandler(type, handler);
        }

        public void AddEventHandler<T>(short type, EventHandler<T> handler)
        {
            Dispatcher.AddEventHandler(type, handler);
        }

        public void RemoveEventHandler<T>(short type, EventHandler<T> handler)
        {
            Dispatcher.RemoveEventHandler(type, handler);
        }

        public void SendEvent(short type)
        {
            Dispatcher.SendEvent(type);
        }

        public void SendEvent<T>(short type, T msg)
        {
            Dispatcher.SendEvent(type, msg);
        }

        public EventDispatcher.EventHandlerListData GetEventHandlerList(short type)
        {
            return Dispatcher.GetEventHandlerList(type);
        }

        public void Clear()
        {
            Dispatcher.Clear();
        }

        public void PresizeHandler(short type, int size)
        {
            Dispatcher.PresizeHandler(type, size);
        }

        public void ClearNullEventHandler()
        {
            Dispatcher.ClearNullEventHandler();
        }
    }
}