using System;
using System.Collections.Generic;
using SS14.Shared.GameObjects;
using SS14.Shared.Interfaces.GameObjects;
using SS14.Shared.Interfaces.Reflection;
using SS14.Shared.IoC;

namespace Content.Shared.GameObjects
{
    /// <summary>
    ///     Apply this to a component class definition and it will be automatically registered to the component factory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegAttribute : Attribute
    {
        public IReadOnlyList<Type> Types { get; }

        /// <param name="types">
        ///     A list of reference types that will be registered for this component.
        ///     Note that all parent types (up to <see cref="Component"/>) are automatically registered.
        /// </param>
        public AutoRegAttribute(params Type[] types)
        {
            Types = types;
        }

        public static void DoRegistrations()
        {
            var factory = IoCManager.Resolve<IComponentFactory>();
            var refl = IoCManager.Resolve<IReflectionManager>();

            var register = fac

            foreach (var type in refl.FindTypesWithAttribute<AutoRegAttribute>())
            {

            }
        }
    }
}
