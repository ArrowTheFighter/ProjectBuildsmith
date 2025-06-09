using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor.Localization;
using UnityEngine.Localization;

namespace DS.Utilities
{
    using Data;
    using Data.Save;
    using Elements;
    using ScriptableObjects;
    using Windows;

    public static class DSIOUtility
    {
        private static DSGraphView graphView;

        private static string graphFileName;
        private static string containerFolderPath;

        private static List<DSNode> nodes;
        private static List<DSGroup> groups;

        private static Dictionary<string, DSDialogueGroupSO> createdDialogueGroups;
        private static Dictionary<string, DSDialogueSO> createdDialogues;
        private static Dictionary<string, ScriptableObject> createdScriptableObjects;
        private static Dictionary<string, DSItemRequirementSO> createdItemDialogues;
        private static Dictionary<string, DSCloseDialogSO> createdCloseDialogues;

        private static Dictionary<string, DSGroup> loadedGroups;
        private static Dictionary<string, DSNode> loadedNodes;

        public static void Initialize(DSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/Resources/DialogueSystem/Dialogues/{graphName}";

            nodes = new List<DSNode>();
            groups = new List<DSGroup>();

            createdScriptableObjects = new Dictionary<string, ScriptableObject>();
            createdDialogueGroups = new Dictionary<string, DSDialogueGroupSO>();
            createdDialogues = new Dictionary<string, DSDialogueSO>();
            createdItemDialogues = new Dictionary<string, DSItemRequirementSO>();
            createdCloseDialogues = new Dictionary<string, DSCloseDialogSO>();

            loadedGroups = new Dictionary<string, DSGroup>();
            loadedNodes = new Dictionary<string, DSNode>();
        }

        public static void Save()
        {
            CreateDefaultFolders();

            //Gets all the nodes from the open window
            GetElementsFromGraphView();

            DSGraphSaveDataSO graphData = CreateAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");

            graphData.Initialize(graphFileName);

            DSDialogueContainerSO dialogueContainer = CreateAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);

            dialogueContainer.Initialize(graphFileName);

            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }

