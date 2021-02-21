using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Sidecar.Client.Internal.Middlewares
{
    public static class GrpcRouteParser
    {

        //This can become more reach
        public static (string packageName, string serviceName, string methodName) GetGrpcDescriptions(string requestPath)
        {
            var pathParts = requestPath.Split('/');
            var packageName = pathParts[1].Split('.');
            var serviceName = pathParts[2];

            return ("", "", "");
        }

    }
}
