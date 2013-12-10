﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports.TemplateExtensions.EvalComponents
{
    /// <summary>
    /// Custom exception for evaluation errors
    /// </summary>
    public class EvalException : Exception
    {
        /// <summary>
        /// Zero-base position in expression where exception occurred
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message that describes this exception</param>
        /// <param name="position">Position within expression where exception occurred</param>
        public EvalException(string message, int position)
            : base(message)
        {
            Column = position;
        }

        /// <summary>
        /// Gets the message associated with this exception
        /// </summary>
        public override string Message
        {
            get
            {
                return String.Format("{0} (column {1})", base.Message, Column + 1);
            }
        }
    }

    internal enum SymbolStatus
    {
        OK,
        UndefinedSymbol,
    }

    // ProcessSymbol arguments
    internal class SymbolEventArgs : EventArgs
    {
        public string Name { get; set; }
        public double Result { get; set; }
        public SymbolStatus Status { get; set; }
    }

    internal enum FunctionStatus
    {
        OK,
        UndefinedFunction,
        WrongParameterCount,
    }

    // ProcessFunction arguments
    internal class FunctionEventArgs : EventArgs
    {
        public string Name { get; set; }
        public List<double> Parameters { get; set; }
        public double Result { get; set; }
        public FunctionStatus Status { get; set; }
    }

    /// <summary>
    /// Expression evaluator class
    /// </summary>
    internal class Eval
    {
        // Event handers
        public delegate void ProcessSymbolHandler(SymbolEventArgs e);
        public delegate void ProcessFunctionHandler(FunctionEventArgs e);

        // Token state enums
        protected enum State
        {
            None = 0,
            Operand = 1,
            Operator = 2,
            UnaryOperator = 3
        }

        // Error messages
        protected const string ErrInvalidOperand = "Invalid operand";
        protected const string ErrOperandExpected = "Operand expected";
        protected const string ErrOperatorExpected = "Operator expected";
        protected const string ErrUnmatchedClosingParen = "Closing parenthesis without matching open parenthesis";
        protected const string ErrMultipleDecimalPoints = "Operand contains multiple decimal points";
        protected const string ErrUnexpectedCharacter = "Unexpected character encountered \"{0}\"";
        protected const string ErrUndefinedSymbol = "Undefined symbol \"{0}\"";
        protected const string ErrUndefinedFunction = "Undefined function \"{0}\"";
        protected const string ErrClosingParenExpected = "Closing parenthesis expected";
        protected const string ErrWrongParamCount = "Wrong number of function parameters";

        // To distinguish it from a minus operator,
        // we'll use a character unlikely to appear
        // in expressions to signify a unary negative
        protected const string UnaryMinus = "\x80";

        //
        private Eval()
        {
        }

        public static double Execute(string expression)
        {
            return Execute(expression, null, null);
        }

        public static double Execute(string expression, ProcessSymbolHandler ProcessSymbol)
        {
            return Execute(expression, ProcessSymbol, null);
        }

        public static double Execute(string expression, ProcessFunctionHandler ProcessFunction)
        {
            return Execute(expression, null, ProcessFunction);
        }

        /// <summary>
        /// Evaluates the given expression and returns the result
        /// </summary>
        /// <param name="expression">The expression to evaluate</param>
        /// <returns></returns>
        public static double Execute(string expression, ProcessSymbolHandler ProcessSymbol, ProcessFunctionHandler ProcessFunction)
        {
            return ExecuteTokens(TokenizeExpression(expression, ProcessSymbol, ProcessFunction));
        }

        /// <summary>
        /// Converts a standard infix expression to list of tokens in
        /// postfix order.
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns></returns>
        protected static List<string> TokenizeExpression(string expression, ProcessSymbolHandler ProcessSymbol, ProcessFunctionHandler ProcessFunction)
        {
            List<string> tokens = new List<string>();
            Stack<string> stack = new Stack<string>();
            State state = State.None;
            int parenCount = 0;
            string temp;

            TextParser parser = new TextParser(expression);

            while (!parser.EndOfText)
            {
                if (Char.IsWhiteSpace(parser.Peek()))
                {
                    // Ignore spaces, tabs, etc.
                }
                else
                {
                    switch (parser.Peek())
                    {
                        case '(':
                            // Cannot follow operand
                            if (state == State.Operand)
                                throw new EvalException(ErrOperatorExpected, parser.Position);
                            // Allow additional unary operators after "("
                            if (state == State.UnaryOperator)
                                state = State.Operator;
                            // Push opening parenthesis onto stack
                            stack.Push(parser.Peek().ToString());
                            // Track number of parentheses
                            parenCount++;
                            break;
                        case ')':
                            // Must follow operand
                            if (state != State.Operand)
                                throw new EvalException(ErrOperandExpected, parser.Position);
                            // Must have matching open parenthesis
                            if (parenCount == 0)
                                throw new EvalException(ErrUnmatchedClosingParen, parser.Position);
                            // Pop all operators until matching "(" found
                            temp = stack.Pop();
                            while (temp != "(")
                            {
                                tokens.Add(temp);
                                temp = stack.Pop();
                            }
                            // Track number of parentheses
                            parenCount--;
                            break;
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                            if (state == State.Operand)
                            {
                                // Pop operators with precedence >= current operator
                                int currPrecedence = GetPrecedence(parser.Peek().ToString());
                                while (stack.Count > 0 && GetPrecedence(stack.Peek()) >= currPrecedence)
                                    tokens.Add(stack.Pop());
                                stack.Push(parser.Peek().ToString());
                                state = State.Operator;
                            }
                            else if (state == State.UnaryOperator)
                            {
                                // Don't allow two unary operators together
                                throw new EvalException(ErrOperandExpected, parser.Position);
                            }
                            else
                            {
                                // Test for unary operator
                                if (parser.Peek() == '-')
                                {
                                    // Push unary minus
                                    stack.Push(UnaryMinus);
                                    state = State.UnaryOperator;
                                }
                                else if (parser.Peek() == '+')
                                {
                                    // Just ignore unary plus
                                    state = State.UnaryOperator;
                                }
                                else
                                {
                                    throw new EvalException(ErrOperandExpected, parser.Position);
                                }
                            }
                            break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '.':
                            if (state == State.Operand)
                            {
                                // Cannot follow other operand
                                throw new EvalException(ErrOperatorExpected, parser.Position);
                            }
                            // Parse number
                            temp = ParseNumberToken(parser);
                            tokens.Add(temp);
                            state = State.Operand;
                            if (parser.Peek() != temp[temp.Length - 1])
                                parser.MoveAhead(-1);
                            break;
                        default:
                            double result;

                            // Parse symbols and functions
                            if (state == State.Operand)
                            {
                                // Symbol or function cannot follow other operand
                                throw new EvalException(ErrOperatorExpected, parser.Position);
                            }
                            if (!(Char.IsLetter(parser.Peek()) || parser.Peek() == '_'))
                            {
                                // Invalid character
                                temp = String.Format(ErrUnexpectedCharacter, parser.Peek());
                                throw new EvalException(temp, parser.Position);
                            }
                            // Save start of symbol for error reporting
                            int symbolPos = parser.Position;
                            // Parse this symbol
                            temp = ParseSymbolToken(parser);
                            // Skip whitespace
                            parser.MovePastWhitespace();
                            // Check for parameter list
                            if (parser.Peek() == '(')
                            {
                                // Found parameter list, evaluate function
                                result = EvaluateFunction(parser, temp, symbolPos, ProcessFunction);
                            }
                            else
                            {
                                // No parameter list, evaluate symbol (variable)
                                result = EvaluateSymbol(temp, symbolPos, ProcessSymbol);
                            }
                            // Handle negative result
                            if (result < 0)
                            {
                                stack.Push(UnaryMinus);
                                result = Math.Abs(result);
                            }
                            tokens.Add(result.ToString());
                            state = State.Operand;
                            break;
                    }
                }
                parser.MoveAhead();
            }
            // Expression cannot end with operator
            if (state == State.Operator || state == State.UnaryOperator)
                throw new EvalException(ErrOperandExpected, parser.Position);
            // Check for balanced parentheses
            if (parenCount > 0)
                throw new EvalException(ErrClosingParenExpected, parser.Position);
            // Retrieve remaining operators from stack
            while (stack.Count > 0)
                tokens.Add(stack.Pop());
            return tokens;
        }

        /// <summary>
        /// Parses and extracts a numeric value at the current position
        /// </summary>
        /// <param name="parser">TextParser object</param>
        /// <returns></returns>
        protected static string ParseNumberToken(TextParser parser)
        {
            bool hasDecimal = false;
            int start = parser.Position;
            while (Char.IsDigit(parser.Peek()) || parser.Peek() == '.')
            {
                if (parser.Peek() == '.')
                {
                    if (hasDecimal)
                        throw new EvalException(ErrMultipleDecimalPoints, parser.Position);
                    hasDecimal = true;
                }
                parser.MoveAhead();
            }
            // Extract token
            string token = parser.Extract(start, parser.Position);
            if (token == ".")
                throw new EvalException(ErrInvalidOperand, parser.Position - 1);
            return token;
        }

        /// <summary>
        /// Parses and extracts a symbol at the current position
        /// </summary>
        /// <param name="parser">TextParser object</param>
        /// <returns></returns>
        protected static string ParseSymbolToken(TextParser parser)
        {
            int start = parser.Position;
            while (Char.IsLetterOrDigit(parser.Peek()) || parser.Peek() == '_')
                parser.MoveAhead();
            return parser.Extract(start, parser.Position);
        }

        /// <summary>
        /// Evaluates a function and returns its value. It is assumed the current
        /// position is at the opening parenthesis of the argument list.
        /// </summary>
        /// <param name="parser">TextParser object</param>
        /// <param name="name">Name of function</param>
        /// <param name="pos">Position at start of function</param>
        /// <returns></returns>
        protected static double EvaluateFunction(TextParser parser, string name, int pos, ProcessFunctionHandler ProcessFunction)
        {
            double result = default(double);

            // Parse function parameters
            List<double> parameters = ParseParameters(parser);

            // We found a function reference
            FunctionStatus status = FunctionStatus.UndefinedFunction;
            if (ProcessFunction != null)
            {
                FunctionEventArgs args = new FunctionEventArgs();
                args.Name = name;
                args.Parameters = parameters;
                args.Result = result;
                args.Status = FunctionStatus.OK;
                ProcessFunction(args);
                result = args.Result;
                status = args.Status;
            }
            if (status == FunctionStatus.UndefinedFunction)
                throw new EvalException(String.Format(ErrUndefinedFunction, name), pos);
            if (status == FunctionStatus.WrongParameterCount)
                throw new EvalException(ErrWrongParamCount, pos);
            return result;
        }

        /// <summary>
        /// Evaluates each parameter of a function's parameter list and returns
        /// a list of those values. An empty list is returned if no parameters
        /// were found. It is assumed the current position is at the opening
        /// parenthesis of the argument list.
        /// </summary>
        /// <param name="parser">TextParser object</param>
        /// <returns></returns>
        protected static List<double> ParseParameters(TextParser parser)
        {
            // Move past open parenthesis
            parser.MoveAhead();

            // Look for function parameters
            List<double> parameters = new List<double>();
            parser.MovePastWhitespace();
            if (parser.Peek() != ')')
            {
                // Parse function parameter list
                int paramStart = parser.Position;
                int parenCount = 1;

                while (!parser.EndOfText)
                {
                    if (parser.Peek() == ',')
                    {
                        // Note: Ignore commas inside parentheses. They could be
                        // from a parameter list for a function inside the parameters
                        if (parenCount == 1)
                        {
                            parameters.Add(EvaluateParameter(parser, paramStart));
                            paramStart = parser.Position + 1;
                        }
                    }
                    if (parser.Peek() == ')')
                    {
                        parenCount--;
                        if (parenCount == 0)
                        {
                            parameters.Add(EvaluateParameter(parser, paramStart));
                            break;
                        }
                    }
                    else if (parser.Peek() == '(')
                    {
                        parenCount++;
                    }
                    parser.MoveAhead();
                }
            }
            // Make sure we found a closing parenthesis
            if (parser.Peek() != ')')
                throw new EvalException(ErrClosingParenExpected, parser.Position);
            // Move past closing parenthesis
            parser.MoveAhead();
            // Return parameter list
            return parameters;
        }

        /// <summary>
        /// Extracts and evaluates a function parameter and returns its value. If an
        /// exception occurs, it is caught and the column is adjusted to reflect the
        /// position in original string, and the exception is rethrown.
        /// </summary>
        /// <param name="parser">TextParser object</param>
        /// <param name="paramStart">Column where this parameter started</param>
        /// <returns></returns>
        protected static double EvaluateParameter(TextParser parser, int paramStart)
        {
            try
            {
                // Extract expression and evaluate it
                string expression = parser.Extract(paramStart, parser.Position);
                return Execute(expression);
            }
            catch (EvalException ex)
            {
                // Adjust column and rethrow exception
                ex.Column += paramStart;
                throw ex;
            }
        }

        /// <summary>
        /// This method evaluates a symbol name and returns its value.
        /// </summary>
        /// <param name="name">Name of symbol</param>
        /// <param name="pos">Position at start of symbol</param>
        /// <returns></returns>
        protected static double EvaluateSymbol(string name, int pos, ProcessSymbolHandler ProcessSymbol)
        {
            double result = default(double);

            // We found a symbol reference
            SymbolStatus status = SymbolStatus.UndefinedSymbol;
            if (ProcessSymbol != null)
            {
                SymbolEventArgs args = new SymbolEventArgs();
                args.Name = name;
                args.Result = result;
                args.Status = SymbolStatus.OK;
                ProcessSymbol(args);
                result = args.Result;
                status = args.Status;
            }
            if (status == SymbolStatus.UndefinedSymbol)
                throw new EvalException(String.Format(ErrUndefinedSymbol, name), pos);
            return result;
        }

        /// <summary>
        /// Evaluates the given list of tokens and returns the result.
        /// Tokens must appear in postfix order.
        /// </summary>
        /// <param name="tokens">List of tokens to evaluate.</param>
        /// <returns></returns>
        protected static double ExecuteTokens(List<string> tokens)
        {
            Stack<double> stack = new Stack<double>();
            double tmp, tmp2;

            foreach (string token in tokens)
            {
                switch (token[0])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                        stack.Push(double.Parse(token));
                        break;
                    case '+':
                        stack.Push(stack.Pop() + stack.Pop());
                        break;
                    case '-':
                        tmp = stack.Pop();
                        tmp2 = stack.Pop();
                        stack.Push(tmp2 - tmp);
                        break;
                    case '*':
                        stack.Push(stack.Pop() * stack.Pop());
                        break;
                    case '/':
                        tmp = stack.Pop();
                        tmp2 = stack.Pop();
                        stack.Push(tmp2 / tmp);
                        break;
                    default:
                        if (token == UnaryMinus)
                            stack.Push(-stack.Pop());
                        break;
                }
            }
            // Remaining item on stack contains result
            return (stack.Count > 0) ? stack.Pop() : 0.0;
        }

        /// <summary>
        /// Returns a value that indicates the relative precedence of
        /// the specified operator
        /// </summary>
        /// <param name="s">Operator to be tested</param>
        /// <returns></returns>
        protected static int GetPrecedence(string s)
        {
            switch (s)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                    return 3;
                case UnaryMinus:
                    return 10;
            }
            return 0;
        }
    }
}
