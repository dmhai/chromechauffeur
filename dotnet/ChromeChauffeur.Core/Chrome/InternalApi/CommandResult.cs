using System;
using ChromeChauffeur.Core.Chrome.Api;

namespace ChromeChauffeur.Core.Chrome.InternalApi
{
    internal class CommandResult
    {
        public CommandResult(RemoteDebuggerCommandResponse response)
        {
            WasExceptionThrown = response.Result.WasThrown;
            Value = response.Result.Result.Value;
        }

        public readonly bool WasExceptionThrown;
        public readonly string Value;

        public T GetValue<T>()
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }
    }
}