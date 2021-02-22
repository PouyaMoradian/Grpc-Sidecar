using System;
using System.Collections.Generic;

namespace Hades.Core.Abbstraction
{
    /// <summary>
    /// 
    /// </summary>
    public class ProtobufLoadResult
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Type> Messages { get; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Type> Services { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="services"></param>
        public ProtobufLoadResult(
            IEnumerable<Type> messages, 
            IEnumerable<Type> services)
        {
            Messages = messages 
                ?? throw new ArgumentNullException(nameof(messages));
            Services = services 
                ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
