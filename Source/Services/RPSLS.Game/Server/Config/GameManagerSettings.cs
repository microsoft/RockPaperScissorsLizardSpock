namespace RPSLS.Game.Server.Config
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
