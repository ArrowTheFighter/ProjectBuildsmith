using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Windows
{
    using Elements;
    using Enumerations;

    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DSGraphView dsGraphView)
        {
            graphView = dsGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Nodes"), 1),
                new SearchTreeEntry(new GUIContent("NPC Dialog", indentationIcon))
                {
                    userData = DSDialogueType.SingleChoice,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("NPC Dialog With Choices", indentationIcon))
                {
                    userData = DSDialogueType.MultipleChoice,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Settings"), 1),
                new SearchTreeEntry(new GUIContent("Item Check", indentationIcon))
                {
                    userData = DSDialogueType.ItemRequirement,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Give Item", indentationIcon))
                {
                    userData = DSDialogueType.GiveItem,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Flag Check", indentationIcon))
                {
                    userData = DSDialogueType.RequireFlag,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Set Flag", indentationIcon))
                {
                    userData = DSDialogueType.SetFlag,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Run Event", indentationIcon))
                {
                    userData = DSDialogueType.RunEvent,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Assign Quest", indentationIcon))
                {
                    userData = DSDialogueType.AssignQuest,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Groups"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    userData = new Group(),
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Close Dialog", indentationIcon))
                {
                    userData = DSDialogueType.CloseDialog,
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Connector", indentationIcon))
                {
                    userData = DSDialogueType.Connector,
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Starter Node", indentationIcon))
                {
                    userData = DSDialogueType.StartDialog,
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Return to Start", indentationIcon))
                {
                    userData = DSDialogueType.ReturnToStart,
                    level = 1
                },
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch (SearchTreeEntry.userData)
            {
                case DSDialogueType.SingleChoice:
                {
                    DSSingleChoiceNode singleChoiceNode = (DSSingleChoiceNode) graphView.CreateNode("DialogueName", DSDialogueType.SingleChoice, localMousePosition);

                    graphView.AddElement(singleChoiceNode);

                    return true;
                }

                case DSDialogueType.MultipleChoice:
                {
                    DSMultipleChoiceNode multipleChoiceNode = (DSMultipleChoiceNode) graphView.CreateNode("DialogueName", DSDialogueType.MultipleChoice, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }

                case DSDialogueType.ItemRequirement:
                {
                    DSItemRequirementNode itemRequirementNode = (DSItemRequirementNode)graphView.CreateNode("ItemCheck", DSDialogueType.ItemRequirement, localMousePosition);

                    graphView.AddElement(itemRequirementNode);

                    return true;
                }

                case DSDialogueType.GiveItem:
                {
                    DSGiveItemNode giveItemNode = (DSGiveItemNode)graphView.CreateNode("GiveItem", DSDialogueType.GiveItem, localMousePosition);

                    graphView.AddElement(giveItemNode);

                    return true;
                }

                case DSDialogueType.CloseDialog:
                {
                    DSCloseDialogNode closeDialogNode = (DSCloseDialogNode)graphView.CreateNode("CloseDialog", DSDialogueType.CloseDialog, localMousePosition);

                    graphView.AddElement(closeDialogNode);

                    return true;
                }

                case DSDialogueType.RequireFlag:
                {
                    DSRequireFlagNode requireFlagNode = (DSRequireFlagNode)graphView.CreateNode("FlagCheck", DSDialogueType.RequireFlag, localMousePosition);

                    graphView.AddElement(requireFlagNode);

                    return true;
                }

                case DSDialogueType.SetFlag:
                {
                        DSSetFlagNode setFlagNode = (DSSetFlagNode)graphView.CreateNode("SetFlag", DSDialogueType.SetFlag, localMousePosition);

                    graphView.AddElement(setFlagNode);

                    return true;
                }

                case DSDialogueType.RunEvent:
                {
                    DSRunEventNode runEventFlagNode = (DSRunEventNode)graphView.CreateNode("RunEvent", DSDialogueType.RunEvent, localMousePosition);

                    graphView.AddElement(runEventFlagNode);

                    return true;
                }

                case DSDialogueType.AssignQuest:
                    {
                        DSAssignQuestNode assignQuestNode = (DSAssignQuestNode)graphView.CreateNode("AssignQuest", DSDialogueType.AssignQuest, localMousePosition);

                        graphView.AddElement(assignQuestNode);

                        return true;
                    }

                case DSDialogueType.Connector:
                {
                    DSConnectorNode ConnectorNode = (DSConnectorNode)graphView.CreateNode("", DSDialogueType.Connector, localMousePosition);

                    graphView.AddElement(ConnectorNode);

                    return true;
                }

                case DSDialogueType.StartDialog:
                {
                    DSStartDialogNode ConnectorNode = (DSStartDialogNode)graphView.CreateNode("Start Dialog", DSDialogueType.StartDialog, localMousePosition);

                    graphView.AddElement(ConnectorNode);

                    return true;
                }
                case DSDialogueType.ReturnToStart:
                {
                    DSReturnToStartNode ReturnToStartNode = (DSReturnToStartNode)graphView.CreateNode("Return To Start", DSDialogueType.ReturnToStart, localMousePosition);

                    graphView.AddElement(ReturnToStartNode);

                    return true;
                }

                case Group _:
                {
                    graphView.CreateGroup("DialogueGroup", localMousePosition);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}