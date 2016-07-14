using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitBucketVs.Contracts
{
    public interface IDialogWindow : IView
    {
        bool? ShowModal();
}
}
