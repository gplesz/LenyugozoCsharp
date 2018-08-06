using System;
using System.Collections.Generic;

namespace bot.server.Controllers
{
    internal class Reminders
    {
        static List<Reminder> reminders = new List<Reminder>();
        internal static void Add(Reminder reminder)
        {
           reminders.Add(reminder);
        }
    }
}