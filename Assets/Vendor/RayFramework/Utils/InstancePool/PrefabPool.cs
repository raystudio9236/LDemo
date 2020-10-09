using System;
using System.Collections.Generic;
using RayStudio.UtilScripts.Extension;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RayStudio.UtilScripts.Pool
{
    [Serializable]
    public class PrefabPool
    {
        public Transform SpawnRoot; // 实例化时放的根节点
        public Transform PoolRoot; // 回池时放的根节点
        public GameObject Prefab;

        private readonly Stack<GameObject> _pool = new Stack<GameObject>();
        private readonly HashSet<GameObject> _outPoolSet = new HashSet<GameObject>();

        private bool _cloned = false;

        public void PreLoad(int count)
        {
            if (count <= 0) return;
            var needLoadCount = count - _pool.Count - _outPoolSet.Count;
            if (needLoadCount <= 0) return;

            for (var i = 0; i < needLoadCount; i++)
            {
                var go = Object.Instantiate(Prefab, PoolRoot, true);
                go.SetActive(false);
                _pool.Push(go);
            }
        }

        public GameObject Spawn()
        {
            GameObject go;
            if (_pool.Count <= 0)
                go = Object.Instantiate(Prefab);
            else
                go = _pool.Pop();

            _outPoolSet.Add(go);
            go.transform.SetParent(SpawnRoot);
            return go;
        }

        public bool Recycle(GameObject go)
        {
            if (_outPoolSet.Contains(go))
            {
                go.gameObject.SetActive(false);
                go.transform.SetParent(PoolRoot);
                _pool.Push(go);
                _outPoolSet.Remove(go);
                return true;
            }

            return false;
        }

        public void RecycleAll()
        {
            var items = new GameObject[_outPoolSet.Count];
            var idx = 0;
            foreach (var item in _outPoolSet)
                items[idx++] = item;

            foreach (var item in items)
            {
                Recycle(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _pool)
                Object.Destroy(item);

            foreach (var item in _outPoolSet)
                Object.Destroy(item);

            _pool.Clear();
            _outPoolSet.Clear();
        }

        public void AddComponent<Y>() where Y : Component
        {
            Clone();

            Prefab.AddComponent<Y>();

            foreach (var obj in _pool)
                obj.gameObject.AddComponent<Y>();

            foreach (var obj in _outPoolSet)
                obj.gameObject.AddComponent<Y>();
        }

        public void RemoveComponent<Y>() where Y : Component
        {
            Clone();

            Prefab.RemoveComponent<Y>();

            foreach (var obj in _pool)
                obj.gameObject.RemoveComponent<Y>();

            foreach (var obj in _outPoolSet)
                obj.gameObject.RemoveComponent<Y>();
        }


        public void AddComponent(Type compType)
        {
            Clone();

            Prefab.AddComponent(compType);

            foreach (var obj in _pool)
                obj.AddComponent(compType);

            foreach (var obj in _outPoolSet)
                obj.AddComponent(compType);
        }

        public void RemoveComponent(Type compType)
        {
            Clone();

            Prefab.RemoveComponent(compType);

            foreach (var obj in _pool)
                obj.RemoveComponent(compType);

            foreach (var obj in _outPoolSet)
                obj.RemoveComponent(compType);
        }

        private void Clone()
        {
            if (_cloned) return;
            _cloned = true;

            Prefab = GameObject.Instantiate(Prefab, PoolRoot);
            Prefab.SetActive(false);
        }
    }

    [Serializable]
    public class PrefabPool<T> where T : Component
    {
        public Transform SpawnRoot;
        public Transform PoolRoot;
        public GameObject Prefab;

        private bool _cloned = false;

        private readonly Stack<T> _pool = new Stack<T>();
        private readonly HashSet<T> _outPoolSet = new HashSet<T>();

        public void PreLoad(int count)
        {
            if (count <= 0) return;
            var needLoadCount = count - _pool.Count - _outPoolSet.Count;
            if (needLoadCount <= 0) return;

            for (var i = 0; i < needLoadCount; i++)
            {
                var go = Object.Instantiate(Prefab, PoolRoot, true);
                go.SetActive(false);
                var comp = go.GetComponent<T>();
                _pool.Push(comp);
            }
        }

        public T Spawn()
        {
            T comp;
            if (_pool.Count <= 0)
            {
                var go = Object.Instantiate(Prefab);
                comp = go.GetComponent<T>();
            }
            else
            {
                comp = _pool.Pop();
            }

            _outPoolSet.Add(comp);
            comp.transform.SetParent(SpawnRoot);
            return comp;
        }

        public bool Recycle(T comp)
        {
            if (_outPoolSet.Contains(comp))
            {
                comp.gameObject.SetActive(false);
                comp.transform.SetParent(PoolRoot);
                _pool.Push(comp);
                _outPoolSet.Remove(comp);
                return true;
            }

            return false;
        }

        public void RecycleAll()
        {
            var items = new T[_outPoolSet.Count];
            var idx = 0;
            foreach (var item in _outPoolSet)
                items[idx++] = item;

            foreach (var item in items)
            {
                Recycle(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _pool)
                Object.Destroy(item.gameObject);

            foreach (var item in _outPoolSet)
                Object.Destroy(item.gameObject);

            _pool.Clear();
            _outPoolSet.Clear();
        }

        public void AddComponent<Y>() where Y : Component
        {
            Clone();

            Prefab.AddComponent<Y>();

            foreach (var item in _pool)
                item.gameObject.AddComponent<Y>();

            foreach (var item in _outPoolSet)
                item.gameObject.AddComponent<Y>();
        }

        public void RemoveComponent<Y>() where Y : Component
        {
            if (typeof(Y) == typeof(T))
            {
                throw new Exception($"PrefabPool<{typeof(T)}> RemoveComponent Error, Remove component type cannot be same with generic type");
            }

            Clone();

            Prefab.RemoveComponent<Y>();

            foreach (var item in _pool)
                item.gameObject.RemoveComponent<Y>();

            foreach (var item in _outPoolSet)
                item.gameObject.RemoveComponent<Y>();
        }


        public void AddComponent(Type compType)
        {
            Clone();

            Prefab.AddComponent(compType);

            foreach (var item in _pool)
                item.gameObject.AddComponent(compType);

            foreach (var item in _outPoolSet)
                item.gameObject.AddComponent(compType);
        }

        public void RemoveComponent(Type compType)
        {
            if (compType == typeof(T))
            {
                throw new Exception($"PrefabPool<{typeof(T)}> RemoveComponent Error, Remove component type cannot be same with generic type");
            }

            Clone();

            Prefab.RemoveComponent(compType);

            foreach (var item in _pool)
                item.gameObject.RemoveComponent(compType);

            foreach (var item in _outPoolSet)
                item.gameObject.RemoveComponent(compType);
        }

        private void Clone()
        {
            if (_cloned) return;
            _cloned = true;

            Prefab = GameObject.Instantiate(Prefab, PoolRoot);
            Prefab.SetActive(false);
        }
    }
}