using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ViralVial.Utilities
{
    /// <summary>
    /// Singleton-style class that can be used to trigger events as well as start or stop listening 
    /// to these events.
    /// </summary>
    public class EventManager
    {
        private Dictionary<string, Action<Dictionary<string, object>>> eventDictionaryArgs;
        private Dictionary<string, Dictionary<string, object>> pendingEventArgsArgs;

        private Dictionary<string, Action> eventDictionaryNoArgs;
        private HashSet<string> pendingEventsNoArgs;


        private bool logEvents = false;

        private static EventManager instance = null;
        public static EventManager Instance
        {
            get
            {
                if (instance == null) instance = new EventManager();
                return instance;
            }
        }

        private EventManager()
        {
            if (eventDictionaryArgs == null) eventDictionaryArgs = new Dictionary<string, Action<Dictionary<string, object>>>();
            if (pendingEventArgsArgs == null) pendingEventArgsArgs = new Dictionary<string, Dictionary<string, object>>();
            if (eventDictionaryNoArgs == null) eventDictionaryNoArgs = new Dictionary<string, Action>();
            if (pendingEventsNoArgs == null) pendingEventsNoArgs = new HashSet<string>();
        }

        // Events With Arguments ------------------------------------------------------------------

        /// <summary>
        /// Starts a function listening to an event. The function must take a Dictionary<string, object> 
        /// as its parameters. The dictionary will hold any parameters passed to the function when the 
        /// event is triggered. If the function's object is destroyed, remember to call StopListening down below.
        /// </summary>
        /// <param name="eventName">event to listen to</param>
        /// <param name="listener">function that will listen for event</param>
        public void SubscribeToEvent(string eventName, Action<Dictionary<string, object>> listener)
        {
            Action<Dictionary<string, object>> thisEvent;
            if (eventDictionaryArgs.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                eventDictionaryArgs[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionaryArgs.Add(eventName, thisEvent);
            }
        }

        /// <summary>
        /// Starts a function listening to an event. The function must take a Dictionary<string, object> 
        /// as its parameters. The dictionary will hold any parameters passed to the function when the 
        /// event is triggered. If the function's object is destroyed, remember to call StopListening down below.
        /// </summary>
        /// <param name="eventName">event to listen to</param>
        /// <param name="listener">function that will listen for event</param>
        public void SubscribeToEventAndPending(string eventName, Action<Dictionary<string, object>> listener)
        {
            Action<Dictionary<string, object>> thisEvent;
            if (eventDictionaryArgs.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                eventDictionaryArgs[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionaryArgs.Add(eventName, thisEvent);
            }

            if (pendingEventArgsArgs.ContainsKey(eventName))
            {
                listener.Invoke(pendingEventArgsArgs[eventName]);
                pendingEventArgsArgs.Remove(eventName);
            }
        }

        /// <summary>
        /// Stops a function listening to an event. This should be called before for any functions 
        /// belonging to an object that is about to be destroyed.
        /// </summary>
        /// <param name="eventName">event to stop listening to</param>
        /// <param name="listener">function that will stop listening for event</param>
        public void UnsubscribeFromEvent(string eventName, Action<Dictionary<string, object>> listener)
        {
            Action<Dictionary<string, object>> thisEvent;
            if (eventDictionaryArgs.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                eventDictionaryArgs[eventName] = thisEvent;
                if (eventDictionaryArgs[eventName] == null) eventDictionaryArgs.Remove(eventName);
            }
        }

        /// <summary>
        /// Invokes the event specified by event name.
        /// </summary>
        /// <param name="eventName">event to invoke</param>
        /// <param name="args">parameters passed to any function invoked by event</param>
        public void InvokeEvent(string eventName, Dictionary<string, object> args)
        {
            Action<Dictionary<string, object>> thisEvent;
            if (eventDictionaryArgs.TryGetValue(eventName, out thisEvent))
                thisEvent.Invoke(args);

#if DEBUG
            if (logEvents)
            {
                if (thisEvent == null) Debug.Log($"{Instance.GetType()}: Attempted to invoke event {eventName} but it had no listeners");
                else Debug.Log($"{Instance.GetType()}: Invoked event {eventName} on {thisEvent.GetInvocationList().Count()} listeners");
            }
#endif
        }

        /// <summary>
        /// Invokes the event specified by the event name. If the event has no subscribers, the event will wait until 
        /// is has subscribers and then invoke those subscribers. This is useful if events may get invoked before they 
        /// have subscribers.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        public void InvokeEventOrWaitOnSubscriber(string eventName, Dictionary<string, object> args)
        {
            Action<Dictionary<string, object>> thisEvent;
            if (eventDictionaryArgs.TryGetValue(eventName, out thisEvent)) thisEvent.Invoke(args);
            else if (pendingEventArgsArgs.ContainsKey(eventName)) return;
            else pendingEventArgsArgs.Add(eventName, args);

#if DEBUG
            if (logEvents)
            {
                if (thisEvent == null) Debug.Log($"{Instance.GetType()}: Attempted to invoke event {eventName} but it had no listeners");
                else Debug.Log($"{Instance.GetType()}: Invoked event {eventName} on {thisEvent.GetInvocationList().Count()} listeners");
            }
#endif
        }

        // Events With No Arguments ---------------------------------------------------------------

        /// <summary>
        /// Starts a function listening to an event. The function must take a Dictionary<string, object> 
        /// as its parameters. The dictionary will hold any parameters passed to the function when the 
        /// event is triggered. If the function's object is destroyed, remember to call StopListening down below.
        /// </summary>
        /// <param name="eventName">event to listen to</param>
        /// <param name="listener">function that will listen for event</param>
        public void SubscribeToEvent(string eventName, Action listener)
        {
            Action thisEvent;
            if (eventDictionaryNoArgs.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                eventDictionaryNoArgs[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionaryNoArgs.Add(eventName, thisEvent);
            }
        }

        /// <summary>
        /// Starts a function listening to an event. The function must take a Dictionary<string, object> 
        /// as its parameters. The dictionary will hold any parameters passed to the function when the 
        /// event is triggered. If the function's object is destroyed, remember to call StopListening down below.
        /// </summary>
        /// <param name="eventName">event to listen to</param>
        /// <param name="listener">function that will listen for event</param>
        public void SubscribeToEventAndPending(string eventName, Action listener)
        {
            Action thisEvent;
            if (eventDictionaryNoArgs.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                eventDictionaryNoArgs[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionaryNoArgs.Add(eventName, thisEvent);
            }

            if (pendingEventsNoArgs.Contains(eventName))
            {
                listener.Invoke();
                pendingEventsNoArgs.Remove(eventName);
            }
        }

        /// <summary>
        /// Stops a function listening to an event. This should be called before for any functions 
        /// belonging to an object that is about to be destroyed.
        /// </summary>
        /// <param name="eventName">event to stop listening to</param>
        /// <param name="listener">function that will stop listening for event</param>
        public void UnsubscribeFromEvent(string eventName, Action listener)
        {
            Action thisEvent;
            if (eventDictionaryNoArgs.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                eventDictionaryNoArgs[eventName] = thisEvent;
                if (eventDictionaryNoArgs[eventName] == null) eventDictionaryNoArgs.Remove(eventName);
            }
        }

        /// <summary>
        /// Invokes the event specified by event name.
        /// </summary>
        /// <param name="eventName">event to invoke</param>
        /// <param name="args">parameters passed to any function invoked by event</param>
        public void InvokeEvent(string eventName)
        {
            Action thisEvent;
            if (eventDictionaryNoArgs.TryGetValue(eventName, out thisEvent))
                thisEvent.Invoke();

#if DEBUG
            if (logEvents)
            {
                if (thisEvent == null) Debug.Log($"{Instance.GetType()}: Attempted to invoke event {eventName} but it had no listeners");
                else Debug.Log($"{Instance.GetType()}: Invoked event {eventName} on {thisEvent.GetInvocationList().Count()} listeners");
            }
#endif
        }

        /// <summary>
        /// Invokes the event specified by the event name. If the event has no subscribers, the event will wait until 
        /// is has subscribers and then invoke those subscribers. This is useful if events may get invoked before they 
        /// have subscribers.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        public void InvokeEventOrWaitOnSubscriber(string eventName)
        {
            Action thisEvent;
            if (eventDictionaryNoArgs.TryGetValue(eventName, out thisEvent)) thisEvent.Invoke(); // if already has subscribers, then invoke
            else if (pendingEventArgsArgs.ContainsKey(eventName)) return; // if already added as pending event, then just return
            else pendingEventsNoArgs.Add(eventName); // if not added as pending event, then add

#if DEBUG
            if (logEvents)
            {
                if (thisEvent == null) Debug.Log($"{Instance.GetType()}: Attempted to invoke event {eventName} but it had no listeners");
                else Debug.Log($"{Instance.GetType()}: Invoked event {eventName} on {thisEvent.GetInvocationList().Count()} listeners");
            }
#endif
        }
    }
}
