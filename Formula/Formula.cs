// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 
// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private string formula;
        private List<string> formulatokens;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// normalize takes in a string, returns a string
        /// isValid takes in a string, returns a boolean
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            VerifySyntax(formula);             // syntax errors verification

            // variable verification, if any. 
            Regex r = new Regex(@"[a-zA-Z_](?: [a-zA-Z_]|\d)*");

            string token;
            for (int i = 0; i < formulatokens.Count; i++)   // POTENTIAL BUG TO BE FIXED
            {
                if (r.Match(formulatokens[i]).Success) // is a variable
                {
                    try
                    {
                        token = normalize(formulatokens[i]);
                    }
                    catch (Exception e)
                    {
                        throw new FormulaFormatException("Error while trying to normalize " + formulatokens[i] +
                            " in " + formula + ". \n Stack Trace: " + e.StackTrace);
                    }

                    if (!isValid(token)) //  attempts to validate             // STILL NEED TO CATCH EXCEPTIONS
                        throw new FormulaFormatException(token + " is not valid inside of the formula: " + formula);
                    else
                        formulatokens[i] = token;   // reset the token to the normalized one. 
                }
            }

            this.formula = ConvertListToFormula(formulatokens);   // convert the normalized formulalist tostring object.     
        }

        /// <summary>
        /// Verify the syntax of formula and throw FormulaSyntaxException when the formula is in invalid form. 
        /// Parsing: 
        ///     We have provided a private method that will convert an input string into tokens. 
        ///     After tokenizing, your code should verify that the only tokens 
        ///     are (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation)
        /// One Token Rule: 
        ///     There must be at least one token.
        /// Right Parentheses Rule: 
        ///     When reading tokens from left to right, at no point should the number of closing parentheses seen so far be greater than the number of opening parentheses seen so far.
        /// Balanced Parentheses Rule: 
        ///     The total number of opening parentheses must equal the total number of closing parentheses.
        /// Starting Token Rule: 
        ///     The first token of an expression must be a number, a variable, or an opening parenthesis.
        /// Ending Token Rule: 
        ///     The last token of an expression must be a number, a variable, or a closing parenthesis.
        /// Parenthesis/Operator Following Rule: 
        ///     Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.
        /// Extra Following Rule: 
        ///     Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.  
        /// </summary>
        private void VerifySyntax(string formula)
        {
            this.formula = formula ?? throw new FormulaFormatException("The formula is null. ");
            formulatokens = new List<string>(GetTokens(formula));

            if (formula.Equals(""))     // One token Rule   
            {
                throw new FormulaFormatException("The formula is empty. ");
            }
            else if (formulatokens.Count == 0)
            {
                throw new FormulaFormatException("The formula does not contain any useful token. ");
            }

            int leftparamcount = 0, rightparamcount = 0;
            string tokens = formulatokens[0];
            string tokentype = Determinetype(tokens);

            if (!tokentype.Equals("left param") && !tokentype.Equals("variable") && !tokentype.Equals("value")) // starting token rule 
            {
                throw new FormulaFormatException("The starting token of " + formula + " is " + tokens + " of " + tokentype + ". " +
                    "The only allowed starting token are value, variable, or \'(\'         ");
            }
            else
            {
                for (int i = 0; i < formulatokens.Count - 1; i++)
                {
                    tokens = formulatokens[i];
                    tokentype = Determinetype(tokens);
                    if (tokentype.Equals("left param"))
                    {
                        tokentype = Determinetype(formulatokens[i + 1]);
                        if (!tokentype.Equals("left param") && !tokentype.Equals("value") && !tokentype.Equals("variable"))  // parenthesis/operator following rule
                            throw new FormulaFormatException("The token following \'(\' in " + formula + " is " + formulatokens[i + 1] + " of token type " + tokentype +
                                ". This is invalid as only \'(\', value, or variable are allowed to be after \'(\'.         ");
                        leftparamcount++;
                    }
                    else if (tokentype.Equals("right param"))
                    {
                        rightparamcount++;
                        if (rightparamcount > leftparamcount)   // right param rule
                        {
                            throw new FormulaFormatException("Inside of formula " + formula + " there exist invalid \')\'.       ");
                        }

                        tokentype = Determinetype(formulatokens[i + 1]);
                        if (!tokentype.Equals("operator") && !tokentype.Equals("right param"))  // extra following rule 
                        {
                            throw new FormulaFormatException("The token following \')\' in " + formula + " is " + formulatokens[i + 1] + " of token type " + tokentype +
                                ". This is invalid as only \')\' or operator are allowed to be after a \')\'.         ");
                        }
                    }
                    else if (tokentype.Equals("operator"))
                    {
                        tokentype = Determinetype(formulatokens[i + 1]);
                        if (!tokentype.Equals("left param") && !tokentype.Equals("value") && !tokentype.Equals("variable"))    // parenthesis/operator following rule
                            throw new FormulaFormatException("The token following \'" + tokens + "\' in " + formula + " is " + formulatokens[i + 1] + " of token type " + tokentype +
                                ". This is invalid as only \'(\', value, or variable are allowed to be after \'" + tokens + "\' .         ");
                    }
                    else if (tokentype.Equals("value"))
                    {
                        tokentype = Determinetype(formulatokens[i + 1]);
                        if (!tokentype.Equals("operator") && !tokentype.Equals("right param"))  // extra following rule
                        {
                            throw new FormulaFormatException("The token following \'" + tokens + "\' in " + formula + " is " + formulatokens[i + 1] + " of token type " + tokentype +
                                ". This is invalid as only \')\' or operator are allowed to be after a \'" + tokens + "\'.         ");
                        }
                    }
                    else if (tokentype.Equals("variable"))
                    {
                        tokentype = Determinetype(formulatokens[i + 1]);
                        if (!tokentype.Equals("operator") && !tokentype.Equals("right param")) // extra following rule
                        {
                            throw new FormulaFormatException("The token following \'" + tokens + "\' in " + formula + " is " + formulatokens[i + 1] + " of token type " + tokentype +
                                ". This is invalid as only \')\' or operator are allowed to be after a \'" + tokens + "\'.         ");
                        }
                    }
                    else
                    {
                        //         parsing  
                        throw new FormulaFormatException("Invalid Token: " + tokens + " is not allowed to be in the formula " + formula);
                    }
                }

                tokens = formulatokens[formulatokens.Count - 1];  // last token  
                tokentype = Determinetype(tokens);
                if (!tokentype.Equals("value") && !tokentype.Equals("variable") && !tokentype.Equals("right param"))  // ending token rule   
                    throw new FormulaFormatException("The last token of " + formula + " is " + tokens + " of " + tokentype + ". " +
                        "The only allowed starting token are number, variable, or \'(\'         ");
                else
                {
                    if (tokentype.Equals("right param"))
                    {
                        rightparamcount++;

                        if (rightparamcount > leftparamcount)   // right param rule
                        {
                            throw new FormulaFormatException("Inside of formula " + formula + " there exist invalid \')\'.       ");
                        }
                    }

                    if (rightparamcount != leftparamcount)    // balanced parenthesis rule
                    {
                        throw new FormulaFormatException("Invalid total amount of parameters in formula " + formula +
                            ". The total amount of \'(\' are " + leftparamcount + " and the total amount of \')\' are " + rightparamcount);
                    }
                }
            }
        }

        /// <summary>
        /// Determine the type of a String, whether it is value, variable, or operator, or invalid String
        /// if the string is an int, then it is a value
        /// if the string is a Letter + int, then it is a variable
        /// if the string is (, ), /, *, +, or -, then it is an operator
        /// Else, the string is invalid 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static String Determinetype(String s)
        {
            string digitpattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            string varpattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"; 
            string type; 

            if (s[0] == '_' || Char.IsLetter(s[0]))
            {
                if (new Regex(varpattern, RegexOptions.IgnorePatternWhitespace).Match(s).Success)    // variable
                    type = "variable";
                else
                    type = "value"; 
                return type;
            }
            else
            {
                if (new Regex(digitpattern, RegexOptions.IgnorePatternWhitespace).Match(s).Success)
                    type = "value"; 
                else if (s.Equals("("))     // left/closing parameter
                    type = "left param";
                else if (s.Equals(")"))     // right/closing parameter
                    type = "right param";
                else if (s.Equals("/") || s.Equals("*") || s.Equals("+") || s.Equals("-"))  // operators
                    type = "operator";
                else
                    type = "bad token";
                return type;
            }
        }

        /// <summary>
        /// Helper method that helps converting list into a formula by 
        /// concatinating members of a list into a string
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string ConvertListToFormula(List<string> list)
        {
            // Console.WriteLine("list size: " + list.Count);
            string str = "";
            foreach (string s in list)
                str += s;



            return str;
        }


        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        /// 
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> variableEvaluator)
        {
            try
            {
                Stack<string> ops = new Stack<string>();
                Stack<double> val = new Stack<double>();

                string s, tokentype;
                for (int idx = 0; idx < formulatokens.Count; idx++)   // construct Stacks
                {
                    s = formulatokens[idx];
                    if (s.Equals(""))
                        continue;

                    tokentype = Determinetype(s);
                    if (tokentype.Equals("operator") || tokentype.Equals("left param") || tokentype.Equals("right param"))
                    {
                        // Console.WriteLine("NEW Operator: " + s);

                        if (s.Equals("-") || s.Equals("+"))
                        {
                            if (ops.Count != 0 && (ops.Peek().Equals("-") || ops.Peek().Equals("+")))
                            {
                                if (val.Count >= 2)
                                    val.Push((Double)Doarith(val.Pop(), ops.Pop(), val.Pop()));
                            }
                            ops.Push(s);
                        }
                        else if (s.Equals("/") || s.Equals("*"))
                        {
                            ops.Push(s);
                        }
                        else if (s.Equals("("))
                        {
                            ops.Push(s);
                        }
                        else if (s.Equals(")"))
                        {
                            if (ops.Count != 0 && (ops.Peek().Equals("-") || ops.Peek().Equals("+")))
                            {
                                if (val.Count >= 2)
                                    val.Push((Double)Doarith(val.Pop(), ops.Pop(), val.Pop()));
                            }

                            if (ops.Count != 0 && ops.Peek().Equals("(")) // next element should be (
                            {
                                ops.Pop();
                            }

                            if (ops.Count != 0 && (ops.Peek().Equals("/") || ops.Peek().Equals("*")))
                            {
                                if (val.Count >= 2)
                                {
                                    Object tmp = Doarith(val.Pop(), ops.Pop(), val.Pop()); // for detecting division error   
                                    if (tmp.GetType().Equals(typeof(FormulaError)))
                                        return tmp;     // return formula error 
                                    else
                                        val.Push((Double)tmp);
                                }

                            }
                        }
                    }
                    else
                    {
                        double valueInDouble;
                        if (tokentype.Equals("variable")) // s is a variable 
                        {
                            valueInDouble = variableEvaluator(s); // convert the variable into String
                        }
                        else // if (tokentype.Equals("value"))  // s is a value
                        {
                            valueInDouble = Double.Parse(s);
                        }


                        if (ops.Count != 0 && (ops.Peek().Equals("*") || ops.Peek().Equals("/")))
                        {
                            Object tmp = Doarith(valueInDouble, ops.Pop(), val.Pop()); // for detecting division error   
                            if (tmp.GetType().Equals(typeof(FormulaError)))
                            {
                                return tmp;     // return a formula error
                            }
                            else
                            {
                                val.Push((Double)tmp);   // push the result of s operation with the top value of val. 
                            }
                        }
                        else
                        {
                            val.Push(valueInDouble);
                        }

                    }
                }

                if (ops.Count == 0 && val.Count == 1)
                {
                    return val.Pop();
                }
                else // if (ops.Count == 1 && val.Count == 2)
                {
                    return Doarith(val.Pop(), ops.Pop(), val.Pop());
                }
            }
            catch(Exception e)
            {
                return new FormulaError(e.Message); 
            }

            // return new FormulaError("Unknown Error"); 
        }

        /// <summary>
        /// Do Arithmatic with operand a and b, and operator o. 
        /// if o is + or *, then return a+b or a*b 
        /// if o is - or /, then return b-a or b/a
        /// if a is 0 when o is /, then throw an Arithmatic Exception because cannot divide by 0. 
        /// </summary>
        /// <param name="a">first operator </param>
        /// <param name="o">operand </param>
        /// <param name="b">second operator </param>
        /// <returns>the result of the operation or formula error</returns>
        private static Object Doarith(double a, string o, double b)
        {
            if (o.Equals("+"))
                return a + b;
            else if (o.Equals("-"))
                return b - a;
            else if (o.Equals("*"))
                return a * b;
            else // if (o.Equals("/"))
                if (a == 0)
                return new FormulaError("DIV 0 Error");
            else
                return b / a;
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> vars = new HashSet<string>();
            Regex r = new Regex(@"[a-zA-Z_](?: [a-zA-Z_]|\d)*");

            foreach (string s in formulatokens)
                if (r.Match(s).Success)  // match the variable format
                    vars.Add(s);

            return vars;
        }



        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return formula;
        }


        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null || !obj.GetType().Equals(typeof(Formula)))
            {
                return false;
            }
            else
            {
                Formula objformula = (Formula)obj;
                List<string> formulalist = objformula.formulatokens;
                string formulatoken1, formulatoken2;
                if (formulalist.Count == formulatokens.Count)   // length are the same
                {
                    for (int i = 0; i < formulalist.Count; i++)  // loop through individual indices of the both tokens list. 
                    {
                        formulatoken1 = formulalist[i];
                        formulatoken2 = formulatokens[i];
                        string type1 = Determinetype(formulatoken1);
                        string type2 = Determinetype(formulatoken2); 
                        if (type1.Equals(type2))  // same type for each token in the same index
                        {
                            if (type1.Equals("value"))   // the type is a value
                            {
                                if (Double.Parse(formulatoken1).ToString() != Double.Parse(formulatoken2).ToString()) // double are not equal
                                {
                                    return false;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else  // type is (, ), operators, variables 
                            {
                                if (!formulatoken1.Equals(formulatoken2)) // tokens not matching
                                    return false;
                            }
                        }
                        else  // tokens have different type
                        {
                            return false;
                        }
                    }
                }
                else   // tokens have different type
                {
                    return false;
                }
            }

            return true;
        }



        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
                return true;   // both are null
            else if ((f1 is null && f2 is Formula) || f1 is Formula && f2 is null)
                return false;  // either one is null
            else
                return (f1.Equals(f2));
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }


        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int value = 0;
            string strtobehashed;
            foreach (string token in formulatokens)
            {
                strtobehashed = token;
                if (Determinetype(token).Equals("value"))
                    strtobehashed = Double.Parse(token).ToString();

                for (int i = 0; i < strtobehashed.Length; i++)
                {
                    value += token[i];
                }
            }
            return value;
        }


        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);



            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }


    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        { }
    }


    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason) : this()
        { Reason = reason;  }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}