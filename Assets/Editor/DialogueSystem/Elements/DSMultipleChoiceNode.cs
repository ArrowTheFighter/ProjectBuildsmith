using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSMultipleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.MultipleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Choice Text",
                LocalizeKey = "LOCAL_CHOICE_KEY"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                if (Choices.Count > 3) return;
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = "Choice Text",
                    LocalizeKey = "LOCAL_CHOICE_KEY"
                };

                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);

                Port settingsPort = this.CreatePort();
                Foldout foldout = new Foldout()
                {
                    text = "Settings"
                };
                foldout.style.paddingTop = 5;
                foldout.style.paddingBottom = 5;
                Button add_itemRequirement = new Button()
                {
                    text = "Add Item Requirement"
                };
                foldout.Add(add_itemRequirement);
                settingsPort.Add(foldout);
                //outputContainer.Add(settingsPort);

            });

            addChoiceButton.AddToClassList("ds-node__button");

            Label label = new Label("Dialog with choices");

            label.style.paddingTop = 8;
            label.style.paddingBottom = 8;
            label.style.paddingLeft = 8;
            label.style.fontSize = 18;

            mainContainer.Insert(1, addChoiceButton);
            mainContainer.Insert(1, label);

            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);
                VisualElement settingsPort = new VisualElement();
                Foldout foldout = new Foldout()
                {
                    text = "Settings"
                };
                foldout.style.paddingTop = 5;
                foldout.style.paddingBottom = 5;
                Button add_itemRequirement = new Button()
                {
                    text = "Add Item Requirement"
                };
                foldout.Add(add_itemRequirement);
                settingsPort.Add(foldout);
                //choicePort.Add(settingsPort);
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


            TextField choiceLocalizeKeyField = DSElementUtility.CreateTextField(choiceData.LocalizeKey, null, callback =>
            {
                choiceData.LocalizeKey = callback.newValue;
            });

            choiceLocalizeKeyField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );

            Color textColor;
            ColorUtility.TryParseHtmlString("#ffc629", out textColor);
            choiceLocalizeKeyField.Q("unity-text-input").style.color = new StyleColor(textColor);
            choiceLocalizeKeyField.Q("unity-text-input").style.unityFontStyleAndWeight = FontStyle.Italic;

            // Foldout settings_foldout = new Foldout()
            // {
            //     text = "Settings"
            // };
            // Toggle toggle = new Toggle()
            // {
            //     text = "Close Dialog"
            // }; Toggle toggle2 = new Toggle()
            // {
            //     text = "Close Dialog"
            // }; Toggle toggle3 = new Toggle()
            // {
            //     text = "Close Dialog"
            // };
            // settings_foldout.Add(toggle);
            // settings_foldout.Add(toggle2);
            // settings_foldout.Add(toggle3);

            // choicePort.Add(settings_foldout);
            choicePort.Add(choiceLocalizeKeyField);
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            //choicePort.Add(first_Section);


            return choicePort;
        }
    }
}