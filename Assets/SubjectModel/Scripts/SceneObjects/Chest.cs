using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    public class Chest : SceneObject
    {
        private static readonly Rect WindowRect = new Rect(280f, 156f, 1360f, 768f); 
        
        protected override void DrawGUI()
        {
            GUILayout.Window(1, WindowRect, id =>
            {
                GUILayout.BeginHorizontal();
                
                GUILayout.BeginVertical("Box");
                GUILayout.EndVertical();
                
                GUILayout.BeginVertical("Box");
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }, ButtonText);
        }
    }
}
