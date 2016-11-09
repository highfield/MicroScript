using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    [Instruct(Name = "nop", Mode = OperandMode.None)]
    internal class OpNop : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            //no op
        }
    }


    internal static class OperationHelpers
    {

        public static bool EvaluateAsLogicalTrue(object obj)
        {
            string s;
            if (obj == null)
            {
                return false;
            }
            else if (obj is bool)
            {
                return (bool)obj;
            }
            else if (obj is double)
            {
                return (double)obj != 0;
            }
            else if ((s = obj as string) != null)
            {
                return s.Length != 0;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

    }
}
