using ObjectPool.RunTime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ObjectPool.Editor
{
    
    [CustomEditor(typeof(PoolingItemSO))]
    public class CustomPoolingItem : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset visualTree = default;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            visualTree.CloneTree(root);

            var codeNameField = root.Q<TextField>("PoolingNameField");
            codeNameField.RegisterValueChangedCallback(HandleAssetNameChange);
                
            return root;
        }
        
        private void HandleAssetNameChange(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.newValue))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a name.", "OK");
                return;
            }
            
            string assetPath = AssetDatabase.GetAssetPath(target);
            string newName = $"{evt.newValue}";
        
            var message = AssetDatabase.RenameAsset(assetPath, newName);
            if (string.IsNullOrEmpty(message))
                target.name = newName;
            else
            {
                EditorUtility.DisplayDialog("Error", message, "OK");
            }
        }

    }
}