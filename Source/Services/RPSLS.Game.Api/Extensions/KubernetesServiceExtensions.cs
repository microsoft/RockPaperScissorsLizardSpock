using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Extensions
{
    static class KubernetesServiceExtensions
    {
        public static bool MatchSelector(this k8s.Models.V1Service svc, string selector)
        {
            return svc.Metadata.MatchSelector(selector);
        }
    }
}
