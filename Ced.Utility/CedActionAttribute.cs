using System;
using Ced.BusinessEntities;

namespace Ced.Utility
{
    public class CedActionAttribute : Attribute
    {
        public ActionType ActionType { get; set; }

        public bool Loggable { get; set; }

        public bool External { get; set; }
    }
}