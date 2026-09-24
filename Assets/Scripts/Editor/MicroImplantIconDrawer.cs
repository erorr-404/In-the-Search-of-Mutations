using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MicroImplantIconDrawer
{
    static MicroImplantIconDrawer()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset")) return;

        MicroImplantData data = AssetDatabase.LoadAssetAtPath<MicroImplantData>(path);
        if (data == null || data.Icon == null) return;

        Sprite sprite = data.Icon;
        Texture2D texture = sprite.texture;
        if (texture == null) return;

        // Рахуємо UV-координати конкретного спрайта в атласі
        Rect texRect = sprite.rect;
        Rect uv = new Rect(
            texRect.x / texture.width,
            texRect.y / texture.height,
            texRect.width / texture.width,
            texRect.height / texture.height
        );

        Color bgColor = EditorGUIUtility.isProSkin 
            ? new Color(0.219f, 0.219f, 0.219f, 1f) 
            : new Color(0.76f, 0.76f, 0.76f, 1f);

        // Режим великих іконок (сітка, як у тебе зараз)
        if (selectionRect.height > 32f)
        {
            float iconSize = selectionRect.width;
            Rect iconRect = new Rect(selectionRect.x, selectionRect.y, iconSize, iconSize);

            // 1. Затираємо синій кубик фоновим кольором вікна
            EditorGUI.DrawRect(iconRect, bgColor);

            // 2. Малюємо вирізаний спрайт по центру
            float padding = 6f;
            Rect spriteRect = new Rect(
                iconRect.x + padding,
                iconRect.y + padding,
                iconRect.width - padding * 2f,
                iconRect.height - padding * 2f
            );

            GUI.DrawTextureWithTexCoords(spriteRect, texture, uv, true);
        }
        else
        {
            // Режим маленького списку
            Rect iconRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);
            EditorGUI.DrawRect(iconRect, bgColor);
            GUI.DrawTextureWithTexCoords(iconRect, texture, uv, true);
        }
    }
}