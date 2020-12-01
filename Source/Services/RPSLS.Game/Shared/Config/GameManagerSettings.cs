namespace RPSLS.Game.Shared.Config
{
    public class GameManagerSettings
    {
        public string Url { get; set; }

        public GrpcSettings Grpc { get; set; }

        public GameManagerSettings()
        {
            Grpc = new GrpcSettings();
        }
    }
}
