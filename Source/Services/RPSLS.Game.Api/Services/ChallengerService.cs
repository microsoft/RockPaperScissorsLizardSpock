using GameApi.Proto;
using k8s;
using k8s.Exceptions;
using k8s.Models;
using Microsoft.Extensions.Logging;
using RPSLS.Game.Api.Config;
using RPSLS.Game.Api.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace RPSLS.Game.Api.Services
{
    public class ChallengerService : IChallengerService
    {
        private readonly List<ChallengerOptions> _challengersOptions;
        private readonly List<IChallenger> _challengers;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;

        public ChallengerService(IHttpClientFactory clientFactory, IEnumerable<ChallengerOptions> options, ILoggerFactory loggerFactory)
        {
            _clientFactory = clientFactory;
            _logger = loggerFactory.CreateLogger<ChallengerService>();
            _challengersOptions = new List<ChallengerOptions>(options);
            _challengers = new List<IChallenger>();
            LoadChallengers();
        }

        private void LoadChallengers()
        {
            var services = LoadKubernetesServices();

            foreach (var challengerOptions in _challengersOptions)
            {
                if (!string.IsNullOrEmpty(challengerOptions.Url))
                {
                    _challengers.Add(new ExternalChallenger(challengerOptions, _clientFactory));
                }
                else
                {
                    if (services != null)
                    {
                        var svc = services.Items.FirstOrDefault(s => s.MatchSelector(challengerOptions.Selector));
                        if (svc != null)
                        {
                            _challengers.Add(new Challenger(challengerOptions, svc, _clientFactory));
                        }
                    }
                }
            }
        }

        private V1ServiceList LoadKubernetesServices()
        {
            try
            {
                var config =
                    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_PORT"))
                    ? KubernetesClientConfiguration.BuildConfigFromConfigFile()
                    : KubernetesClientConfiguration.InClusterConfig();

                var client = new Kubernetes(config);
                return client.ListNamespacedService("default");
            }
            catch (KubeConfigException ex)
            {
                _logger.LogError(ex, "This is not a FATAL error. It only means that K8S service discovery is off.");
                return null;
            }
        }

        public IChallenger SelectChallenger(GameRequest request)
        {
            return _challengers.FirstOrDefault(c => c.Info.Name == request.Challenger);
        }

        public IEnumerable<IChallenger> Challengers => _challengers;
    }
}
