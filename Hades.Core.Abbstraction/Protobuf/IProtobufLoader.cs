using System;

namespace Hades.Core.Abbstraction
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProtobufLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="protoByte"></param>
        /// <param name="protoName"></param>
        /// <returns></returns>
        ProtobufLoadResult LoadProtoFile(ReadOnlySpan<byte> protoByte, string protoName);
    }
}
