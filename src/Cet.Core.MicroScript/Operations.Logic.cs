using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    [Instruct(Name = "or", Mode = OperandMode.None)]
    internal class OpOr : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            object ob = stack.Pop();
            object oa = stack.Pop();

            bool isTrue = OperationHelpers.EvaluateAsLogicalTrue(oa);
            stack.Push(isTrue ? oa : ob);
        }
    }


    [Instruct(Name = "and", Mode = OperandMode.None)]
    internal class OpAnd : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            object ob = stack.Pop();
            object oa = stack.Pop();

            bool isTrue = OperationHelpers.EvaluateAsLogicalTrue(oa);
            stack.Push(isTrue ? ob : oa);
        }
    }
}
