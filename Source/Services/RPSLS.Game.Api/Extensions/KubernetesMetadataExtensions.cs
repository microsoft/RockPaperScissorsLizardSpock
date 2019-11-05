using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Extensions
{
    static class KubernetesMetadataExtensions
    {
        public static bool MatchSelector(this k8s.Models.V1ObjectMeta meta, string selector)
        {
            if (string.IsNullOrEmpty(selector))
            {
                return false;
            }
            var labelpairs = selector.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in labelpairs)
            {
                string desiredLabel = null;
                string desiredLabelValue = null;
                var indexOfEquals = pair.IndexOf('=');
                if (indexOfEquals != -1)
                {
                    desiredLabel = pair.Substring(0, indexOfEquals);
                    desiredLabelValue = pair.Substring(indexOfEquals + 1);
                }
                else
                {
                    desiredLabel = pair;
                }

                if (meta.Labels.TryGetValue(desiredLabel, out string k8sLabelValue))
                {
                    if (desiredLabelValue != null && k8sLabelValue != desiredLabelValue)
                    {
                        return false;           // Label name match, but we have desired label value, that do not match, so this selector do not apply
                    }
                }
                else
                {
                    return false;               // No label match this selector, and ALL selectors must be matched.
                }
            }

            return true;

        }
    }
}
