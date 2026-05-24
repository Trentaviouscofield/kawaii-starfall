#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BuildMainScene
{
    [MenuItem("Kawaii Starfall/Build Main Scene")]
    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "MainScene";

        var cameraGo = new GameObject("Main Camera", typeof(Camera));
        var cam = cameraGo.GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cameraGo.transform.position = new Vector3(0, 0, -10);

        CreateStarfieldBackground();

        var gameManager = new GameObject("GameManager").AddComponent<GameManager>();
        var uiManager = new GameObject("UIManager").AddComponent<UIManager>();
        var spawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();

        var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);

        var playerPrefab = CreatePlayerPrefab();
        var enemyPrefab = CreateEnemyPrefab();
        var bulletPrefab = CreateBulletPrefab();
        var orbPrefab = CreateUpgradeOrbPrefab();

        var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        player.name = "Player";
        player.transform.position = new Vector3(0, -4.8f, 0);
        var firePoint = new GameObject("FirePoint").transform;
        firePoint.SetParent(player.transform);
        firePoint.localPosition = new Vector3(0, 0.8f, 0);

        var pc = player.GetComponent<PlayerController>();
        SetSerializedObject(pc, "bulletPrefab", bulletPrefab);
        SetSerializedObject(pc, "firePoint", firePoint);

        var enemyComp = enemyPrefab.GetComponent<Enemy>();
        SetSerializedObject(enemyComp, "upgradeOrbPrefab", orbPrefab);
        EditorUtility.SetDirty(enemyPrefab);

        SetSerializedObject(spawner, "enemyPrefab", enemyPrefab);
        SetSerializedFloat(spawner, "xSpawnLimit", 7.5f);
        SetSerializedFloat(spawner, "ySpawnPosition", 6.2f);
        SetSerializedFloat(spawner, "bottomLimit", -6.4f);
        SetSerializedFloat(spawner, "baseEnemySpeed", 0.9f);
        SetSerializedFloat(spawner, "spawnInterval", 1.4f);

        BuildUI(canvasGo.transform, uiManager);

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/MainScene.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("MainScene built and wired.");
    }

    static void BuildUI(Transform canvas, UIManager ui)
    {
        var startPanel = CreatePanel(canvas, "StartScreenPanel", new Color(0f, 0f, 0f, 0.42f));
        var startCanvasGroup = startPanel.AddComponent<CanvasGroup>();
        var title = CreateTMP(startPanel.transform, "Title", "Kawaii Starfall", 72, new Vector2(0, 200));
        var subtitle = CreateTMP(startPanel.transform, "Subtitle", "A neon arcade shooter", 36, new Vector2(0, 120));
        var startBtn = CreateButton(startPanel.transform, "StartButton", "Start Game", new Vector2(0, 0));

        var score = CreateTMP(canvas, "ScoreText", "Score: 0", 36, new Vector2(-780, 480));
        var wave = CreateTMP(canvas, "WaveText", "Wave: 1", 36, new Vector2(0, 480));
        var hearts = CreateTMP(canvas, "HeartsText", "Hearts: 3", 36, new Vector2(780, 480));
        var weapon = CreateTMP(canvas, "WeaponText", "Weapon Lv: 1", 30, new Vector2(0, 430));

        var gameOver = CreatePanel(canvas, "GameOverPanel", new Color(0f, 0f, 0f, 0.75f));
        CreateTMP(gameOver.transform, "GameOverText", "GAME OVER", 72, new Vector2(0, 120));
        var restartBtn = CreateButton(gameOver.transform, "RestartButton", "Restart", new Vector2(0, 0));

        var so = new SerializedObject(ui);
        so.FindProperty("scoreText").objectReferenceValue = score;
        so.FindProperty("waveText").objectReferenceValue = wave;
        so.FindProperty("heartsText").objectReferenceValue = hearts;
        so.FindProperty("weaponText").objectReferenceValue = weapon;
        so.FindProperty("startScreenPanel").objectReferenceValue = startPanel;
        so.FindProperty("startScreenCanvasGroup").objectReferenceValue = startCanvasGroup;
        so.FindProperty("titleText").objectReferenceValue = title;
        so.FindProperty("subtitleText").objectReferenceValue = subtitle;
        so.FindProperty("gameOverPanel").objectReferenceValue = gameOver;
        so.ApplyModifiedPropertiesWithoutUndo();

        UnityEventTools.AddPersistentListener(startBtn.onClick, ui.OnStartGamePressed);
        UnityEventTools.AddPersistentListener(restartBtn.onClick, ui.OnRestartPressed);
    }

    static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = color;
        return go;
    }

    static void CreateStarfieldBackground()
    {
        var go = new GameObject("StarfieldBackground", typeof(SpriteRenderer));
        var renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = CreateStarfieldSprite(512, 512, 220);
        renderer.sortingOrder = -100;
        go.transform.position = new Vector3(0f, 0f, 0f);
        go.transform.localScale = new Vector3(18f, 13f, 1f);
    }

    static Sprite CreateStarfieldSprite(int width, int height, int stars)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var bg = new Color(0.03f, 0.04f, 0.1f, 1f);
        var pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = bg;

        Random.InitState(1337);
        for (int i = 0; i < stars; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            float b = Random.Range(0.55f, 0.95f);
            var c = new Color(b, b, b, 1f);
            pixels[y * width + x] = c;
        }

        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 64f);
    }

    static TMP_Text CreateTMP(Transform parent, string name, string text, int size, Vector2 anchored)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text; tmp.fontSize = size; tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1200, 120); rt.anchoredPosition = anchored;
        return tmp;
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 anchored)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.1f, 0.5f, 0.9f, 1f);
        var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(360, 100); rt.anchoredPosition = anchored;
        var t = CreateTMP(go.transform, "Label", label, 40, Vector2.zero);
        t.rectTransform.sizeDelta = rt.sizeDelta;
        return go.GetComponent<Button>();
    }

    static GameObject CreatePlayerPrefab()
    {
        var go = new GameObject("PlayerPlaceholder", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(PlayerController));
        var renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = LoadSpriteOrFallback("Assets/Art/player/playerShip1_blue.png", new Color(0.2f, 1f, 1f), "player");
        renderer.sortingOrder = 10;
        go.transform.localScale = new Vector3(1.4f, 1f, 1f);
        go.GetComponent<BoxCollider2D>().isTrigger = true;
        go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return SavePrefab(go, "Assets/Prefabs/PlayerPlaceholder.prefab");
    }
    static GameObject CreateEnemyPrefab()
    {
        var go = new GameObject("EnemyPlaceholder", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(Enemy));
        var renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = LoadSpriteOrFallback("Assets/Art/enemies/enemyBlack2.png", new Color(0.1f, 0.1f, 0.15f), "enemy");
        renderer.sortingOrder = 10;
        go.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        go.GetComponent<CircleCollider2D>().isTrigger = true;
        go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return SavePrefab(go, "Assets/Prefabs/EnemyPlaceholder.prefab");
    }
    static GameObject CreateBulletPrefab()
    {
        var go = new GameObject("BulletPlaceholder", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(Bullet));
        var renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = LoadSpriteOrFallback("Assets/Art/projectiles/laserBlue02.png", new Color(0.6f, 1f, 0.95f), "projectile");
        renderer.sortingOrder = 12;
        go.transform.localScale = new Vector3(0.35f, 0.7f, 1);
        go.GetComponent<CircleCollider2D>().isTrigger = true;
        go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return SavePrefab(go, "Assets/Prefabs/BulletPlaceholder.prefab");
    }
    static GameObject CreateUpgradeOrbPrefab()
    {
        var go = new GameObject("UpgradeOrbPlaceholder", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(UpgradeOrb));
        var renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = MakeSprite(new Color(1f, 0.5f, 1f));
        renderer.sortingOrder = 11;
        go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        go.GetComponent<CircleCollider2D>().isTrigger = true;
        go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        return SavePrefab(go, "Assets/Prefabs/UpgradeOrbPlaceholder.prefab");
    }

    static GameObject SavePrefab(GameObject go, string path)
    {
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    static Sprite MakeSprite(Color color)
    {
        var tex = new Texture2D(32, 32);
        var px = new Color[32 * 32];
        for (int i = 0; i < px.Length; i++) px[i] = color;
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 16);
    }

    static Sprite LoadSpriteOrFallback(string assetPath, Color fallbackColor, string category)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (sprite != null)
        {
            Debug.Log($"Using configured {category} sprite: {assetPath}");
            return sprite;
        }

        Debug.LogWarning($"Configured {category} sprite not found at {assetPath}. Using generated placeholder sprite.");
        return MakeSprite(fallbackColor);
    }

    static void SetSerializedObject(Object target, string fieldName, Object value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(fieldName).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void SetSerializedFloat(Object target, string fieldName, float value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(fieldName).floatValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }
}
#endif
