using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    public static partial class Compiler
    {
        /// <summary>
        /// Main generic-line parser
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="line"></param>
        private static void ParseLine(
            CompilationContext cctx,
            string line
            )
        {
            var pctx = new ParserContext();
            pctx.Buffer = line.ToCharArray();

            //leading wsp
            ConsumeWhitespace(pctx);

            //label decl
            var resLabel = ConsumeLabelDecl(pctx);
            if (resLabel.Error == null)
            {
                //Console.WriteLine($"label decl: {resLabel.Data};");
                cctx.LabelMap.Add(resLabel.Data, cctx.CodeStream.InstructList.Count);
            }

            ConsumeWhitespace(pctx);

            //instruction
            ParseInstruction(cctx, pctx);

            ConsumeWhitespace(pctx);

            //comment
            ConsumeComment(pctx);

            //trailing non-wsp characters
            var resNonWsp = ConsumeNonWhitespace(pctx);
            if (resNonWsp.ConsumedCount != 0)
            {
                throw new CompilationException($"unrecognized: {resNonWsp.Data};");
            }
        }


        /// <summary>
        /// Composite instruction (opcode, operands, etc) parser
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        private static void ParseInstruction(
            CompilationContext cctx,
            ParserContext pctx
            )
        {
            var result = ConsumeInstruction(pctx);
            if (result.Error != null) return;

            //Console.WriteLine($"instr: {resInstr.Data};");
            InstructEntryDescriptor ientry;
            if (_opmap.TryGetValue(result.Data, out ientry) == false)
            {
                //TODO error
            }

            ConsumeWhitespace(pctx);

            //operand
            var iresolver = new InstructionModeResolver();

            if (ParseOperandImmediate1(cctx, pctx, iresolver) ||
                ParseOperandRegister1(cctx, pctx, iresolver) ||
                ParseOperandDirect1(cctx, pctx, iresolver)
                )
            {
                //separator
                ConsumeWhitespace(pctx);
                var ressep = ConsumeOperandSeparator(pctx);
                if (ressep.Error == null && ressep.Data)
                {
                    ConsumeWhitespace(pctx);
                    if (ParseOperandImmediate2(cctx, pctx, iresolver) ||
                        ParseOperandRegister2(cctx, pctx, iresolver) ||
                        ParseOperandDirect2(cctx, pctx, iresolver)
                        )
                    {
                        //parsed okay
                    }
                    else
                    {
                        //TODO error
                    }
                }
            }

            var iactual = ientry.DsList.FirstOrDefault(_ => _.Mode == iresolver.Mode);
            if (iactual == null && iresolver.Mode == OperandMode.Dir1)
            {
                iactual = ientry.DsList.FirstOrDefault(_ => _.Mode == OperandMode.Label);
            }

            var instance = Activator.CreateInstance(iactual.Type);
            {
                var obj = instance as IOperationImmediate1;
                if (obj != null) obj.Value1 = iresolver.Value1;
            }
            {
                var obj = instance as IOperationRegister1;
                if (obj != null) obj.RegName1 = iresolver.RegName1;
            }
            {
                var obj = instance as IOperationDirect1;
                if (obj != null) obj.VarName1 = iresolver.VarName1;
            }
            {
                var obj = instance as IOperationImmediate2;
                if (obj != null) obj.Value2 = iresolver.Value2;
            }
            {
                var obj = instance as IOperationRegister2;
                if (obj != null) obj.RegName2 = iresolver.RegName2;
            }
            {
                var obj = instance as IOperationDirect2;
                if (obj != null) obj.VarName2 = iresolver.VarName2;
            }
            {
                var obj = instance as IOperationLabel;
                if (obj != null) obj.LabelName = iresolver.VarName1;
            }
            cctx.CodeStream.InstructList.Add((IOperation)instance);
        }


        private class InstructionModeResolver
        {
            public OperandMode Mode;

            public object Value1;
            public string RegName1;
            public string VarName1;

            public object Value2;
            public string RegName2;
            public string VarName2;

            public Exception Error;
        }


        /// <summary>
        /// Parse first operand as register reference
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandRegister1(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeRegister(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"operand: {resOper.Data};");
                iresolver.Mode |= OperandMode.Reg1;
                iresolver.RegName1 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse second operand as register reference
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandRegister2(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeRegister(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"operand: {resOper.Data};");
                iresolver.Mode |= OperandMode.Reg2;
                iresolver.RegName2 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse first operand as external data reference
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandDirect1(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeIdToken(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"operand: {resOper.Data};");
                iresolver.Mode |= OperandMode.Dir1;
                iresolver.VarName1 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse second operand as external data reference
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandDirect2(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeIdToken(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"operand: {resOper.Data};");
                iresolver.Mode |= OperandMode.Dir2;
                iresolver.VarName2 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse first operand as immediate value
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediate1(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeImmediatePrefix(pctx);
            if (result.Error == null)
            {
                if (ParseOperandImmediateBoolean1(cctx, pctx, iresolver) ||
                    ParseOperandImmediateNumeric1(cctx, pctx, iresolver) ||
                    ParseOperandImmediateString1(cctx, pctx, iresolver)
                    )
                {
                    //parsed okay
                }
                else
                {
                    result.Error = new CompilationException("unable to parse immediate value (1): ;");
                }
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse second operand as immediate value
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediate2(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeImmediatePrefix(pctx);
            if (result.Error == null)
            {
                if (ParseOperandImmediateBoolean2(cctx, pctx, iresolver) ||
                    ParseOperandImmediateNumeric2(cctx, pctx, iresolver) ||
                    ParseOperandImmediateString2(cctx, pctx, iresolver)
                    )
                {
                    //parsed okay
                }
                else
                {
                    result.Error = new CompilationException("unable to parse immediate value (2): ;");
                }
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse first operand as immediate boolean constant
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediateBoolean1(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeBooleanValue(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"imm bool: {resValBool.Data};");
                iresolver.Mode |= OperandMode.Imm1;
                iresolver.Value1 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse second operand as immediate boolean constant
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediateBoolean2(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeBooleanValue(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"imm bool: {resValBool.Data};");
                iresolver.Mode |= OperandMode.Imm2;
                iresolver.Value2 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse first operand as immediate numeric constant
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediateNumeric1(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeNumericValue(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"imm num: {resValNum.Data};");
                iresolver.Mode |= OperandMode.Imm1;
                iresolver.Value1 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse second operand as immediate numeric constant
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediateNumeric2(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeNumericValue(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"imm num: {resValNum.Data};");
                iresolver.Mode |= OperandMode.Imm2;
                iresolver.Value2 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse first operand as immediate string constant
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediateString1(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeStringValue(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"imm str: {resValStr.Data};");
                iresolver.Mode |= OperandMode.Imm1;
                iresolver.Value1 = result.Data;
            }

            return result.Error == null;
        }


        /// <summary>
        /// Parse second operand as immediate string constant
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="pctx"></param>
        /// <param name="iresolver"></param>
        /// <returns></returns>
        private static bool ParseOperandImmediateString2(
            CompilationContext cctx,
            ParserContext pctx,
            InstructionModeResolver iresolver
            )
        {
            var result = ConsumeStringValue(pctx);
            if (result.Error == null)
            {
                //Console.WriteLine($"imm str: {resValStr.Data};");
                iresolver.Mode |= OperandMode.Imm2;
                iresolver.Value2 = result.Data;
            }

            return result.Error == null;
        }

    }
}
