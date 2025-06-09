using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSSetFlagNode : DSNode
    {

        [SerializeField] public string flag_id;
        [SerializeField] public bool is_true;
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.SetFlag;

            DSChoiceSaveData outputData = new DSChoiceSaveData()
            {
                Text = "output",
                OutputID = "IsTrue"
            };

            Choices.Add(outputData);

            flag_id = "";
            is_true = true;
        }

        public override void Draw()
        {

            Label label = new Label("Set Flag");

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

            TextField itemIDTextField = DSElementUtility.CreateTextArea(flag_id, "Flag ID", callback => flag_id = callback.newValue);

            itemIDTextField.AddClasses(
                "ds-node__text-field-small"
            );

            itemIDTextField.style.unityTextAlign = TextAnchor.MiddleRight;

            // Check box //

            Toggle remove_items = new Toggle()
            {
                text = "Set Flag to True",
                value = is_true
            };

            remove_items.RegisterValueChangedCallback(callback =>
            {
                is_true = callback.newValue;
            });

            remove_items.style.alignSelf = Align.FlexEnd;
            remove_items.style.paddingTop = 8;

            textFoldout.Add(itemIDTextField);
            textFoldout.Add(remove_items);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

            /* OUTPUT CONTAINER */
            // Port truePort = this.CreatePort("Flag is true");
            // Port falsePort = this.CreatePort("Flag is false");

            // truePort.userData = Choices[0];
            // falsePort.userData = Choices[0];

            // outputContainer.Add(truePort);
            // outputContainer.Add(falsePort);

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
