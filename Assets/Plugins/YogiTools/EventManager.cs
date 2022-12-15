using System;
using System.Collections.Generic;

namespace YogiTools
{
    #region 骚操作,用于躲避拆箱装箱

    public interface IEventInfo
    {
    }

    public class EventInfo<T> : IEventInfo
    {
        public List<Action<T>> action;

        public EventInfo(Action<T> _action)
        {
            action = new List<Action<T>>() { _action };
        }

        public void Add(Action<T> act)
        {
            action.Add(act);
        }

        public void AddToTop(Action<T> act)
        {
            action.Insert(0, act);
        }

        public void Remove(Action<T> act)
        {
            action.Remove(act);
        }

        public void InvokeBySequence(T obj)
        {
            for (var i = 0; i < this.action.Count; i++)
            {
                this.action[i].Invoke(obj);
            }
        }
    }

    public class EventInfo : IEventInfo
    {
        public Action action;

        public EventInfo(Action _action)
        {
            action = _action;
        }
    }

    #endregion

    /// <summary>
    /// 事件管理器, 用于总控事件(观察者设计模式)
    /// 使用方法: 监听事件记得添加移出监听接口 触发事件即可
    /// </summary>
    public class EventManager
    {
        public static EventManager Instance
        {
            get
            {

                if (_Instance == null)
                {
                    _Instance = new EventManager();
                }

                return _Instance;
            }
        } 
        public static EventManager _Instance; 
        public EventManager()
        {
        }

        private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

        #region 无参事件

        public void AddEventListener(string eventName, Action action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                try
                {
                    (eventDic[eventName] as EventInfo).action += action; // 父类强转子类 然后就可以不用object装箱拆箱
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(string.Format("EventError:[{0}]Exception:[{1}]", eventName, e));
                }
            }
            else
            {
                eventDic.Add(eventName, new EventInfo(action));
            }
        }

        public void RemoveEventListener(string eventName, Action action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo).action -= action;
            }
        }

        public void EventTrigger(string eventName)
        {
            if (eventDic.ContainsKey(eventName))
            {
                try
                {
                    Action v = (eventDic[eventName] as EventInfo).action;
                    if (v != null)
                    {
                        v.Invoke();
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(string.Format("EventError:{0} Exception:{1}", eventName, e));
                }
            }
        }

        #endregion

        #region 单个传递参数事件

        public void AddEventListener<T>(string eventName, Action<T> action, bool isAddToTop = false)
        {
            if (eventDic.ContainsKey(eventName))
            {
                try
                {
                    //注: 不能同一个事件名 不同类型的参数
                    if (isAddToTop)
                    {
                        (eventDic[eventName] as EventInfo<T>).AddToTop(action); // 父类强转子类 然后就可以不用object装箱拆箱
                    }
                    else
                    {
                        (eventDic[eventName] as EventInfo<T>).Add(action); // 父类强转子类 然后就可以不用object装箱拆箱
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(string.Format("Event Add Failed: Name:[{0}]:{e}", eventName, e));
                }
            }
            else
            {
                eventDic.Add(eventName, new EventInfo<T>(action));
            }
        }

        public void RemoveEventListener<T>(string eventName, Action<T> action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T>).Remove(action);
            }
        }

        public void EventTrigger<T>(string eventName, T message)
        {
            if (eventDic.ContainsKey(eventName))
            {
                try
                {
                    IEventInfo e = eventDic[eventName];
                    if (e != null)
                    {
                        if (e is EventInfo<T> eInfo)
                        {
                            eInfo.InvokeBySequence(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"EventError:[{eventName}] Exception:[{e}] Msg:[{message}]");
                }
            }
        }

        /// <summary>
        /// 一次性触发多个事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="eventNames"></param>
        public void MultiEventsTrigger<T>(T msg, params string[] eventNames)
        {
            for (int i = 0; i < eventNames.Length; i++)
            {
                EventTrigger<T>(eventNames[i], msg);
            }
        }

        #endregion

        public void Clear()
        {
            eventDic.Clear();
        }
    }
}