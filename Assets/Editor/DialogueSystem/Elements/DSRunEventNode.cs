using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSRunEventNode : DSNode
    {

        [SerializeField] public string event_id;
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.RunEvent;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Output"
            };

            Choices.Add(choiceData);

            event_id = "";
        }

        public override void Draw()
        {

            Label label = new Label("Run Event");

            label.style.paddingTop = 8;
            label.style.paddingBottom = 8;
            label.style.paddingLeft = 8;
            label.style.fontSize = 18;

            mainContainer.Insert(1, label);

            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    DialogueName = target.value;

                    graphView.AddUngroupedNode(this);

                    return;
                }

                DSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                DialogueName = target.value;

                graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, dialogueNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);

            /* EXTENSION CONTAINER */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Settings");

            TextField itemIDTextField = DSElementUtility.CreateTextArea(event_id, "Event ID", callback => event_id = callback.newValue);

            itemIDTextField.AddClasses(
                "ds-node__text-field-small"
            );

            itemIDTextField.style.unityTextAlign = TextAnchor.MiddleRight;

            textFoldout.Add(itemIDTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

            /* OUTPUT CONTAINER */


            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort("Next Dialog");

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
