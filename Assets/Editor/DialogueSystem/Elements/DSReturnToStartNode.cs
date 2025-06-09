using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSReturnToStartNode : DSNode
    {

        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.ReturnToStart;

        }

        public override void Draw()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                // if (string.IsNullOrEmpty(target.value))
                // {
                //     if (!string.IsNullOrEmpty(DialogueName))
                //     {
                //         ++graphView.NameErrorsAmount;
                //     }
                // }
                // else
                // {
                //     if (string.IsNullOrEmpty(DialogueName))
                //     {
                //         --graphView.NameErrorsAmount;
                //     }
                // }

                // if (Group == null)
                // {
                //     graphView.RemoveUngroupedNode(this);

                //     DialogueName = target.value;

                //     graphView.AddUngroupedNode(this);

                //     return;
                // }

                // DSGroup currentGroup = Group;

                // graphView.RemoveGroupedNode(this, Group);

                DialogueName = target.value;

                // graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__filename-text-field"
            );


            //titleContainer.Insert(0, dialogueNameTextField);

            Label label = new Label("Return to start");

            label.style.paddingTop = 8;
            label.style.paddingBottom = 8;
            label.style.paddingLeft = 18;
            label.style.paddingRight = 18;
            label.style.fontSize = 18;

            mainContainer.Insert(1, label);
            titleContainer.style.display = DisplayStyle.None;

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

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
