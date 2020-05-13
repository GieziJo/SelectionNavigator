// ===============================
// AUTHOR          : J. Giezendanner
// CREATE DATE     : 13.05.2020
// MODIFIED DATE   : 
// PURPOSE         : Creates a simple navigatable selection log
// SPECIAL NOTES   : 
// ===============================
// Change History:
//==================================


using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GieziTools
{
    [InitializeOnLoad]
    public class SelectionNavigator
    {
        private static SelectionNavigatorHistory _selectionNavigationHistory; // scriptable object which remebers the selection list
        private const string SELECTIONS_NAVIGATOR_HISTORY_PATH = "Assets/Editor/GieziTools/SelectionNavigator/SelectionNavigatorHistory.asset"; // scriptable object path
        private const int MAX_SELECTIONS = 50; // number of objects to remember
        
        private static bool _navigating = false; // checks if user is currently navigating

        private static int _historyIndex = 0; // check where the user is in the navigation
    
        public static List<Object> SelectionNavigationList // link to the history list in the so
        {
            get
            {
                CheckSelectionLog();
                return _selectionNavigationHistory.SelectionNavigationList;
            }
            set
            {
                CheckSelectionLog();
                _selectionNavigationHistory.SelectionNavigationList = value;
            }
        }

        // Called on load, initialises the class
        static SelectionNavigator()
        {
            // initialise log
            InitialiseSelectionLog();
            // capture selection changed events
            Selection.selectionChanged += OnSelectionChanged;
            // capture change of scene events to remove invalid references
            EditorSceneManager.sceneOpened += (path, mode) => CleanSelectionList();
        }
        
        // initialise log
        static void InitialiseSelectionLog()
        {
            // try to load default so
            _selectionNavigationHistory = AssetDatabase.LoadAssetAtPath<SelectionNavigatorHistory>(SELECTIONS_NAVIGATOR_HISTORY_PATH);
            // if null, create one
            if (_selectionNavigationHistory is null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SELECTIONS_NAVIGATOR_HISTORY_PATH));
                _selectionNavigationHistory = ScriptableWizard.CreateInstance<SelectionNavigatorHistory>();
                AssetDatabase.CreateAsset(_selectionNavigationHistory, SELECTIONS_NAVIGATOR_HISTORY_PATH);
            }
            else
            {
                // clean the selection list
                CleanSelectionList();
            }
        }
        
        // clean the list
        private static void CleanSelectionList()
        {
            // reset navigation history to 0
            _historyIndex = 0;
            // set navigating bool to false
            _navigating = false;
            // remove all invalid references from the list
            SelectionNavigationList.RemoveAll(item => item == null);
        }
        
        // check if the link to the selection log has been lost, if so get it
        private static void CheckSelectionLog()
        {
            if (_selectionNavigationHistory is null)
                InitialiseSelectionLog();
        }
        
        // called when the selection changes
        private static void OnSelectionChanged()
        {
            // if user is not navigating in list
            if (!_navigating)
            {
                // get active object
                Object activeObject = Selection.activeObject;
                // if active object is not null (not nothing selected or deleted)
                if (!(activeObject is null))
                {
                    // check if there is a histor index
                    if (_historyIndex != 0)
                    {
                        // if so, delete history head
                        int index = SelectionNavigationList.Count - _historyIndex;
                        SelectionNavigationList.RemoveRange(index, _historyIndex);
                        _historyIndex = 0;
                    }
                    // add current selection to list
                    SelectionNavigationList.Add(activeObject);
                    // remove first entries if selection history is too long
                    if(SelectionNavigationList.Count > MAX_SELECTIONS)
                        SelectionNavigationList.RemoveAt(0);
                }
                else
                {
                    // clean selection list because avtive object might be null from delete
                    CleanSelectionList();
                }
            }
            // set next step to be not a navigation step, unless called again from GoToPrevious or GoToNext
            _navigating = false;
        }
        
        // User callable function to got to previously selected object
        [Shortcut("SelectionNavigator.Previous", KeyCode.G, ShortcutModifiers.Action)]
        public static void GoToPrevious()
        {
            // if there is anything to go back to
            if (_historyIndex < (SelectionNavigationList.Count - 1))
            {
                // keep track of where in the list we are
                _historyIndex++;
                // update selection
                UpdateSelectionFromNavigation();
            }
        }

        // User callable function to got to next selected object
        [Shortcut("SelectionNavigator.Next", KeyCode.G, ShortcutModifiers.Alt)]
        public static void GoToNext()
        {
            // check if there is a next object to go to
            if (_historyIndex > 0)
            {
                // keep track of where in the list we are
                _historyIndex--;
                // update selection
                UpdateSelectionFromNavigation();
            }
        }
        
        // update selection
        private static void UpdateSelectionFromNavigation()
        {
            // inform that we are navigating, and thus should not add selected object to list head
            _navigating = true;
            // select object index in history
            int selectionIndex = SelectionNavigationList.Count - 1 - _historyIndex;
            // select object
            Selection.activeObject = SelectionNavigationList[selectionIndex];
        }
    
    }
}

// SO to keep track of objects
public class SelectionNavigatorHistory : ScriptableObject
{
    public List<Object> SelectionNavigationList = new List<Object>();
}