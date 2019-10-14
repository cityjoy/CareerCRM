using System;
using System.ComponentModel;

namespace CareerCRM.Repository.Core
{
    public abstract class Entity
    {
        [Description("Id")]
        public string Id { get; set; }

        public Entity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
