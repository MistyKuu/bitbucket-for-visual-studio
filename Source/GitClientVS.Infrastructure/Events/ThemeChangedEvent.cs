using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Infrastructure.Events
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
