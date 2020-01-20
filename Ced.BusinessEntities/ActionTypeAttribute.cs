using System;

namespace Ced.BusinessEntities
{
    public class ActionTypeAttribute : Attribute
    {
        public EntityType EntityType { get; set; }
    }
}