using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TagSelectorAttribute))]
public class TagSelectorPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType == SerializedPropertyType.String) {
            EditorGUI.BeginProperty(position, label, property);

            TagSelectorAttribute attrib = this.attribute as TagSelectorAttribute;

            if (attrib.useDefaultTagFieldDrawer) {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            } else {
                List<string> tagList = new List<string>();
                tagList.Add("<NoTag>");
                tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                string propertyString = property.stringValue;
                int index = -1;
                if (propertyString == "") {
                    index = 0;
                } else {
                    for (int i = 1; i < tagList.Count; i++) {
                        if (tagList[i] == propertyString) {
                            index = i;
                            break;
                        }
                    }
                }

                index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

                if (index == 0) {
                    property.stringValue = "";
                } else if (index >= 1) {
                    property.stringValue = tagList[index];
                } else {
                    property.stringValue = "";
                }
            }

            EditorGUI.EndProperty();
        } else {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}

[CustomPropertyDrawer(typeof(MultipleTagSelector))]
public class MultipleTagSelectorDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        Rect currentPosition = position;
        currentPosition.height = EditorGUIUtility.singleLineHeight;
        currentPosition.y += EditorGUIUtility.standardVerticalSpacing;

        List<string> tagList = new List<string>();
        tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);

        SerializedProperty prop_actualtagList = property.FindPropertyRelative("tags");
        List<string> actualTagList = new List<string>();

        //for (int i = 0; i < prop_actualtagList.arraySize; i++) {
        //    actualTagList.Add(prop_actualtagList.GetArrayElementAtIndex(i).stringValue);
        //}

        SerializedProperty prop_state = property.FindPropertyRelative("state");
        MultipleTagSelector.State state = (MultipleTagSelector.State)prop_state.enumValueIndex;

        //EditorGUI.DropShadowLabel(currentPosition, state.ToString());

        switch (state) {
            case MultipleTagSelector.State.MULTIPLE:
                for (int i = 0; i < prop_actualtagList.arraySize; i++) {
                    actualTagList.Add(prop_actualtagList.GetArrayElementAtIndex(i).stringValue);
                }
                break;
            case MultipleTagSelector.State.EVERYTHING:
                actualTagList.AddRange(tagList);
                break;
        }

        string dropdownName = "...Multiple";
        if (actualTagList.Count == 0) {
            dropdownName = "None";
        } else if (actualTagList.Count == 1) {
            dropdownName = actualTagList[0];
        } else if (actualTagList.Count == tagList.Count) {
            dropdownName = "Everything";
        }

        EditorGUI.LabelField(currentPosition, new GUIContent(property.displayName));
        currentPosition.x += EditorGUIUtility.labelWidth;
        currentPosition.width -= EditorGUIUtility.labelWidth;
        if (EditorGUI.DropdownButton(currentPosition, new GUIContent(dropdownName), FocusType.Passive)) {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < tagList.Count; i++) {
                bool isInList = false;
                if (actualTagList.Contains(tagList[i])) { isInList = true; }
                menu.AddItem(new GUIContent(tagList[i]), isInList, handleItemClicked, tagList[i]);
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("None"), false, CheckNone);
            menu.AddItem(new GUIContent("Eveything"), false, CheckAll);
            menu.DropDown(currentPosition);
        }

        prop_actualtagList.ClearArray();
        for (int i = 0; i < actualTagList.Count; i++) {
            prop_actualtagList.InsertArrayElementAtIndex(i);
            prop_actualtagList.GetArrayElementAtIndex(i).stringValue = actualTagList[i];
        }
        property.serializedObject.ApplyModifiedProperties();

        void handleItemClicked(object parameter) {
            string tag = parameter as string;
            if (tag == null) { return; }
            if (actualTagList.Contains(tag)) {
                prop_actualtagList.DeleteArrayElementAtIndex(actualTagList.IndexOf(tag));
                actualTagList.Remove(tag);
            } else {
                int size = prop_actualtagList.arraySize;
                prop_actualtagList.InsertArrayElementAtIndex(size);
                prop_actualtagList.GetArrayElementAtIndex(size).stringValue = tag;
                actualTagList.Add(tag);
            }
            UpdateState();
            property.serializedObject.ApplyModifiedProperties();
        }

        void CheckNone() {
            prop_actualtagList.ClearArray();
            prop_state.intValue = (int)MultipleTagSelector.State.NONE;
            property.serializedObject.ApplyModifiedProperties();
        }

        void CheckAll() {
            prop_actualtagList.ClearArray();
            for (int i = 0; i < tagList.Count; i++) {
                prop_actualtagList.InsertArrayElementAtIndex(i);
                prop_actualtagList.GetArrayElementAtIndex(i).stringValue = tagList[i];
            }
            prop_state.intValue = (int)MultipleTagSelector.State.EVERYTHING;
            property.serializedObject.ApplyModifiedProperties();
        }

        void UpdateState() {
            if (actualTagList.Count == 0) {
                state = MultipleTagSelector.State.NONE;
            } else if (actualTagList.Count == tagList.Count) {
                state = MultipleTagSelector.State.EVERYTHING;
            } else {
                state = MultipleTagSelector.State.MULTIPLE;
            }
            prop_state.intValue = (int)state;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }
}
