using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DS.Utilities
{
    using Elements;
    using UnityEngine;

    public static class DSElementUtility
    {
        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text
            };

            return button;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed
            };

            return foldout;
        }

        public static Port CreatePort(this DSNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }

        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null,string placeHolderString = null)
        {
            TextField textField = CreateTextField(value, label, onValueChanged);

            Label placeholder = new Label(placeHolderString);
            placeholder.style.position = Position.Absolute;
            placeholder.style.color = new StyleColor(Color.gray);
            placeholder.style.unityTextAlign = TextAnchor.MiddleLeft;
            placeholder.style.marginLeft = 4;
            placeholder.style.marginTop = 2;

            // Position the placeholder over the input area
            textField.Add(placeholder);

            // Hide/show placeholder based on input
            textField.RegisterValueChangedCallback(evt =>
            {
                placeholder.style.display = string.IsNullOrEmpty(evt.newValue)
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });

            // Also hide it if the user clicks in and types
            if (!string.IsNullOrEmpty(textField.value))
                placeholder.style.display = DisplayStyle.None;


            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);

            textArea.multiline = true;

            return textArea;
        }
    }
}