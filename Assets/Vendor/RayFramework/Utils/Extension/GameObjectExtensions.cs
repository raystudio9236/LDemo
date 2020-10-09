using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace RayStudio.UtilScripts.Extension
{
    public static class GameObjectUtil
    {
        public static void SetActiveSafe(this GameObject obj, bool value)
        {
            if (obj == null || obj.activeSelf == value) return;
            obj.SetActive(value);
        }

        public static void SetActiveSafe(this Transform tf, bool value)
        {
            if (tf == null || tf.gameObject.activeSelf == value) return;
            tf.gameObject.SetActive(value);
        }

        public static void ChangeLayersRecursively(this Transform trans, string name, string ignoreLayerName = "")
        {
            var ignoreLayer = LayerMask.NameToLayer(ignoreLayerName);
            if (trans.gameObject.layer != ignoreLayer)
                trans.gameObject.layer = LayerMask.NameToLayer(name);

            foreach (Transform child in trans)
            {
                child.ChangeLayersRecursively(name, ignoreLayerName);
            }
        }

        public static void ChangeLayersRecursively(this Transform trans, int layer, int ignoreLayer = -1)
        {
            if (trans.gameObject.layer != ignoreLayer)
                trans.gameObject.layer = layer;

            foreach (Transform child in trans)
            {
                child.ChangeLayersRecursively(layer, ignoreLayer);
            }
        }

        public static void ChangeDefaultLayersRecursively(this Transform trans, int layer)
        {
            if (trans.gameObject.layer == 0)
                trans.gameObject.layer = layer;

            foreach (Transform child in trans)
            {
                child.ChangeDefaultLayersRecursively(layer);
            }
        }

        public static List<Transform> GetChildren(this Transform parent)
        {
            if (parent == null)
                return null;

            if (parent.childCount == 0)
                return null;

            var children = new List<Transform>();
            for (int i = 0, count = parent.childCount; i < count; i++)
            {
                children.Add(parent.GetChild(i));
            }

            return children;
        }

        public static T GetComponentInChildren<T>(GameObject go, bool inactiveObject = false) where T : Component
        {
            T ret = null;
            if (null == go) return null;
            if (false == inactiveObject)
            {
                return go.GetComponentInChildren<T>();
            }

            var arr = go.GetComponentsInChildren<T>(true);
            if (null != arr && 0 < arr.Length)
            {
                ret = arr[0];
            }

            return ret;
        }

        public static T GetTagComponent<T>(string tag, bool inactiveObject = false,
            bool multipleObjects = false) where T : Component
        {
            T ret = null;

            if (false == multipleObjects)
            {
                GameObject go = GameObject.FindWithTag(tag);
                if (null != go)
                {
                    ret = go.GetComponent<T>();
                }
            }
            else
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);

                if (null == gos) return null;
                foreach (var oneGo in gos)
                {
                    if (null == oneGo) continue;
                    Component oneCom = oneGo.GetComponent<T>();
                    if (null == oneCom) continue;
                    ret = (T) oneCom;
                    break;
                }
            }

            return ret;
        }

        public static T GetTagComponentInChildren<T>(string tag, bool inactiveObject = false,
            bool multipleObjects = false) where T : Component
        {
            T ret = null;

            if (false == multipleObjects)
            {
                GameObject go = GameObject.FindGameObjectWithTag(tag);

                ret = GetComponentInChildren<T>(go, inactiveObject);
            }
            else
            {
                var gos = GameObject.FindGameObjectsWithTag(tag);

                if (null == gos) return null;
                foreach (var oneGo in gos)
                {
                    if (null == oneGo) continue;

                    Component oneCom = GetComponentInChildren<T>(oneGo, inactiveObject);
                    if (null == oneCom) continue;

                    ret = (T) oneCom;
                    break;
                }
            }

            return ret;
        }

        public static T[] GetTagComponentsInChildren<T>(string tag, bool inactiveObject = false) where T : Component
        {
            var gos = GameObject.FindGameObjectsWithTag(tag);
            if (null == gos) return null;

            var tablist = new List<T>();

            foreach (var oneGo in gos)
            {
                if (null == oneGo) continue;

                var oneCom = GetComponentInChildren<T>(oneGo, inactiveObject);
                if (null != oneCom)
                {
                    tablist.Add(oneCom);
                }
            }

            var ret = tablist.ToArray();

            return ret;
        }

        public static List<GameObject> GetRootGameObjects()
        {
            return GetRootGameObjects(false);
        }

        public static List<GameObject> GetRootGameObjects(bool includeInactive)
        {
            var lstGo = new List<GameObject>();
            var arrTrans = false == includeInactive
                ? Object.FindObjectsOfType<Transform>()
                : Resources.FindObjectsOfTypeAll<Transform>();

            foreach (var t in arrTrans)
            {
                if (t.parent == null)
                {
                    lstGo.Add(t.gameObject);
                }
            }

            return lstGo;
        }

        public static List<GameObject> GetAllGameObjects()
        {
            return GetAllGameObjects(false);
        }

        public static List<GameObject> GetAllGameObjects(bool includeInactive)
        {
            var lstGo = new List<GameObject>();
            Transform[] arrTrans;
            arrTrans = false == includeInactive
                ? Object.FindObjectsOfType<Transform>()
                : Resources.FindObjectsOfTypeAll<Transform>();

            foreach (var t in arrTrans)
            {
                lstGo.Add(t.gameObject);
            }

            return lstGo;
        }

        public static T AddMissingComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }

        public static Component AddMissingComponent(this GameObject go, Type t)
        {
            var comp = go.GetComponent(t);
            if (comp == null)
            {
                comp = go.AddComponent(t);
            }

            return comp;
        }

        public static bool RemoveComponent<T>(this GameObject o) where T : Component
        {
            var comp = o.GetComponent<T>();
            if (comp == null)
                return false;

            Object.Destroy(comp);
            return true;
        }


        public static bool RemoveComponent(this GameObject o, Type compType)
        {
            var comp = o.GetComponent(compType);
            if (comp == null)
                return false;

            Object.Destroy(comp);
            return true;
        }

        public static GameObject FindGameObject(this GameObject obj, string name, bool includeInactive = true,
            bool showError = true)
        {
            var trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    return t.gameObject;
                }
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2} {3}, please check!", name, obj.GetType(),
                    obj, obj.name);

            return null;
        }

        public static void ResetLocal(this GameObject obj)
        {
            if (obj == null) return;
            obj.transform.ResetLocal();
        }

        public static void ResetLocal(this Transform tf)
        {
            if (tf == null) return;

            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
            tf.localRotation = Quaternion.identity;
        }

        public static void Reset(this GameObject obj)
        {
            if (obj == null) return;
            obj.transform.Reset();
        }

        public static void Reset(this Transform tf)
        {
            if (tf == null) return;

            tf.position = Vector3.zero;
            tf.localScale = Vector3.one;
            tf.rotation = Quaternion.identity;
        }

        #region Unity Editor

