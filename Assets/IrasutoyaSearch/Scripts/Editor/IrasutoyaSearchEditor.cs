using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


namespace Ho.IrasutoyaSearch
{
#if UNITY_EDITOR
    public class IrasutoyaSearchEditor : EditorWindow
    {
        [MenuItem("IrasutoyaSearch/IrasutoyaSearchWindow")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<IrasutoyaSearchEditor>();

        }

        private string searchWord = "";
        private string savePath = "Assets/IrasutoyaSearch/Images";

        private float verticalScrollbar = 0.0f;
        private float timer = 0;
        private int maxSize = 10;

        private Vector2 scrollPos;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                Repaint();
                timer = 0;
            }
        }


        private void OnGUI()
        {

            /*
            GUILayout.Label("Select an object in the hierarchy view");
            if (Selection.activeGameObject)
            {
                Selection.activeGameObject.name =
                    EditorGUILayout.TextField("Object Name: ", Selection.activeGameObject.name);
            }
            */
            GUILayout.Label("Search word");
            searchWord =GUILayout.TextField(searchWord);
            GUILayout.Label("Save path");
            savePath = GUILayout.TextField(savePath);
            GUILayout.Label("Max size");
            maxSize = EditorGUILayout.IntField(maxSize);

            if (GUILayout.Button("Search"))
            {
                if (string.IsNullOrEmpty(searchWord))
                    return;
                IrasutoyaSearcher.SearchImagesAsync(searchWord,maxSize);
            }


            var imageList = IrasutoyaSearcher.imageList;

            if (imageList == null)
            {
                return;
            }

            //EditorGUILayout.BeginHorizontal(GUI.skin.box);


            //EditorGUILayout.BeginVertical(GUI.skin.box);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUI.skin.box);
            if (imageList!=null)
            {
                for (int i = 0; i < imageList.Count; i++)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Box(IrasutoyaSearcher.imageList[i], GUILayout.Width(256 / 4), GUILayout.Height(256 / 4));
                        GUILayout.FlexibleSpace();
                        //
                    }
                    EditorGUILayout.EndHorizontal();


                    if (GUILayout.Button("Download"))
                    {
                        IrasutoyaSearcher.CreateAsset(IrasutoyaSearcher.dataList[i],savePath);
                    }
                    /*
                    if (GUILayout.Button("DownloadSprite"))
                    {
                        IrasutoyaManager2.CreateAsset(IrasutoyaManager2.dataList[i], savePath);
                    }
                    */
                    EditorGUILayout.EndVertical();
                                       
                }

            }

            //EditorGUILayout.EndVertical();


            //float verticalSize = 10.0f;
            //verticalScrollbar = GUILayout.VerticalScrollbar(verticalScrollbar, verticalSize, 0.0f, 100.0f, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            //EditorGUILayout.EndHorizontal();
        }
    }
#endif
}