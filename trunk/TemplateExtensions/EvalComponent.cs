using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate;
using System.Text.RegularExpressions;
using Org.Reddragonit.PDFReports.TemplateExtensions.EvalComponents;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.PDFReports.TemplateExtensions
{
    class EvalComponent : IComponent
    {
        private static readonly Regex regMatch = new Regex("^(E|e)(V|v)(A|a)(L|l)\\((.+)\\)$",RegexOptions.Compiled | RegexOptions.ECMAScript);

        private List<IComponent> _expression;

        #region IComponent Members

        public bool CanLoad(Token token)
        {
            return regMatch.IsMatch(token.Content);
        }

        public bool Load(Queue<Token> tokens, Type tokenizerType, TemplateGroup group)
        {
            Token tok = tokens.Dequeue();
            Match m = regMatch.Match(tok.Content);
            _expression = ((Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { m.Groups[5].Value })).TokenizeStream(group);
            return true;
        }

        public IComponent NewInstance()
        {
            return new EvalComponent();
        }

        public void Append(ref Dictionary<string, object> variables, IOutputWriter writer)
        {
            StringOutputWriter sow = new StringOutputWriter();
            foreach (IComponent comp in _expression)
                comp.Append(ref variables, sow);
            writer.Append(Eval.Execute(sow.ToString()).ToString());
        }

        #endregion
    }
}
