using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5.General
{
    public class ToggleSidebarMessage : ValueChangedMessage<bool>
    {
        public ToggleSidebarMessage(bool isToggled) : base(isToggled) { }
    }
}
