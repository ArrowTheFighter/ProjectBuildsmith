using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSConnectorNode : DSNode
    {

        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.Connector;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = ""
            };

            Choices.Add(choiceData);

        }

        public override void Draw()
        {
            /* TITLE CONTAINER */


            titleContainer.style.display = DisplayStyle.None;
            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);

            /* EXTENSION CONTAINER */


            /* OUTPUT CONTAINER */


            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);
                
                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
