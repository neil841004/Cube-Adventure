using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class GhostScriptableWizard : ScriptableWizard
{
    public delegate void VoidFunc();
    public VoidFunc OnCreate = null;

    public string characterName = "Ghost Effect 3D";
    public GameObject empty;
    private GameObject curObj = null;

    [MenuItem("Ghost Effect 2D e 3D/Create Ghost Effect/Create Ghost Effect 3D")]
    public static GhostScriptableWizard CreateQuestPro5()
    {
        GhostScriptableWizard creator = (GhostScriptableWizard)DisplayWizard("Ghost Effect 3D Create", typeof(GhostScriptableWizard));

        creator.helpString = "Drag an empty template that is in the scene to add all parameters within the Ghost effect";
        return creator;
    }

    void OnWizardUpdate()
    {
        if (empty != curObj)
        {

            characterName = empty.name;
        }
        curObj = empty;
    }

    void OnWizardCreate()
    {
        if (!empty)
        {
            UnityEditor.EditorUtility.DisplayDialog("Error", "Character not assiged.", "OK");
            return;
        }

        if (!empty.activeInHierarchy)
        {
            empty.SetActive(true);
        }

        PrefabType ptype = PrefabUtility.GetPrefabType(empty);
        bool needs2create = ptype == PrefabType.ModelPrefab;


        if (needs2create)
        {
            empty = Instantiate(empty);
            Undo.RegisterCreatedObjectUndo(empty, "Create Ghost Effect 3D");
        }
        empty.name = characterName;

        //Load functions down
        _addScript();
        _addMeshFilter();
    }

    bool _addScript()
    {
        PlayerGhost3D playerGhost = empty.GetComponent<PlayerGhost3D>();
        if (!playerGhost)
        {
            playerGhost = Undo.AddComponent<PlayerGhost3D>(empty);
        }

        return true;
    }

    bool _addMeshFilter()
    {
        MeshRenderer meshRenderer = empty.GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            meshRenderer = Undo.AddComponent<MeshRenderer>(empty);
        }

        MeshFilter meshFilter = empty.GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            meshFilter = Undo.AddComponent<MeshFilter>(empty);
        }

        return true;
    }
}

public class Ghost2DScriptableWizard : ScriptableWizard
{
    public delegate void VoidFunc();
    public VoidFunc OnCreate = null;

    public string characterName2D = "Ghost Effect 2D";
    public GameObject empty;
    private GameObject curObj = null;

    [MenuItem("Ghost Effect 2D e 3D/Create Ghost Effect/Create Ghost Effect 2D")]
    public static Ghost2DScriptableWizard CreateQuestPro5()
    {
        Ghost2DScriptableWizard creator = (Ghost2DScriptableWizard)DisplayWizard("Ghost Effect 2D Create", typeof(Ghost2DScriptableWizard));

        creator.helpString = "Drag an empty template that is in the scene to add all parameters within the Ghost effect";
        return creator;
    }

    void OnWizardUpdate()
    {
        if (empty != curObj)
        {

            characterName2D = empty.name;
        }
        curObj = empty;
    }

    void OnWizardCreate()
    {
        if (!empty)
        {
            UnityEditor.EditorUtility.DisplayDialog("Error", "Character not assiged.", "OK");
            return;
        }

        if (!empty.activeInHierarchy)
        {
            empty.SetActive(true);
        }

        PrefabType ptype = PrefabUtility.GetPrefabType(empty);
        bool needs2create = ptype == PrefabType.ModelPrefab;


        if (needs2create)
        {
            empty = Instantiate(empty);
            Undo.RegisterCreatedObjectUndo(empty, "Create Ghost Effect 2D");
        }
        empty.name = characterName2D;

        //Load functions down
        _addScript();
        _SpriteRenderer();
    }

    bool _addScript()
    {
        PlayerGhost playerGhost = empty.GetComponent<PlayerGhost>();
        if (!playerGhost)
        {
            playerGhost = Undo.AddComponent<PlayerGhost>(empty);
        }

        // playerGhost.colorGhost = new Vector4(255f, 255f, 255, 122f);
        Color color = Color.white;
        color.a = 0.5f;
        playerGhost.colorGhost = color;

        return true;
    }

    bool _SpriteRenderer()
    {
        SpriteRenderer meshRenderer = empty.GetComponent<SpriteRenderer>();
        if (!meshRenderer)
        {
            meshRenderer = Undo.AddComponent<SpriteRenderer>(empty);
        }

        return true;
    }
}

public class LoadGhost3DScriptableWizard : ScriptableWizard
{
    public GameObject meshPlayer;
    public GameObject ghost3D;
    private GameObject curObj = null;

    [MenuItem("Ghost Effect 2D e 3D/Load Ghost Effect/Load Ghost Effect 3D")]
    public static LoadGhost3DScriptableWizard CreateQuestPro5()
    {
        LoadGhost3DScriptableWizard creator = (LoadGhost3DScriptableWizard)DisplayWizard("Ghost Effect 3D Create", typeof(LoadGhost3DScriptableWizard));

        creator.helpString = "Drag an Mesh Ghost 3D template that is in the scene to add all parameters within the Ghost effect";
        return creator;
    }

    void OnWizardUpdate()
    {
        if (ghost3D != curObj)
        {
        }
        curObj = ghost3D;
    }

    void OnWizardCreate()
    {
        if (!ghost3D)
        {
            UnityEditor.EditorUtility.DisplayDialog("Error", "NPC Quest not assiged.", "OK");
            return;
        }
        if (!ghost3D.activeInHierarchy)
        {
            ghost3D.SetActive(true);
        }
        ghost3D.GetComponent<MeshFilter>().mesh = meshPlayer.GetComponent<MeshFilter>().mesh;

    }
}
