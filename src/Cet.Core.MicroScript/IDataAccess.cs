using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    public interface IDataAccess
    {
        object GetData(string name);
        void SetData(string name, object value);
    }
}
