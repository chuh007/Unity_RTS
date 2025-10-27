using System;
using System.Collections.Generic;
using Code.Units;

namespace Code.CoreSystem
{
    public static class Bus<T> where T : IEvent
    {
        //arguments
        public delegate void Event(T evt);

        public static Dictionary<Owner, Event> OnEvents = new Dictionary<Owner, Event>
        {
            { Owner.Invalid , null},
            { Owner.UnOwned , null},
            { Owner.Player , null},
            { Owner.AI1 , null},
            { Owner.AI2 , null},
            { Owner.AI3 , null},
        };
        
        public static void Raise(Owner owner, T evt) => OnEvents[owner]?.Invoke(evt);

        public static void RegisterForAll(Event handler)
        {
            foreach (Owner owner in Enum.GetValues(typeof(Owner)))
            {
                OnEvents[owner] += handler;
            }
        }

        public static void UnRegisterForAll(Event handler)
        {
            foreach (Owner owner in Enum.GetValues(typeof(Owner)))
            {
                OnEvents[owner] -= handler;
            }
        }
    }
}