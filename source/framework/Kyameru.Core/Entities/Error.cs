using System.Text.Json;

namespace Kyameru.Core.Entities
{
    public class Error
    {
        public string Component { get; private set; }

        public string CurrentAction { get; private set; }

        public string Message { get; private set; }

        public Error(string component, string action, string message)
        {
            this.Component = component;
            this.CurrentAction = action;
            this.Message = message;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}