using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSStartDialogNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.StartDialog;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Output",
                LocalizeKey = ""
            };

            Choices.Add(choiceData);
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

                //graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, dialogueNameTextField);
            titleContainer.style.display = DisplayStyle.None;

            /* MAIN CONTAINER */

            Button addOutputButton = DSElementUtility.CreateButton("Add Output", () =>
            {
                DSChoiceSaveData outputData = new DSChoiceSaveData()
                {
                    Text = "Output",
                };

                Choices.Add(outputData);

                Port choicePort = CreateChoicePort(outputData);

                outputContainer.Add(choicePort);

                
            });

            addOutputButton.AddToClassList("ds-node__button");

            Label label = new Label("Dialog Start Node");

            label.style.paddingTop = 8;
            label.style.paddingBottom = 8;
            label.style.paddingLeft = 8;
            label.style.fontSize = 18;

            mainContainer.Insert(1, addOutputButton);
            mainContainer.Insert(1, label);

            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData) userData;

            Button deleteChoiceButton = DSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);

                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = DSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );


            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            //choicePort.Add(first_Section);


            return choicePort;
        }
    }
}