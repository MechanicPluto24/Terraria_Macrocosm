using System;

namespace Macrocosm.Common.UI
{
    public interface IFocusable
    {
        public bool HasFocus { get; set; }

        public string FocusContext { get; set; }

        public Action OnFocusGain { get; set; }
        public Action OnFocusLost { get; set; }
    }
}
