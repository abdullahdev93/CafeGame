using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{
    public static class LogicalLineUtils
    {
        public static class Encapsulation
        {
            public struct EncapsulatedData
            {
                public bool isNull => lines == null;
                public List<string> lines;
                public int startingIndex;
                public int endingIndex;
            }

            private const char ENCAPSULATION_START = '{';
            private const char ENCAPSULATION_END = '}';

            public static EncapsulatedData RipEncapsulationData(Conversation conversation, int startingIndex, bool ripHeaderAndEncapsualators = false, int parentStartingIndex = 0)
            {
                int encapsulationDepth = 0;
                EncapsulatedData data = new EncapsulatedData { lines = new List<string>(), startingIndex = (startingIndex + parentStartingIndex), endingIndex = 0 };

                for (int i = startingIndex; i < conversation.Count; i++)
                {
                    string line = conversation.GetLines()[i];
                    
                    if (ripHeaderAndEncapsualators || (encapsulationDepth > 0 && !IsEncapsulationEnd(line)))
                        data.lines.Add(line);

                    if (IsEncapsulationStart(line))
                    {
                        encapsulationDepth++;
                        continue;
                    }

                    if (IsEncapsulationEnd(line))
                    {
                        encapsulationDepth--;
                        if (encapsulationDepth == 0)
                        {
                            data.endingIndex = (i + parentStartingIndex);
                            break;
                        }
                    }
                }

                return data;
            }

            public static bool IsEncapsulationStart(string line) => line.Trim().StartsWith(ENCAPSULATION_START);
            public static bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(ENCAPSULATION_END);
        }

        public static class Expressions
        {
            public static HashSet<string> OPERATORS = new HashSet<string>() { "-", "-=", "+", "+=", "*", "*=", "/", "/=", "=" };
            public static readonly string REGEX_ARITHMATIC = @"([-+*/=]=?)";
            public static readonly string REGEX_OPERATOR_LINE = @"^\$\w+\s*(=|\+=|-=|\*=|/=|)\s*";

            public static object CalculateValue(string[] expressionParts)
            {
                List<string> operandStrings = new List<string>();
                List<string> operatorStrings = new List<string>();
                List<object> operands = new List<object>();

                for (int i = 0; i < expressionParts.Length; i++)
                {
                    string part = expressionParts[i].Trim();

                    if (part == string.Empty)
                        continue;

                    if (OPERATORS.Contains(part))
                        operatorStrings.Add(part);
                    else
                        operandStrings.Add(part);
                }

                foreach (string operandString in operandStrings)
                {
                    operands.Add(ExtractValue(operandString));
                }

                CalculateValue_DivisionAndMultiplication(operatorStrings, operands);

                CalculateValue_AdditionAndSubtraction(operatorStrings, operands);

                return operands[0];
            }

            private static void CalculateValue_DivisionAndMultiplication(List<string> operatorStrings, List<object> operands)
            {
                for (int i = 0; i < operatorStrings.Count; i++)
                {
                    string operatorString = operatorStrings[i];

                    if (operatorString == "*" || operatorString == "/")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);

                        if (operatorString == "*")
                            operands[i] = leftOperand * rightOperand;
                        else
                        {
                            if (rightOperand == 0)
                            {
                                if (rightOperand == 0) // Check for division by zero
                                {
                                    Debug.LogError("Cannot divide by zero!");
                                    return;
                                }
                                operands[i] = leftOperand / rightOperand;
                            }
                        }

                        operands.RemoveAt(i + 1);
                        operatorStrings.RemoveAt(i);
                        i--;
                    }
                }
            }

            private static void CalculateValue_AdditionAndSubtraction(List<string> operatorStrings, List<object> operands)
            {
                for (int i = 0; i < operatorStrings.Count; i++)
                {
                    string operatorString = operatorStrings[i];

                    if (operatorString == "+" || operatorString == "-")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);

                        if (operatorString == "+")
                            operands[i] = leftOperand  + rightOperand;
                        else
                            operands[i] = leftOperand - rightOperand;

                        operands.RemoveAt(i + 1);
                        operatorStrings.RemoveAt(i);
                        i--;
                    }
                }
            }

            public static object ExtractValue(string value)
            {
                bool negate = false;

                if (value.StartsWith('!'))
                {
                    negate = true;
                    value = value.Substring(1);
                }

                if (value.StartsWith(VariableStore.VARIABLE_ID))
                {
                    string variableName = value.TrimStart(VariableStore.VARIABLE_ID);
                    if (!VariableStore.HasVariable(variableName))
                    {
                        Debug.LogError($"Variable {variableName} does not exist!");
                        return null;
                    }

                    VariableStore.TryGetValue(variableName, out object val);

                    if (val is bool boolVal && negate)
                        return !boolVal;

                    return val;
                }
                else if (value.StartsWith('\"') && value.EndsWith('\"'))
                {
                    value = TagManager.Inject(value, injectTags: true, injectVariables: true);
                    return value.Trim('"');
                } 
                else
                {
                    if (int.TryParse(value, out int intValue))
                    {
                        return intValue;
                    }
                    else if (float.TryParse(value, out float floatValue))
                    {
                        return floatValue;
                    }
                    else if (bool.TryParse(value, out bool boolValue))
                    {
                        return negate ? !boolValue : boolValue;
                    }
                    else
                    {
                        value = TagManager.Inject(value, injectTags: true, injectVariables: true);
                        return value;
                    }
                }
            }
        }

        public static class Conditions
        {
            public static readonly string REGEX_CONDITIONAL_OPERATORS = @"(==|!=|<=|>=|<|>|&&|\|\|)";

            public static void ShowConditionFailureMessage(string condition)
            {
                // [condition: $PlayerStats.Empathy >= 9]  

                Debug.Log($"[ShowConditionFailureMessage] Raw: {condition}");

                if (string.IsNullOrWhiteSpace(condition))
                {
                    VNMenuManager.instance.StatRequirementUI("You don't meet the requirements.");
                    return;
                }

                string lower = condition.ToLower();

                if (lower.Contains("empathy"))
                    VNMenuManager.instance.StatRequirementUI("I want to understand, but my Empathy isn't strong enough."); 
                else if (lower.Contains("charisma"))
                    VNMenuManager.instance.StatRequirementUI("I want to speak up, but my Charisma isn't high enough.");
                else if (lower.Contains("confidence"))
                    VNMenuManager.instance.StatRequirementUI("I want to say something, but I don't feel confident enough.");
                else if (lower.Contains("insight"))
                    VNMenuManager.instance.StatRequirementUI("I want to see what's really going on, but I lack the Insight.");
                else if (lower.Contains("creativity"))
                    VNMenuManager.instance.StatRequirementUI("I want to come up with something, but my Creativity falls short.");
                else if (lower.Contains("courage"))
                    VNMenuManager.instance.StatRequirementUI("I want to act, but I don't have the Courage right now.");
                else
                    VNMenuManager.instance.StatRequirementUI("I don't meet the requirement."); 
            }

            public static bool EvaluateCondition(string condition)
            {
                if (string.IsNullOrWhiteSpace(condition))
                    return false;

                condition = TagManager.Inject(condition, injectTags: true, injectVariables: true);

                // Remove outer parentheses if present
                condition = condition.Trim();
                if (condition.StartsWith("(") && condition.EndsWith(")"))
                {
                    condition = condition.Substring(1, condition.Length - 2);
                }

                // Handle nested parentheses using recursion
                while (condition.Contains("("))
                {
                    int open = -1;
                    int close = -1;
                    int depth = 0;

                    for (int i = 0; i < condition.Length; i++)
                    {
                        if (condition[i] == '(')
                        {
                            if (depth == 0) open = i;
                            depth++;
                        }
                        else if (condition[i] == ')')
                        {
                            depth--;
                            if (depth == 0)
                            {
                                close = i;
                                break;
                            }
                        }
                    }

                    if (open >= 0 && close >= 0)
                    {
                        string inner = condition.Substring(open + 1, close - open - 1);
                        bool result = EvaluateCondition(inner);
                        condition = condition.Substring(0, open) + result.ToString().ToLower() + condition.Substring(close + 1);
                    }
                    else
                    {
                        break;
                    }
                }

                // Handle AND and OR logic
                if (condition.Contains("&&"))
                {
                    var parts = condition.Split(new[] { "&&" }, StringSplitOptions.None);
                    foreach (var part in parts)
                    {
                        if (!EvaluateCondition(part.Trim()))
                            return false;
                    }
                    return true;
                }

                if (condition.Contains("||"))
                {
                    var parts = condition.Split(new[] { "||" }, StringSplitOptions.None);
                    foreach (var part in parts)
                    {
                        if (EvaluateCondition(part.Trim()))
                            return true;
                    }
                    return false;
                }

                // Basic expression evaluation
                string[] partsSplit = Regex.Split(condition, REGEX_CONDITIONAL_OPERATORS)
                    .Select(p => p.Trim()).ToArray();

                if (partsSplit.Length == 1)
                {
                    if (bool.TryParse(partsSplit[0], out bool result))
                        return result;
                    else
                    {
                        Debug.LogError($"Could not parse condition: {condition}");
                        return false;
                    }
                }
                else if (partsSplit.Length == 3)
                {
                    return EvaluateExpression(partsSplit[0], partsSplit[1], partsSplit[2]);
                }

                Debug.LogError($"Unsupported condition format: {condition}");
                return false;
            } 

            /*public static bool EvaluateCondition(string condition)
            {
                condition = TagManager.Inject(condition, injectTags: true, injectVariables: true);

                // Handle parentheses by evaluating the inner expression first
                if (condition.Contains("("))
                {
                    int open = condition.IndexOf('(');
                    int close = condition.LastIndexOf(')');
                    if (open < close)
                    {
                        string inner = condition.Substring(open + 1, close - open - 1);
                        bool innerResult = EvaluateCondition(inner);
                        condition = condition.Substring(0, open) + innerResult.ToString().ToLower() + condition.Substring(close + 1);
                    }
                }

                // Handle compound conditions (&& and ||)
                if (condition.Contains("&&"))
                {
                    string[] subConditions = condition.Split(new[] { "&&" }, StringSplitOptions.None);
                    foreach (var sub in subConditions)
                    {
                        if (!EvaluateCondition(sub.Trim()))
                            return false;
                    }
                    return true;
                }

                if (condition.Contains("||"))
                {
                    string[] subConditions = condition.Split(new[] { "||" }, StringSplitOptions.None);
                    foreach (var sub in subConditions)
                    {
                        if (EvaluateCondition(sub.Trim()))
                            return true;
                    }
                    return false;
                }

                // Simple expression: "X == Y"
                string[] parts = Regex.Split(condition, REGEX_CONDITIONAL_OPERATORS)
                    .Select(p => p.Trim()).ToArray();

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("\"") && parts[i].EndsWith("\""))
                        parts[i] = parts[i].Substring(1, parts[i].Length - 2);
                }

                if (parts.Length == 1)
                {
                    if (bool.TryParse(parts[0], out bool result))
                        return result;
                    else
                    {
                        Debug.LogError($"Could not parse condition: {condition}");
                        return false;
                    }
                }
                else if (parts.Length == 3)
                {
                    return EvaluateExpression(parts[0], parts[1], parts[2]);
                }

                Debug.LogError($"Unsupported condition format: {condition}");
                return false;
            }*/

            /*public static bool EvaluateCondition(string condition)
            {
                condition = TagManager.Inject(condition, injectTags: true, injectVariables: true);

                string[] parts = Regex.Split(condition, REGEX_CONDITIONAL_OPERATORS)
                    .Select(p => p.Trim()).ToArray();

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("\"") && parts[i].EndsWith("\""))
                        parts[i] = parts[i].Substring(1, parts[i].Length - 2);
                }

                if (parts.Length == 1)
                {
                    if (bool.TryParse(parts[0], out bool result))
                        return result;
                    else
                    {
                        Debug.LogError($"Could not parse condition: {condition}");
                        return false;
                    }
                }
                else if (parts.Length == 3)
                {
                    return EvaluateExpression(parts[0], parts[1], parts[2]);
                }
                else
                {
                    Debug.LogError($"Unsupported condition format: {condition}");
                    return false;
                }
            }*/

            private delegate bool OperatorFunc<T>(T left, T right);

            private static Dictionary<string, OperatorFunc<bool>> boolOperators = new Dictionary<string, OperatorFunc<bool>>()
            {
                { "&&", (left, right) => left && right },
                { "||", (left, right) => left || right },
                { "==", (left, right) => left == right },
                { "!=", (left, right) => left != right }
            };

            private static Dictionary<string, OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>()
            {
                { "==", (left, right) => left == right },
                { "!=", (left, right) => left != right },
                { ">", (left, right) => left > right },
                { ">=", (left, right) => left >= right },
                { "<", (left, right) => left < right },
                { "<=", (left, right) => left <= right }
            };

            private static Dictionary<string, OperatorFunc<int>> intOperators = new Dictionary<string, OperatorFunc<int>>()
            {
                { "==", (left, right) => left == right },
                { "!=", (left, right) => left != right },
                { ">", (left, right) => left > right },
                { ">=", (left, right) => left >= right },
                { "<", (left, right) => left < right },
                { "<=", (left, right) => left <= right } 
            };

            /*private static bool EvaluateExpression(string left, string op, string right)
            {
                // Debugging
                Debug.Log($"[EvaluateExpression] Left: '{left}', Op: '{op}', Right: '{right}'");

                // Try parsing as bool
                if (bool.TryParse(left, out bool leftBool) && bool.TryParse(right, out bool rightBool))
                {
                    if (boolOperators.TryGetValue(op, out var boolOp))
                        return boolOp(leftBool, rightBool);
                }

                // Try parsing as int
                if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
                {
                    if (intOperators.TryGetValue(op, out var intOp))
                        return intOp(leftInt, rightInt);
                }

                // Try parsing as float
                if (float.TryParse(left, out float leftFloat) && float.TryParse(right, out float rightFloat))
                {
                    if (floatOperators.TryGetValue(op, out var floatOp))
                        return floatOp(leftFloat, rightFloat);
                }

                // Fallback to string comparison for == and !=
                if (op == "==") return left == right;
                if (op == "!=") return left != right; 

                throw new InvalidOperationException($"Unsupported Operation: {op} for operands '{left}' and '{right}'");
            }*/


            private static bool EvaluateExpression(string left, string op, string right)
            {
                Debug.Log($"[Eval] Left: {left} ({left.GetType()}), Op: {op}, Right: {right} ({right.GetType()})"); 

                if (bool.TryParse(left, out bool leftBool) && bool.TryParse(right, out bool rightBool))
                    return boolOperators[op](leftBool, rightBool);

                if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightint))
                    return intOperators[op](leftInt, rightint);

                if (float.TryParse(left, out float leftFloat) && float.TryParse(right, out float rightFloat))
                    return floatOperators[op](leftFloat, rightFloat);

                switch (op)
                {
                    case "==": return left == right; 
                    case "!=": return left != right;
                    default: throw new InvalidOperationException($"Unsupported Operation: {op}");
                } 
            }
        }
    }
}