using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GitClientVS.UI.Bindings
{
    public class ValidatedBinding : Binding
    {
        public ValidatedBinding(string path) : base(path)
        {
            ValidatesOnDataErrors = true;
            ValidatesOnExceptions = true;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Mode = BindingMode.TwoWay;
        }
    }
}
