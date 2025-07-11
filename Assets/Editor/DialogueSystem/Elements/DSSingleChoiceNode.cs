using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using UnityEditor.Rendering;
    using Utilities;
    using Windows;

    public class DSSingleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.SingleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next Dialogue",
                LocalizeKey = "LOCAL_CHOICE_KEY"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* Title Extra */

            Label label = new Label("NPC Dialog");

            label.style.paddingTop = 8;
            label.style.paddingBottom = 8;
            label.style.paddingLeft = 8;
            label.style.fontSize = 18;

            mainContainer.Insert(1, label);

            // titleContainer.style.flexDirection = FlexDirection.Column;
            // titleContainer.style.flexGrow = 1;
            // titleContainer.style.height = StyleKeyword.Auto;

            // Label label = new Label("Dialog Node");

            // titleContainer.Insert(1,label);

            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData)userData;

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
            //choicePort.Add(choiceLocalizeKeyField);
            choicePort.Add(choiceTextField);
            //choicePort.Add(first_Section);


            return choicePort;
        }
    }
}
