using Hades.Core.Abbstraction;
using System;
using System.IO;

namespace Hades.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProtobufLoaderExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="filePath"></param>
        /// <param name="protoName"></param>
        /// <returns></returns>
        public static ProtobufLoadResult LoadProtoFile(
            this IProtobufLoader loader,
            string filePath,
            string protoName = default)
        {
            _ = loader
                ?? throw new ArgumentNullException(nameof(loader));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            protoName = string.IsNullOrWhiteSpace(filePath) ? Path.GetFileName(protoName) : protoName;
            var bytes = File.ReadAllBytes(filePath);
            return loader.LoadProtoFile(bytes, protoName);
        }
    }
}
