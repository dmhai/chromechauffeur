namespace ChromeChauffeur.Core.Chrome.Api
{
    public class RemoteDebuggerCommand
    {
        public RemoteDebuggerCommand()
        {
            Method = "Runtime.evaluate";
            Id = 1;
            Params = new RemoteDebuggerCommandParams();
        }

        public int Id { get; set; }
        public string Method { get; set; }
        public RemoteDebuggerCommandParams Params { get; set; }
    }
}