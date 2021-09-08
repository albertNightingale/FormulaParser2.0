using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTester
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestDefaultConstructorWOVar()
        {
            Formula formula = new Formula("3 + 2");
            
            Assert.AreEqual(5, (Double)(formula.Evaluate(ConvertVar)), 1e8); 
        }

        [TestMethod]
        public void TestConstructorWoVar()
        {
            Formula formula = new Formula("3 + 2", NormalizeVar, ValidateVar);

            Assert.AreEqual(5, (Double)(formula.Evaluate(ConvertVar)), 1e8);
        }

        [TestMethod]
        public void TestDefaultConstructorWVar()
        {
            Formula formula = new Formula("a7 + 2");

            Assert.AreEqual(7, (Double)(formula.Evaluate(ConvertVar)), 1e8);
        }

        [TestMethod]
        public void TestConstructorWVar()
        {
            Formula formula = new Formula("A3 + 2", NormalizeVar, ValidateVar);

            Assert.AreEqual(7, (Double)(formula.Evaluate(ConvertVar)), 1e8);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWithInvalidVar()
        {
            Formula formula = new Formula("A_3 + 2", NormalizeVar, ValidateVar);
        }

        [TestMethod]
        public void TestDefaultConstructorWNormalizer()
        {
            Formula formula = new Formula("A3 +                         B2", NormalizeVar, ValidateVar);

            Assert.AreEqual("a3+b2", formula.ToString());
        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWParsingException()
        {
            Formula formula = new Formula("3 ~.~ 2", NormalizeVar, ValidateVar);

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWParsingException1()
        {
            Formula formula = new Formula("AB10 - 2", NormalizeVar, ValidateVar);

        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWEmptyFormula()
        {
            Formula formula = new Formula("", NormalizeVar, ValidateVar);

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWNullFormula()
        {
            Formula formula = new Formula(null, NormalizeVar, ValidateVar);

        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWNonParsableFormula()
        {
            Formula formula = new Formula("                                      ", NormalizeVar, ValidateVar);

        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWhenMoreRightParamThanLeft()
        {
            Formula formula = new Formula("(2 + 3) * 5)))", NormalizeVar, ValidateVar);
        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorWUnbalancedParameter()
        {
            Formula formula = new Formula("((2 + 3) * 5) + 5))", NormalizeVar, ValidateVar);

        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorStartingFormulaWithRightParam()
        {
            Formula formula = new Formula(")1*3", NormalizeVar, ValidateVar);

        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorStartingFormulaWithOperator()
        {
            Formula formula = new Formula("* 5 + 7", NormalizeVar, ValidateVar);

        }

        

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorEndingFormulaWithOperator()
        {
            Formula formula = new Formula("3+5*", NormalizeVar, ValidateVar);

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorEndingFormulaLeftParam()
        {
            Formula formula = new Formula("2*2(", NormalizeVar, ValidateVar);

        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowLeftParamInFormulaWithRightParam()
        {
            Formula formula = new Formula("3 + ()", NormalizeVar, ValidateVar);

        }
        


        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowLeftParamInFormulaWithOperator()
        {
            Formula formula = new Formula("(+3 + 6)", NormalizeVar, ValidateVar);

        }

        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowOperatorInFormulaWithOperator()
        {
             Formula formula = new Formula("3+7 + + 6", NormalizeVar, ValidateVar);

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowOperatorInFormulaWithRightParam()
        {
            Formula formula = new Formula("3+7 + ) 6 + 6", NormalizeVar, ValidateVar);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowNumberInFormulaWithLeftParam()
        {
            Formula formula = new Formula("8 * 7 (   46 - 5 ", NormalizeVar, ValidateVar);

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowNumberInFormulaWithVar()
        {
            Formula formula = new Formula("4 -  3     A8 * 6", NormalizeVar, ValidateVar);

        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowNumberInFormulaWithNum()
        {
            Formula formula = new Formula(" 4 + 3            3 - 8", NormalizeVar, ValidateVar);

        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowVariableInFormulaWithLeftParam()
        {
            Formula formula = new Formula(" 3 + A9 ( + 6    ", NormalizeVar, ValidateVar);

        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowVariableInFormulaWithVar()
        {
            Formula formula = new Formula(" 2 + A9 A8 - 12", NormalizeVar, ValidateVar);

        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowVariableInFormulaWithNum()
        {
            Formula formula = new Formula("1 - A8 3 + 7 ", NormalizeVar, ValidateVar);

        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowRightParamInFormulaWithLeftParam()
        {
            Formula formula = new Formula(" 3 + 3)( 6-1   ", NormalizeVar, ValidateVar);

        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowRightParamInFormulaWithVar()
        {
            Formula formula = new Formula(" 3+3 )    A8 * 5", NormalizeVar, ValidateVar);

        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFollowRightParamInFormulaWithNum()
        {
            Formula formula = new Formula("3 + 3 )        3 * 2", NormalizeVar, ValidateVar);

        }


        [TestMethod(), Timeout(5000)]
        public void TestSingleNumber()
        {
            Formula formula = new Formula("5", NormalizeVar, ValidateVar);
            Assert.AreEqual(5, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestSingleVariable()
        { 
            Formula formula = new Formula("X5", NormalizeVar, ValidateVar);
            Assert.AreEqual(13, (Double)formula.Evaluate(s => 13), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestAddition()
        {
            Formula formula = new Formula("5.8+3.1", NormalizeVar, ValidateVar);
            Assert.AreEqual(9.1, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestSubtraction()
        {
            Formula formula = new Formula("18-10.5", NormalizeVar, ValidateVar);
            Assert.AreEqual(7.5, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestMultiplication()
        {
            Formula formula = new Formula("2*4.0", NormalizeVar, ValidateVar);
            Assert.AreEqual(8, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestDivision()
        {
            Formula formula = new Formula("16/2", NormalizeVar, ValidateVar);
            Assert.AreEqual(8, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestArithmeticWithVariable()
        {
            Formula formula = new Formula("2+X1", NormalizeVar, ValidateVar);
            Assert.AreEqual(6, (Double)formula.Evaluate( s => 4), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestLeftToRight()  
        {
            Formula formula = new Formula("2*6+3", NormalizeVar, ValidateVar);
            Assert.AreEqual(15, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestOrderOperations()
        {
            Formula formula = new Formula("2+6*3", NormalizeVar, ValidateVar);
            Assert.AreEqual(20, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestParenthesesTimes()
        {
            Formula formula = new Formula("(2+6)*3", NormalizeVar, ValidateVar);
            Assert.AreEqual(24, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestTimesParentheses()
        {
            Formula formula = new Formula("2*(3+5)", NormalizeVar, ValidateVar);
            Assert.AreEqual(16, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)]
        public void TestPlusParentheses()
        {
            Formula formula = new Formula("2+(3+5)", NormalizeVar, ValidateVar);
            Assert.AreEqual(10, (Double)formula.Evaluate(s => 0), 1e8);

        }


        [TestMethod(), Timeout(5000)]
        public void TestPlusComplex()
        {
            Formula formula = new Formula("2+(3+5*9)", NormalizeVar, ValidateVar);
            Assert.AreEqual(50, (Double)formula.Evaluate(s => 0), 1e8);
        }


        [TestMethod(), Timeout(5000)] 
        public void TestOperatorAfterParens()
        {
            Formula formula = new Formula("(1*1)-2/2", NormalizeVar, ValidateVar);
            Assert.AreEqual(0, (Double)formula.Evaluate( s => 0), 1e8);

        }


        [TestMethod(), Timeout(5000)]
        public void TestComplexTimesParentheses()
        {
            Formula formula = new Formula("2+3*(3+5)", NormalizeVar, ValidateVar);
            Assert.AreEqual(26, (Double)formula.Evaluate(s => 0), 1e8);

        }


        [TestMethod(), Timeout(5000)]
        public void TestComplexAndParentheses()
        {
            Formula formula = new Formula("2+3*5+(3+4*8)*5+2", NormalizeVar, ValidateVar);
            Assert.AreEqual(194, (Double)formula.Evaluate(s => 0), 1e8);

        }


        [TestMethod(), Timeout(5000)]
        public void TestDivideByZero()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.AreEqual(new FormulaError("DIV 0 Error"), formula.Evaluate(s => 0));
        }


        [TestMethod(), Timeout(5000)]
        public void TestToString()
        {
            Formula formula = new Formula(" 6 + (A7 -                                                              11                    )", NormalizeVar, ValidateVar);
            Assert.AreEqual("6+(a7-11)", formula.ToString()); 
        }


        [TestMethod(), Timeout(5000)]
        public void TestGetHashcode()
        {
            Formula formula = new Formula(" 6.00000 + (A7 -                                                              11.0000                    )", NormalizeVar, ValidateVar);
            Formula formula1 = new Formula(" 6 + (A7 -                                                              11                    )", NormalizeVar, ValidateVar);

            Assert.IsTrue(formula.GetHashCode().Equals(formula1.GetHashCode())); 
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqual()
        {
            Formula formula = new Formula(" 6.00000 + (A7 -                                                              11.0000                    )", NormalizeVar, ValidateVar);
            Formula formula1 = new Formula(" 6 + (A7 -                                                              11                    )", NormalizeVar, ValidateVar);

            Assert.IsTrue(formula.Equals(formula1));
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualWithNullParam()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsFalse(formula.Equals(null));
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualWithDifferentType()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsFalse(formula.Equals("This is going to fail"));
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualOperator()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsTrue(null == null);
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualOperator1()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsFalse(formula == null);
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualOperator2()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsFalse(null == formula);
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualOperator3()
        {
            Formula formula = new Formula(" 6.00000 + (A7 -                                                              11.0000                    )", NormalizeVar, ValidateVar);
            Formula formula1 = new Formula(" 6 + (A7 -                                                              11                    )", NormalizeVar, ValidateVar);
            Assert.IsTrue(formula1 == formula);
        }

        [TestMethod(), Timeout(5000)]
        public void TestNotEqualOperator()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsFalse(null != null);
        }

        [TestMethod(), Timeout(5000)]
        public void TestNotEqualOperator1()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsTrue(null != formula);
        }

        [TestMethod(), Timeout(5000)]
        public void TestNotEqualOperator2()
        {
            Formula formula = new Formula("5/0", NormalizeVar, ValidateVar);
            Assert.IsTrue(formula != null);
        }

        [TestMethod(), Timeout(5000)]
        public void TestNotEqualOperator3()
        {
            Formula formula = new Formula(" 26.00000 + (A7 -                                                              11.0000                    )", NormalizeVar, ValidateVar);
            Formula formula1 = new Formula(" 6 + (A7 -                                                              11                    )", NormalizeVar, ValidateVar);
            Assert.IsTrue(formula1 != formula);
        }

        [TestMethod(), Timeout(5000)]
        public void TestGetVariables()
        {
            Formula formula = new Formula(" A8 + (A7 - A9 +  A7  )", NormalizeVar, ValidateVar);

            Assert.AreEqual(3, new List<string>(formula.GetVariables()).Count);
        }



        private double ConvertVar(string s)
        {
            return 5; 
        }

        private string NormalizeVar(string s)
        {
            return s.ToLower(); 
        }

        private bool ValidateVar(string s)
        {
            if (Char.IsLetter(s[0]))
            {
                for (int i = 1; i < s.Length; i++)
                {
                    if (!Char.IsDigit(s[i]))
                    {
                        return false; 
                    }
                }
                return true; 
            }
            else
            {
                return false; 
            }
        }
    }
}
