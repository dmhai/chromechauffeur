namespace ChromeChauffeur.Core.Chrome.Api
{
    internal class RemoteDebuggerCommandResult
    {
        public RemoteDebuggerValue Result { get; set; }
        public bool WasThrown { get; set; }
    }
}