        private static void SaveGroups(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (DSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveDataSO graphData)
        {
            DSGroupSaveData groupData = new DSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(DSGroup group, DSDialogueContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

            DSDialogueGroupSO dialogueGroup = CreateAsset<DSDialogueGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);

            createdDialogueGroups.Add(group.ID, dialogueGroup);

            dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<DSDialogueSO>());

            SaveAsset(dialogueGroup);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        private static void SaveNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();

            List<string> key_set_list = new List<string>();
            foreach (DSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);
                SaveLocalizationStringsFromNode(node,key_set_list);

                if (node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);

                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueName);
            }

            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(DSNode node, DSGraphSaveDataSO graphData)
        {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            switch (node)
            {
                case DSConnectorNode connectorNode:
                    DSConnectorSaveData connectorSaveData = new DSConnectorSaveData()
                    {
                        ID = node.ID,
                        Name = node.DialogueName,
                        Choices = choices,
                        Text = node.Text,
                        GroupID = node.Group?.ID,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position,
                        CloseDialog = node.CloseDialog,
                    };
                    graphData.Nodes.Add(connectorSaveData);
                    break;
                case DSRunEventNode runEventNode:
                    DSRunEventSaveData runEventSaveData = new DSRunEventSaveData()
                    {
                        ID = node.ID,
                        Name = node.DialogueName,
                        Choices = choices,
                        Text = node.Text,
                        GroupID = node.Group?.ID,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position,
                        CloseDialog = node.CloseDialog,
                        event_id = runEventNode.event_id,
                    };
                    graphData.Nodes.Add(runEventSaveData);
                    break;
                case DSSetFlagNode setFlagNode:
                    DSSetFlagSaveData setFlagSaveData = new DSSetFlagSaveData()
                    {
                        ID = node.ID,
                        Name = node.DialogueName,
                        Choices = choices,
                        Text = node.Text,
                        GroupID = node.Group?.ID,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position,
                        CloseDialog = node.CloseDialog,
                        flag_id = setFlagNode.flag_id,
                        is_true = setFlagNode.is_true
                    };
                    graphData.Nodes.Add(setFlagSaveData);
                    break;
                case DSRequireFlagNode flagNode:
                    DSRequireFlagSaveData flagSaveData = new DSRequireFlagSaveData()
                    {
                        ID = node.ID,
                        Name = node.DialogueName,
                        Choices = choices,
                        Text = node.Text,
                        GroupID = node.Group?.ID,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position,
                        CloseDialog = node.CloseDialog,
                        flag_id = flagNode.flag_id,
                        is_true = flagNode.is_true
                    };
                    graphData.Nodes.Add(flagSaveData);
                    break;
                case DSItemRequirementNode itemNode:
                    DSItemRequirementSaveData itemNodeData = new DSItemRequirementSaveData()
                    {
                        ID = node.ID,
                        Name = node.DialogueName,
                        Choices = choices,
                        Text = node.Text,
                        GroupID = node.Group?.ID,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position,
                        CloseDialog = node.CloseDialog,
                        item_id = itemNode.item_id,
                        item_amount = itemNode.item_amount,
                        remove_items = itemNode.remove_items_bool


                    };
                    graphData.Nodes.Add(itemNodeData);
                    break;
                case DSCloseDialogNode closeNode:
                    DSCloseDialogSaveData closeNodeData = new DSCloseDialogSaveData()
                    {
                        ID = closeNode.ID,
                        Name = closeNode.DialogueName,
                        Choices = choices,
                        Text = closeNode.Text,
                        GroupID = closeNode.Group?.ID,
                        DialogueType = closeNode.DialogueType,
                        Position = closeNode.GetPosition().position,
                        CloseDialog = closeNode.CloseDialog
                    };
                    graphData.Nodes.Add(closeNodeData);
                    break;
                case DSNode _node:
                    DSNodeSaveData nodeData = new DSNodeSaveData()
                    {
                        ID = node.ID,
                        Name = node.DialogueName,
                        Choices = choices,
                        Text = node.Text,
                        LocalizedKey = node.LocalizedKey,
                        GroupID = node.Group?.ID,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position,
                        CloseDialog = node.CloseDialog
                    };
                    graphData.Nodes.Add(nodeData);
                    break;
             }
                
        }

        private static void SaveNodeToScriptableObject(DSNode node, DSDialogueContainerSO dialogueContainer)
        {
            switch (node)
            {
                case DSConnectorNode connectorNode:
                    DSDialogueSO connectorSO;

                    if (node.Group != null)
                    {
                        connectorSO = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", "connector");
                    }
                    else
                    {
                        connectorSO = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Global/Dialogues", "connector");
                    }
                    connectorSO.Initialize(
                        node.DialogueName,
                        node.Text,
                        node.LocalizedKey,
                        ConvertNodeChoicesToDialogueChoices(node.Choices),
                        node.DialogueType,
                        node.IsStartingNode(),
                        node.CloseDialog
                    );
                    createdScriptableObjects.Add(node.ID, connectorSO);

                    SaveAsset(connectorSO);
                    break;
                case DSRunEventNode runEventNode:
                    DSRunEventSO runEventSO;

                    if (node.Group != null)
                    {
                        runEventSO = CreateAsset<DSRunEventSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
                    }
                    else
                    {
                        runEventSO = CreateAsset<DSRunEventSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
                    }
                    runEventSO.Initialize(
                        node.DialogueName,
                        node.Text,
                        ConvertNodeChoicesToDialogueChoices(node.Choices),
                        node.DialogueType,
                        node.IsStartingNode(),
                        runEventNode.event_id
                    );
                    createdScriptableObjects.Add(node.ID, runEventSO);

                    SaveAsset(runEventSO);
                    break;

                case DSSetFlagNode flagNode:
                    DSSetFlagSO setFlagSO;

                    if (node.Group != null)
                    {
                        setFlagSO = CreateAsset<DSSetFlagSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
                    }
                    else
                    {
                        setFlagSO = CreateAsset<DSSetFlagSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
                    }
                    setFlagSO.Initialize(
                        flagNode.DialogueName,
                        flagNode.Text,
                        ConvertNodeChoicesToDialogueChoices(flagNode.Choices),
                        flagNode.DialogueType,
                        flagNode.IsStartingNode(),
                        flagNode.flag_id,
                        flagNode.is_true
                    );

                    createdScriptableObjects.Add(node.ID, setFlagSO);

                    SaveAsset(setFlagSO);
                    break;

                case DSRequireFlagNode flagNode:
                    DSRequireFlagSO flagSO;

                    if (node.Group != null)
                    {
                        flagSO = CreateAsset<DSRequireFlagSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
                    }
                    else
                    {
                        flagSO = CreateAsset<DSRequireFlagSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
                    }
                    flagSO.Initialize(
                        flagNode.DialogueName,
                        flagNode.Text,
                        ConvertNodeChoicesToDialogueChoices(flagNode.Choices),
                        flagNode.DialogueType,
                        flagNode.IsStartingNode(),
                        flagNode.flag_id,
                        flagNode.is_true
                    );

                    //createdItemDialogues.Add(node.ID, flagNode);

                    createdScriptableObjects.Add(node.ID, flagSO);

                    SaveAsset(flagSO);
                    break;

                case DSItemRequirementNode itemNode:
                    DSItemRequirementSO item_so;
                    if (node.Group != null)
                    {
                        item_so = CreateAsset<DSItemRequirementSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);

                        //dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
                    }
                    else
                    {
                        item_so = CreateAsset<DSItemRequirementSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);

                        //dialogueContainer.UngroupedDialogues.Add(dialogue);
                    }
                    item_so.Initialize(
                        itemNode.DialogueName,
                        itemNode.Text,
                        ConvertNodeChoicesToDialogueChoices(itemNode.Choices),
                        itemNode.DialogueType,
                        itemNode.IsStartingNode(),
                        itemNode.item_id,
                        itemNode.item_amount,
                        itemNode.remove_items_bool
                    );

                    createdItemDialogues.Add(node.ID, item_so);

                    createdScriptableObjects.Add(node.ID, item_so);

                    SaveAsset(item_so);
                    break;
                case DSCloseDialogNode closeNode:
                    DSCloseDialogSO close_so;
                    if (node.Group != null)
                    {
                        close_so = CreateAsset<DSCloseDialogSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);

                        //dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
                    }
                    else
                    {
                        close_so = CreateAsset<DSCloseDialogSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);

                        //dialogueContainer.UngroupedDialogues.Add(dialogue);
                    }
                    close_so.Initialize(
                        closeNode.DialogueName,
                        closeNode.Text,
                        ConvertNodeChoicesToDialogueChoices(closeNode.Choices),
                        closeNode.DialogueType,
                        closeNode.IsStartingNode()
                    );

                    createdCloseDialogues.Add(node.ID, close_so);


                    createdScriptableObjects.Add(node.ID, close_so);

                    SaveAsset(close_so);
                    break;
                case DSNode _node:
                    DSDialogueSO dialogue;

                    if (node.Group != null)
                    {
                        if (node is DSItemRequirementNode)
                        {

                        }
                        dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);

                        dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
                    }
                    else
                    {
                        dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);

                        dialogueContainer.UngroupedDialogues.Add(dialogue);
                    }

                    dialogue.Initialize(
                        node.DialogueName,
                        node.Text,
                        node.LocalizedKey,
                        ConvertNodeChoicesToDialogueChoices(node.Choices),
                        node.DialogueType,
                        node.IsStartingNode(),
                        node.CloseDialog
                    );

                    createdScriptableObjects.Add(node.ID, dialogue);

                    SaveAsset(dialogue);
                    break;
             }
            
            
        }

        private static List<string> SaveLocalizationStringsFromNode(DSNode node, List<string> key_set_list)
        {
            if (node is not DSSingleChoiceNode && node is not DSMultipleChoiceNode)
            {
                return key_set_list;
             }
            if (node.LocalizedKey == null || node.LocalizedKey == "")
            {
                return key_set_list;
             }
            //LOCAL TABLE NAME GOES HERE
            //Debug.Log("LOCAL TABLE NAME NEEDS CHANGING!!!");
            var tableCollection = LocalizationEditorSettings.GetStringTableCollection("NPC");

            if (tableCollection == null)
            {
                Debug.LogError("Table collection not found!");
                return key_set_list;
            }

            string key = node.LocalizedKey;
            string value = node.Text;

            key_set_list = SaveLocalizationString(tableCollection, key, value, key_set_list);

            if (node is DSMultipleChoiceNode)
            {
                foreach (DSChoiceSaveData choiceData in node.Choices)
                {
                    string choice_key = choiceData.LocalizeKey;
                    string choice_value = choiceData.Text;
                    key_set_list = SaveLocalizationString(tableCollection, choice_key, choice_value, key_set_list);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string tableCollectionPath = AssetDatabase.GetAssetPath(tableCollection);
            AssetDatabase.ImportAsset(tableCollectionPath, ImportAssetOptions.ForceUpdate);
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            return key_set_list;
        }

        private static List<string> SaveLocalizationString(StringTableCollection tableCollection, string key, string value, List<string> previouslySetKeys)
        {
            if (previouslySetKeys.Contains(key))
            {
                Debug.LogWarning($"More then one node contains the same key! Key = <color=orange>{key}</color> (A node probably needs it's key renamed)");
            }
            if (key == "LOCAL_DIALOG_KEY" || key == "LOCAL_CHOICE_KEY")
            {
                Debug.LogWarning("Saved a key with a default value (A key probably needs to be set on a node)");
            }
            foreach (var table in tableCollection.StringTables)
            {
                string localeCode = table.LocaleIdentifier.Code;
                if (localeCode == "en-US") // Only modify English
                {
                    var entry = table.GetEntry(key);
                    if (entry == null)
                    {
                        entry = table.AddEntry(key, value);
                    }
                    else
                    {
                        entry.Value = value; // optional update
                    }
                    EditorUtility.SetDirty(table); // mark for save
                    previouslySetKeys.Add(key);
                    return previouslySetKeys;
                }
            }
            return previouslySetKeys;

        }

        private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSDialogueChoiceData> dialogueChoices = new List<DSDialogueChoiceData>();

            foreach (DSChoiceSaveData nodeChoice in nodeChoices)
            {
                DSDialogueChoiceData choiceData = new DSDialogueChoiceData()
                {
                    Text = nodeChoice.Text,
                    LocalizeKey = nodeChoice.LocalizeKey,
                    OutputID = nodeChoice.OutputID,

                };

                dialogueChoices.Add(choiceData);
            }

            return dialogueChoices;
        }

        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (DSNode node in nodes)
            {
                switch (node)
                {
                    case DSRunEventNode runEventNodeNode:
                        DSRunEventSO runEventDialog = (DSRunEventSO)createdScriptableObjects[node.ID];

                        for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                        {
                            DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                            if (string.IsNullOrEmpty(nodeChoice.NodeID))
                            {
                                continue;
                            }
                            if (createdScriptableObjects.ContainsKey(nodeChoice.NodeID))
                            {
                                runEventDialog.Choices[choiceIndex].NextDialogue = createdScriptableObjects[nodeChoice.NodeID];
                            }

                            SaveAsset(runEventDialog);
                        }
                        break;
                    case DSSetFlagNode setFlagNode:
                        DSSetFlagSO setFlagDialog = (DSSetFlagSO)createdScriptableObjects[node.ID];

                        for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                        {
                            DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                            if (string.IsNullOrEmpty(nodeChoice.NodeID))
                            {
                                continue;
                            }

                            if (createdScriptableObjects.ContainsKey(nodeChoice.NodeID))
                            {
                                setFlagDialog.Choices[choiceIndex].NextDialogue = createdScriptableObjects[nodeChoice.NodeID];
                            }

                            SaveAsset(setFlagDialog);
                        }
                        break;
                    case DSRequireFlagNode flagNode:
                        DSRequireFlagSO flagDialog = (DSRequireFlagSO)createdScriptableObjects[node.ID];

                        for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                        {
                            DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                            if (string.IsNullOrEmpty(nodeChoice.NodeID))
                            {
                                continue;
                            }

                            if (createdScriptableObjects.ContainsKey(nodeChoice.NodeID))
                            {
                                flagDialog.Choices[choiceIndex].NextDialogue = createdScriptableObjects[nodeChoice.NodeID];
                            }


                            SaveAsset(flagDialog);
                        }
                        break;

                    case DSItemRequirementNode itemNode:
                        DSItemRequirementSO itemDialogue = (DSItemRequirementSO)createdScriptableObjects[node.ID];

                        for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                        {
                            DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                            if (string.IsNullOrEmpty(nodeChoice.NodeID))
                            {
                                continue;
                            }

                            // if (createdDialogues.ContainsKey(nodeChoice.NodeID))
                            // {
                            //     itemDialogue.Choices[choiceIndex].NextDialogue = createdItemDialogues[nodeChoice.NodeID];
                            // }
                            if (createdScriptableObjects.ContainsKey(nodeChoice.NodeID))
                            {
                                itemDialogue.Choices[choiceIndex].NextDialogue = createdScriptableObjects[nodeChoice.NodeID];
                            }


                            SaveAsset(itemDialogue);
                        }
                        break;
                    case DSCloseDialogNode closeNode:
                        DSCloseDialogSO closeDialog = (DSCloseDialogSO)createdScriptableObjects[node.ID];

                        for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                        {
                            DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                            if (string.IsNullOrEmpty(nodeChoice.NodeID))
                            {
                                continue;
                            }

                            // if (createdDialogues.ContainsKey(nodeChoice.NodeID))
                            // {
                            //     closeDialog.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID];
                            // }
                            if (createdScriptableObjects.ContainsKey(nodeChoice.NodeID))
                            {
                                closeDialog.Choices[choiceIndex].NextDialogue = createdScriptableObjects[nodeChoice.NodeID];
                            }


                            SaveAsset(closeDialog);
                        }
                        break;
                    case DSConnectorNode:
                    case DSNode _node:
                        DSDialogueSO dialogue = (DSDialogueSO)createdScriptableObjects[node.ID];

                        for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                        {
                            DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                            if (string.IsNullOrEmpty(nodeChoice.NodeID))
                            {
                                continue;
                            }

                            // if (createdDialogues.ContainsKey(nodeChoice.NodeID))
                            // {
                            //     dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID];
                            // }
                            if (createdScriptableObjects.ContainsKey(nodeChoice.NodeID))
                            {
                                dialogue.Choices[choiceIndex].NextDialogue = createdScriptableObjects[nodeChoice.NodeID];
                            }

                            SaveAsset(dialogue);
                        }
                        break;
                 }
                
            }
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }

        public static void Load()
        {
            DSGraphSaveDataSO graphData = LoadAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", graphFileName);
            
            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Resources/DialogueSystem/Graphs/{graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }

            DSEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
        }

        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach (DSGroupSaveData groupData in groups)
            {
                DSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<DSNodeSaveData> nodes)
        {
            foreach (DSNodeSaveData nodeData in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);
                DSNode node = graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);

                node.ID = nodeData.ID;
                node.Choices = choices;
                node.Text = nodeData.Text;
                node.LocalizedKey = nodeData.LocalizedKey;
                node.CloseDialog = nodeData.CloseDialog;
                if (nodeData is DSItemRequirementSaveData)
                {
                    if (node is DSItemRequirementNode)
                    {
                        DSItemRequirementNode itemNode = (DSItemRequirementNode)node;

                        DSItemRequirementSaveData itemNodeData = (DSItemRequirementSaveData)nodeData;
                        itemNode.item_id = itemNodeData.item_id;
                        itemNode.item_amount = itemNodeData.item_amount;
                        itemNode.remove_items_bool = itemNodeData.remove_items;
                    }
                }
                if (nodeData is DSRequireFlagSaveData)
                {
                    if (node is DSRequireFlagNode)
                    {
                        DSRequireFlagNode flagNode = (DSRequireFlagNode)node;

                        DSRequireFlagSaveData flagSaveData = (DSRequireFlagSaveData)nodeData;
                        flagNode.flag_id = flagSaveData.flag_id;
                        flagNode.is_true = flagSaveData.is_true;
                     }
                 }
                if (nodeData is DSSetFlagSaveData)
                {
                    if (node is DSSetFlagNode)
                    {
                        DSSetFlagNode flagNode = (DSSetFlagNode)node;

                        DSSetFlagSaveData flagSaveData = (DSSetFlagSaveData)nodeData;
                        flagNode.flag_id = flagSaveData.flag_id;
                        flagNode.is_true = flagSaveData.is_true;
                    }
                }
                if (nodeData is DSRunEventSaveData)
                {
                    if (node is DSRunEventNode)
                    {
                        DSRunEventNode runEventNode = (DSRunEventNode)node;

                        DSRunEventSaveData runEventSaveData = (DSRunEventSaveData)nodeData;

                        runEventNode.event_id = runEventSaveData.event_id;
                    }
                }

                node.Draw();

                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                DSGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) choicePort.userData;

                    if (string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    DSNode nextNode = loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }

        private static void CreateDefaultFolders()
        {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");

            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/Resources/DialogueSystem", "Dialogues");

            CreateFolder("Assets/Resources/DialogueSystem/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
        }

        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(DSGroup);

            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is DSNode node)
                {
                    nodes.Add(node);

                    return;
                }

                if (graphElement.GetType() == groupType)
                {
                    DSGroup group = (DSGroup) graphElement;

                    groups.Add(group);

                    return;
                }
            });
        }

        public static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }

        public static void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();

            foreach (DSChoiceSaveData choice in nodeChoices)
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID,
                    LocalizeKey = choice.LocalizeKey,
                    OutputID = choice.OutputID
                    
                };

                choices.Add(choiceData);
            }

            return choices;
        }
    }
}