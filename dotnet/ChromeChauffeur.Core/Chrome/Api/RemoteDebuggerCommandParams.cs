namespace ChromeChauffeur.Core.Chrome.Api
{
    public class RemoteDebuggerCommandParams
    {
        public RemoteDebuggerCommandParams()
        {
            ObjectGroup = "console";
            IncludeCommandLineAPI = true;
            DoNotPauseOnExceptions = false;
            ReturnByValue = false;
        }

        public string Expression { get; set; }
        public string ObjectGroup { get; set; }
        public bool IncludeCommandLineAPI { get; set; }
        public bool DoNotPauseOnExceptions { get; set; }
        public bool ReturnByValue { get; set; }
    }
}