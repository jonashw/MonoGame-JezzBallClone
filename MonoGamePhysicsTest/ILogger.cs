using System.Diagnostics;

namespace MonoGamePhysicsTest
{
    public interface ILogger
    {
        void Log(string message);
    }

    public class DebugLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }

    public class NullLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }
}