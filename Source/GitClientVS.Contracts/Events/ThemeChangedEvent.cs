using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Events
{
    public class ThemeChangedEvent
    {
        public Theme Theme { get; set; }

        public ThemeChangedEvent(Theme theme)
        {
            Theme = theme;
        }
    }
}