#if UNITY_EDITOR

        public static bool BindComponent<T>(this GameObject obj, out T component,
            string name, bool includeInactive = true, bool showError = true) where T : Component
        {
            component = null;

            T temp = null;

            Transform[] trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trs)
            {
                if (t.name == name)
                {
                    var c = t.GetComponent<T>();
                    if (c != null)
                    {
                        if (temp != null)
                        {
                            if (showError)
                                Debug.LogErrorFormat(
                                    "ERROR: There is more than 1 Component name of {0}, Please check it", name);

                            return false;
                        }

                        temp = c;
                    }
                    else
                    {
                        if (showError)
                            Debug.LogErrorFormat("ERROR: Cannot find {0} component in {1} Obj", typeof(T), name);
                    }
                }
            }

            if (temp != null)
            {
                component = temp;
                return true;
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2}, please check!", name, obj.GetType(),
                    obj);

            return false;
        }

        public static bool BindComponent(this GameObject obj, out GameObject target,
            string name, bool includeInactive = true, bool showError = true)
        {
            target = null;

            var trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    target = t.gameObject;
                    return true;
                }
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2}, please check!", name, obj.GetType(),
                    obj);

            return false;
        }

        public static bool BindComponent(this GameObject obj, out Transform target,
            string name, bool includeInactive = true, bool showError = true)
        {
            target = null;

            var trs = obj.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == name)
                {
                    target = t;
                    return true;
                }
            }

            if (showError)
                Debug.LogErrorFormat("ERROR: Cannot find {0}(type {1}) for {2}, please check!", name, obj.GetType(),
                    obj);

            return false;
        }

#endif

        #endregion
    }
}