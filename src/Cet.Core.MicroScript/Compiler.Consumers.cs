using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    public static partial class Compiler
    {
        //rules for matching an identifier name
        private static readonly Func<char, bool> IdStartPredicate = c => char.IsLetter(c) || c == '_';
        private static readonly Func<char, bool> IdBodyPredicate = c => char.IsLetter(c) || char.IsDigit(c) || c == '_';

        //rules for matching an instruction name
        private static readonly Func<char, bool> InstrStartPredicate = c => char.IsLetter(c);
        private static readonly Func<char, bool> InstrBodyPredicate = c => char.IsLetter(c) || char.IsDigit(c);


        /// <summary>
        /// Whitespace consumer
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static ParserResult<IReadOnlyCollection<char>> ConsumeWhitespace(
            ParserContext ctx
            )
        {
            return ConsumeChars(ctx, c => char.IsWhiteSpace(c));
        }


        /// <summary>
        /// Non-whitespace consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeNonWhitespace(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, c => char.IsWhiteSpace(c) == false, 1);

            var result = ConcatToString(res1);
            if (result.Error == null)
            {
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Comment consumer (up to the line end)
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeComment(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, c => c == '/', 2);
            var res2 = ConsumeChars(cctx, c => true);

            var result = ConcatToString(res1, res2);
            if (result.Error == null)
            {
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Identificator consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeIdToken(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, IdStartPredicate, 1);
            var res2 = ConsumeChars(cctx, IdBodyPredicate);

            var result = ConcatToString(res1, res2);
            if (result.Error == null)
            {
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Generic literal token consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeLiteral(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, c => char.IsLetter(c));

            var result = ConcatToString(res1);
            if (result.Error == null)
            {
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Label declaration consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeLabelDecl(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, IdStartPredicate, 1);
            var res2 = ConsumeChars(cctx, IdBodyPredicate);
            var resSuffix = ConsumeChars(cctx, c => c == ':', 1, 1);

            var result = ConcatToString(res1, res2);
            result.Error = result.Error ?? resSuffix.Error;
            if (result.Error == null)
            {
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Generic instruction (name) consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeInstruction(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, InstrStartPredicate, 1);
            var res2 = ConsumeChars(cctx, InstrBodyPredicate);

            var result = ConcatToString(res1, res2);
            if (result.Error == null)
            {
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Immediate prefix consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<bool> ConsumeImmediatePrefix(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res = ConsumeChars(cctx, c => c == '#', 1, 1);

            var result = new ParserResult<bool>();
            result.Error = res.Error;
            if (res.Error == null)
            {
                result.Data = true;
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Operands separator consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<bool> ConsumeOperandSeparator(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res = ConsumeChars(cctx, c => c == ',', 1, 1);

            var result = new ParserResult<bool>();
            result.Error = res.Error;
            if (res.Error == null)
            {
                result.Data = true;
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Generic register reference consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeRegister(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, c => c == '$', 1, 1);
            var res2 = ConsumeChars(cctx, IdStartPredicate, 1);
            var res3 = ConsumeChars(cctx, IdBodyPredicate);

            var result = ConcatToString(res2, res3);
            result.Error = res1.Error ?? result.Error;
            if (result.Error == null)
            {
                pctx.Index = cctx.Index;
            }

            return result;
        }


        /// <summary>
        /// Boolean constant consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<bool> ConsumeBooleanValue(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res = ConsumeLiteral(cctx);

            var result = new ParserResult<bool>();
            result.Error = res.Error;
            if (res.Error == null)
            {
                switch (res.Data.ToLowerInvariant())
                {
                    case "false":
                        result.Data = false;
                        pctx.Index = cctx.Index;
                        break;

                    case "true":
                        result.Data = true;
                        pctx.Index = cctx.Index;
                        break;

                    default:
                        result.Error = new CompilationException($"Unable to recognize as boolean constant: {res.Data}");
                        break;
                }
            }

            return result;
        }


        /// <summary>
        /// Numeric constant consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<double> ConsumeNumericValue(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, c => c == '-', 0, 1);
            var res2 = ConsumeChars(cctx, c => char.IsDigit(c), 1);
            var res3 = ConsumeChars(cctx, c => c == '.', 0, 1);
            var res4 = ConsumeChars(cctx, c => char.IsDigit(c));
            var resString = ConcatToString(res1, res2, res3, res4);

            var result = new ParserResult<double>();
            result.Error = resString.Error;
            if (resString.Error == null)
            {
                double value;
                if (double.TryParse(resString.Data, out value))
                {
                    result.Data = value;
                    pctx.Index = cctx.Index;
                }
                else
                {
                    result.Error = new CompilationException($"Unable to parse as a number: {resString.Data}");
                }
            }

            return result;
        }


        /// <summary>
        /// String constant consumer
        /// </summary>
        /// <param name="pctx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConsumeStringValue(
            ParserContext pctx
            )
        {
            var cctx = pctx.CreateChild();
            var res1 = ConsumeChars(cctx, c => c == '"', 1, 1);
            var res2 = ConsumeChars(cctx, c => c != '"');
            var res3 = ConsumeChars(cctx, c => c == '"', 1, 1);
            var result = ConcatToString(res2);
            if (result.Error == null) pctx.Index = cctx.Index;
            return result;
        }


        /// <summary>
        /// Generic character consumer
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="predicate"></param>
        /// <param name="minCount"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        private static ParserResult<IReadOnlyCollection<char>> ConsumeChars(
            ParserContext ctx,
            Func<char, bool> predicate,
            int minCount = 0,
            int maxCount = int.MaxValue
            )
        {
            if (minCount > maxCount)
            {
                throw new ArgumentException(
                    $"Invalid count constraint: min={minCount}; max={maxCount}"
                    );
            }

            var acc = new List<char>();
            int index = ctx.Index - 1;
            while (++index < ctx.Buffer.Length)
            {
                char c = ctx.Buffer[index];
                if (predicate(c) == false) break;
                acc.Add(c);
                //if (count >= maxCount) break;
            }

            var result = new ParserResult<IReadOnlyCollection<char>>();
            if (acc.Count < minCount)
            {
                result.Error = new CompilationException("Not enough data to consume.");
            }
            else if (acc.Count > maxCount)
            {
                result.Error = new CompilationException("Too many data to consume.");
            }
            else
            {
                result.ConsumedCount = index - ctx.Index;
                result.Data = acc;
                ctx.Index = index;
            }

            return result;
        }


        /// <summary>
        /// Helping function to join one or more character arrays into a single string
        /// </summary>
        /// <param name="list0"></param>
        /// <param name="listx"></param>
        /// <returns></returns>
        private static ParserResult<string> ConcatToString(
            ParserResult<IReadOnlyCollection<char>> list0,
            params ParserResult<IReadOnlyCollection<char>>[] listx
            )
        {
            var result = new ParserResult<string>();
            result.Error = list0.Error ?? listx
                .Select(_ => _.Error)
                .Where(_ => _ != null)
                .FirstOrDefault();

            if (result.Error == null)
            {
                var chain = list0.Data
                    .Concat(listx.SelectMany(_ => _.Data));

                var data = chain.ToArray();
                result.Data = new string(data);
                result.ConsumedCount = data.Length;
            }

            return result;
        }

    }
}
