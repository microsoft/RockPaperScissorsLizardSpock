using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using RPSLS.Game.Multiplayer.Builders;
using RPSLS.Game.Multiplayer.Config;
using RPSLS.Game.Multiplayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Multiplayer.Services
{
    public class PlayFabService : IPlayFabService
    {
        private const string queueName = "rpsls_queue";
        private const string WinsStat = "Wins";
        private const string TotalStat = "Total";

        private readonly ILogger<PlayFabService> _logger;
        private readonly MultiplayerSettings _settings;

        private Token _entityToken = null;

        public bool HasCredentials { get => !string.IsNullOrWhiteSpace(_settings.Title) && !string.IsNullOrWhiteSpace(_settings.SecretKey); }

        public PlayFabService(ILogger<PlayFabService> logger, IOptions<MultiplayerSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task Initialize()
        {
            PlayFabSettings.staticSettings.TitleId = _settings.Title;
            PlayFabSettings.staticSettings.DeveloperSecretKey = _settings.SecretKey;

            if (!HasCredentials) return;

            await EnsureQueueExist();
            await EnsureLeaderBoardExists();
        }

        public async Task<string> GetEntityToken(string userTitleId = null)
        {
            if (_entityToken?.Value != null && !_entityToken.IsExpired)
            {
                return _entityToken.Value;
            }

            PlayFabAuthenticationAPI.ForgetAllCredentials();

            var tokenRequestBuilder = new GetEntityTokenRequestBuilder();
            if (!string.IsNullOrWhiteSpace(userTitleId))
            {
                tokenRequestBuilder.WithUserToken(userTitleId);
            }

            var entityTokenResult = await Call(
                PlayFabAuthenticationAPI.GetEntityTokenAsync,
                tokenRequestBuilder);

            _entityToken = new Token(entityTokenResult.EntityToken, _settings.EntityTokenExpirationMinutes);

            return _entityToken.Value;
        }

        public async Task<string> CreateTicket(string username, string token)
        {
            var userEntity = await GetUserEntity(username);

            await Call(
                PlayFabMultiplayerAPI.CancelAllMatchmakingTicketsForPlayerAsync,
                new CancelAllMatchmakingTicketsForPlayerRequestBuilder()
                    .WithEntity(userEntity.Id, userEntity.Type)
                    .WithQueue(queueName));

            var ticketResult = await Call(
                PlayFabMultiplayerAPI.CreateMatchmakingTicketAsync,
                new CreateMatchmakingTicketRequestBuilder()
                    .WithCreatorEntity(userEntity.Id, userEntity.Type, token, username)
                    .WithGiveUpOf(120)
                    .WithQueue(queueName));

            return ticketResult.TicketId;
        }

        public async Task<MatchResult> CheckTicketStatus(string username, string ticketId)
        {
            var result = new MatchResult();
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                return result;
            }

            var userEntity = await GetUserEntity(username);
            var matchTicketResult = await Call(
                PlayFabMultiplayerAPI.GetMatchmakingTicketAsync,
                new GetMatchmakingTicketRequestBuilder()
                    .WithQueue(queueName)
                    .WithTicketId(ticketId));

            var status = matchTicketResult?.Status ?? string.Empty;
            result.Status = status;
            result.MatchId = matchTicketResult?.MatchId ?? string.Empty;
            if (result.Matched)
            {
                var getMatchResult = await Call(
                    PlayFabMultiplayerAPI.GetMatchAsync,
                    new GetMatchRequestBuilder()
                        .WithId(result.MatchId)
                        .WithQueue(queueName)
                        .WithMemberAttributes());

                var opponentEntity = getMatchResult.Members?.FirstOrDefault(u => u.Entity.Id != userEntity.Id);
                if (opponentEntity != null)
                {
                    result.Opponent = "Unknown";
                    var dataObject = opponentEntity.Attributes.DataObject as PlayFab.Json.JsonObject;
                    if (dataObject != null && dataObject.TryGetValue("DisplayName", out object displayName))
                    {
                        result.Opponent = displayName?.ToString() ?? "Unknown";
                    }
                }
            }

            return result;
        }

        public async Task UpdateStats(string username, bool isWinner)
        {
            var isNotTwitterUser = username?.StartsWith("$") ?? false;
            if (_settings.Leaderboard.OnlyTwitter && isNotTwitterUser)
            {
                return;
            }

            if (username.Length < 3 || username.Length > 25)
            {
                _logger.LogWarning($"User {username} cannot be stored in the leaderboard because doesn't have display name at Playfab.");
                return;
            }

            var loginResult = await UserLogin(username.ToUpperInvariant());
            var statsRequestBuilder = new UpdatePlayerStatisticsRequestBuilder()
                .WithPlayerId(loginResult.PlayFabId)
                .WithStatsIncrease(TotalStat);

            if (isWinner)
            {
                statsRequestBuilder.WithStatsIncrease(WinsStat);
            }

            await Call(PlayFabServerAPI.UpdatePlayerStatisticsAsync, statsRequestBuilder);
        }

        public async Task<Leaderboard> GetLeaderboard()
        {
            var entityToken = await GetEntityToken();
            var leaderboardResult = await Call(
                PlayFabServerAPI.GetLeaderboardAsync,
                new GetLeaderboardRequestBuilder()
                    .WithTitleContext(_settings.Title, entityToken)
                    .WithStats(WinsStat)
                    .WithLimits(0, _settings.Leaderboard.Top));

            var players = new List<LeaderboardEntry>();
            foreach (var entry in leaderboardResult.Leaderboard)
            {
                var isTwitterUser = !(entry.DisplayName?.StartsWith("$") ?? false);
                var username = isTwitterUser ? entry.DisplayName : entry.DisplayName.Substring(1);
                players.Add(new LeaderboardEntry
                {
                    Position = entry.Position,
                    Username = username,
                    IsTwitterUser = isTwitterUser,
                    Score = entry.StatValue
                });
            }

            return new Leaderboard { Players = players };
        }

        private async Task<EntityKey> GetUserEntity(string username)
        {
            var loginResult = await UserLogin(username);
            return loginResult.EntityToken.Entity;
        }

        private async Task<LoginResult> UserLogin(string username)
        {
            var loginResult = await Call(
                PlayFabClientAPI.LoginWithCustomIDAsync,
                new LoginWithCustomIDRequestBuilder()
                    .WithUser(username.ToUpperInvariant())
                    .WithAccountInfo()
                    .CreateIfDoesntExist());

            if (loginResult.NewlyCreated || loginResult.InfoResultPayload?.AccountInfo?.TitleInfo?.DisplayName != username)
            {
                // Add a DisplayName to the title user so its easier to retrieve the user;
                await Call(
                    PlayFabClientAPI.UpdateUserTitleDisplayNameAsync,
                    new UpdateUserTitleDisplayNameRequestBuilder()
                        .WithName(username.ToUpperInvariant()));
            }

            return loginResult;
        }

        private async Task EnsureQueueExist()
        {
            var entityToken = await GetEntityToken();
            var fetchQueueResult = await Call(
                PlayFabMultiplayerAPI.GetMatchmakingQueueAsync,
                new GetMatchmakingQueueRequestBuilder()
                    .WithQueue(queueName)
                    .WithTitleContext(_settings.Title, entityToken));

            if (fetchQueueResult == null)
            {
                // Create if queue does not exist
                await Call(
                    PlayFabMultiplayerAPI.SetMatchmakingQueueAsync,
                    new SetMatchmakingQueueRequestBuilder()
                        .WithQueue(queueName, 2)
                        .WithTitleContext(_settings.Title, entityToken)
                        .WithQueueStringRule("TokenRule", "Token", "random"));
            }
        }

        private async Task EnsureLeaderBoardExists()
        {
            var statsResult = await Call(
                PlayFabAdminAPI.GetPlayerStatisticDefinitionsAsync,
                new GetPlayerStatisticDefinitionsRequestBuilder());

            if (statsResult?.Statistics?.FirstOrDefault(s => s.StatisticName == WinsStat) == null)
            {
                await Call(
                    PlayFabAdminAPI.CreatePlayerStatisticDefinitionAsync,
                    new CreatePlayerStatisticDefinitionRequestBuilder()
                        .WithAggregatedStat(WinsStat));
            }

            if (statsResult?.Statistics?.FirstOrDefault(s => s.StatisticName == TotalStat) == null)
            {
                await Call(
                    PlayFabAdminAPI.CreatePlayerStatisticDefinitionAsync,
                    new CreatePlayerStatisticDefinitionRequestBuilder()
                        .WithAggregatedStat(TotalStat));
            }
        }

        private async Task<U> Call<T, U>(Func<T, object, Dictionary<string, string>, Task<PlayFabResult<U>>> playFabCall, BaseRequestBuilder<T> requestBuilder)
            where U : PlayFabResultCommon
            where T : new()
            => (await CallWithError(playFabCall, requestBuilder)).Result;

        private async Task<PlayFabResult<U>> CallWithError<T, U>(Func<T, object, Dictionary<string, string>, Task<PlayFabResult<U>>> playFabCall, BaseRequestBuilder<T> requestBuilder)
            where U : PlayFabResultCommon
            where T : new()
        {
            var taskResult = await playFabCall(requestBuilder.Build(), null, null);
            var apiError = taskResult.Error;
            if (apiError != null)
            {
                var detailedError = PlayFabUtil.GenerateErrorReport(apiError);
                _logger.LogError($"Something went wrong with PlayFab API call {playFabCall.Method.Name}.{Environment.NewLine}{detailedError}");
            }

            return taskResult;
        }
    }
}
