using System.Collections;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{
    public class LL_Input : ILogicalLine
    {
        public string keyword => "input";

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string title = line.dialogueData.rawData;
            InputPanel panel = InputPanel.instance;
            panel.Show(title);

            while (panel.isWaitingOnUserInput)
                yield return null;

            // Access firstName and lastName separately if needed
            string firstName = panel.firstName;
            string lastName = panel.lastName;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}
