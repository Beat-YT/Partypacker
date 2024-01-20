using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Partypacker
{
    public class SelectableOption
    {
        public string Name;
        public Action OnPress;

        public SelectableOption(string name, Action onPress)
        {
            Name = name;
            OnPress = onPress;
        }
    }
}
