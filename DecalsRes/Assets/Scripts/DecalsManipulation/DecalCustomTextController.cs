using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecalCustomTextController : MonoBehaviour
{
    [SerializeField] private RenderTexture textureOrigin;

    [SerializeField] private Camera renderCamera;
    [SerializeField] private Text renderText;

    private string defaultText = "Enter text...";


    private Dictionary<int, RenderTextureInfo> renderTextures = new Dictionary<int, RenderTextureInfo>();

	private void Start()
	{
        renderText.text = defaultText;
    }

	public void CreateNewTexture(int id, int fontID)
	{
        if (renderTextures.ContainsKey(id))
            return;

        RenderTexture texure = new RenderTexture(textureOrigin);
        texure.Create();
        renderTextures.Add(id, new RenderTextureInfo(renderText.text, fontID, texure));
    }

    public void StoreDecalText(int id)
	{
        if (!renderTextures.ContainsKey(id))
            return;

        renderTextures[id].text = renderText.text;
    }

    public void SetTextureToCamera(int id, int fontID)
	{        
        if (!renderTextures.ContainsKey(id))
        {
            SetTexture(textureOrigin);
            SetText(defaultText);
            return;
        }

        if (renderTextures[id].fontID != fontID)
            renderTextures[id].fontID = fontID;

        SetText(renderTextures[id].text, renderTextures[id].fontID);
        SetTexture(renderTextures[id].texture);
    }

    public void GetText(int id, out int fontID, out string text)
	{
        if (!renderTextures.ContainsKey(id))
        {
            fontID = -1;
            text = string.Empty;
            return;
        }

        fontID = renderTextures[id].fontID;
        text = renderTextures[id].text;
    }

    public RenderTexture GetTexture(int id)
	{
        if (renderTextures.ContainsKey(id))
            return renderTextures[id].texture;

        return null;
    }

    public void RemoveTexture(int id)
	{
        if (!renderTextures.ContainsKey(id))
            return;

        renderTextures.Remove(id);
    }

    public void SetText(string text, int fontID = -1)
	{
		//Debug.Log($"SetText {text} {fontID}");
        renderText.text = text;
        renderText.font = fontID == -1 ? renderText.font : SettingsManager.Instance.textDecalSettings.GetFont(fontID);
    }

    public void SetTexture(RenderTexture texture)
	{
        renderCamera.targetTexture = texture;
    }

    private class RenderTextureInfo
	{
        public string text;
        public int fontID;
        public RenderTexture texture;

		public RenderTextureInfo(string text, int fontID, RenderTexture texture)
		{
			this.text = text;
            this.fontID = fontID;
            this.texture = texture;
		}
	}
}
