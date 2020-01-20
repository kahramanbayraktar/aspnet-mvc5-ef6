using System;

namespace Ced.BusinessEntities
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class NotificationAttribute : Attribute
    {
        public readonly bool SendEmail;

        public bool PrivateEmail { get; set; }

        public string ViewName { get; set; }

        public string ButtonText { get; set; }

        public bool Unsubscribable { get; set; }
        
        public string FaIcon { get; set; }
        
        public string TextClass { get; set; }

        public string Fragment { get; set; }

        public EventType[] EventTypes { get; set; }

        public CheckDaysTypes CheckDaysType { get; set; }

        public NotificationAttribute(bool sendEmail = false, bool privateEmail = false, EventType[] eventTypes = null, CheckDaysTypes checkDaysType = CheckDaysTypes.Coming,
            string viewName = null, string buttonText = null, bool unsubscribable = false, string faIcon = null, string textClass = "", string fragment = "")
        {
            SendEmail = sendEmail;
            PrivateEmail = privateEmail;
            EventTypes = eventTypes;
            CheckDaysType = checkDaysType;

            ViewName = viewName;
            ButtonText = buttonText;
            Unsubscribable = unsubscribable;
            FaIcon = faIcon;
            TextClass = textClass;
            Fragment = fragment;
        }

        public enum CheckDaysTypes
        {
            Passed,
            Coming
        }
    }
}
