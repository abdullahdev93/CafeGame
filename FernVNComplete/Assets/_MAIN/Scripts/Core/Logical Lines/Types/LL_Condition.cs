/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Conditions;

namespace DIALOGUE.LogicalLines
{
    public class LL_Condition : ILogicalLine
    {
        public string keyword => "if";
        private const string ELSE = "else";
        private readonly string[] CONTAINERS = new string[] { "(", ")" };

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, false, parentStartingIndex: currentConversation.fileStartIndex);
            EncapsulatedData elseData = new EncapsulatedData();

            if (ifData.endingIndex + 1 < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();
                if (nextLine == ELSE)
                {
                    elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, false, parentStartingIndex: currentConversation.fileStartIndex);
                }
            }

            currentConversation.SetProgress(elseData.isNull ? ifData.endingIndex : elseData.endingIndex);

            EncapsulatedData selData = conditionResult ? ifData : elseData;
            
            if (!selData.isNull && selData.lines.Count > 0)
            {
                //Remove the header and encapsulator lines from the conversation indexes
                selData.startingIndex += 2;//Remove header and starting encapsulator
                selData.endingIndex -= 1;//Remove ending encapsulator

                Conversation newConversation = new Conversation(selData.lines, file: currentConversation.file, fileStartIndex: selData.startingIndex, fileEndIndex: selData.endingIndex);
                DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return line.rawData.Trim().StartsWith(keyword);
        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
            int endIndex = line.IndexOf(CONTAINERS[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}*/ 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Conditions;

namespace DIALOGUE.LogicalLines
{
    public class LL_Condition : ILogicalLine
    {
        public string keyword => "if";
        private const string ELSE = "else";
        private const string ELSE_IF = "else if";
        private readonly string[] CONTAINERS = new string[] { "(", ")" };

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.instance.conversationManager.conversationProgress;

            List<(bool result, EncapsulatedData data)> blocks = new List<(bool, EncapsulatedData)>();

            // Parse primary if block
            bool conditionResult = EvaluateCondition(ExtractCondition(line.rawData.Trim()));
            EncapsulatedData blockData = RipEncapsulationData(currentConversation, currentProgress, false, parentStartingIndex: currentConversation.fileStartIndex);
            blocks.Add((conditionResult, blockData));

            int index = blockData.endingIndex + 1;

            // Check for any else if or else blocks following
            while (index < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[index].Trim();

                if (nextLine.StartsWith(ELSE_IF))
                {
                    string nextCondition = ExtractCondition(nextLine);
                    bool nextResult = EvaluateCondition(nextCondition);
                    EncapsulatedData nextData = RipEncapsulationData(currentConversation, index, false, parentStartingIndex: currentConversation.fileStartIndex);
                    blocks.Add((nextResult, nextData));
                    index = nextData.endingIndex + 1;
                }
                else if (nextLine == ELSE)
                {
                    EncapsulatedData elseData = RipEncapsulationData(currentConversation, index, false, parentStartingIndex: currentConversation.fileStartIndex);
                    blocks.Add((true, elseData)); // else always runs if previous failed
                    index = elseData.endingIndex + 1;
                    break;
                }
                else
                    break;
            }

            currentConversation.SetProgress(index - 1);

            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                if (block.result && !block.data.isNull && block.data.lines.Count > 0)
                {
                    EncapsulatedData data = block.data;
                    data.startingIndex += 2;
                    data.endingIndex -= 1;

                    Conversation newConversation = new Conversation(data.lines, file: currentConversation.file, fileStartIndex: data.startingIndex, fileEndIndex: data.endingIndex);
                    DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
                    break;
                }
            }

            yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            string trimmed = line.rawData.Trim();
            return trimmed.StartsWith(keyword) || trimmed.StartsWith("else if") || trimmed.StartsWith("else");
        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
            int endIndex = line.IndexOf(CONTAINERS[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}
