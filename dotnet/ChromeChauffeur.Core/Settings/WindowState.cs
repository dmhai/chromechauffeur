using System.Diagnostics;

namespace ChromeChauffeur.Core.Settings
{
    public class WindowState
    {
        private readonly ProcessWindowStyle _windowStyle;

        private WindowState(ProcessWindowStyle windowStyle)
        {
            _windowStyle = windowStyle;
        }

        internal ProcessWindowStyle GetProcessWindowStyle()
        {
            return _windowStyle;
        }

        public static readonly WindowState Normal = new WindowState(ProcessWindowStyle.Normal);
        public static readonly WindowState Minimized = new WindowState(ProcessWindowStyle.Minimized);
        public static readonly WindowState Maximized = new WindowState(ProcessWindowStyle.Maximized);
    }
}