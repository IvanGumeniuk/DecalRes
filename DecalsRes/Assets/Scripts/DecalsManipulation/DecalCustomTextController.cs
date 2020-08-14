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

    public bool HasTextDecal(int id)
	{
        return renderTextures.ContainsKey(id);
    }

	public void CreateNewTexture(int id, Font font)
	{
        if (renderTextures.ContainsKey(id))
            return;

        RenderTexture texure = new RenderTexture(textureOrigin);
        texure.Create();
        renderTextures.Add(id, new RenderTextureInfo(renderText.text, font, texure));
    }

    public void StoreDecalText(int id)
	{
        if (!renderTextures.ContainsKey(id))
            return;

        renderTextures[id].text = renderText.text;
    }

    public void SetTextureToCamera(int id)
	{
        if (!renderTextures.ContainsKey(id))
        {
            SetTexture(textureOrigin);
            SetText(defaultText);
            return;
        }

        SetText(renderTextures[id].text, renderTextures[id].font);
        SetTexture(renderTextures[id].texture);
    }

    public string GetText(int id)
	{
        if (!renderTextures.ContainsKey(id))
            return string.Empty;

        return renderTextures[id].text;
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

    public void SetText(string text, Font font = null)
	{
        renderText.text = text;
        renderText.font = font == null ? renderText.font : font;
    }

    public void SetTexture(RenderTexture texture)
	{
        renderCamera.targetTexture = texture;
    }

    private class RenderTextureInfo
	{
        public string text;
        public Font font;
        public RenderTexture texture;

		public RenderTextureInfo(string text, Font font, RenderTexture texture)
		{
			this.text = text;
            this.font = font;
			this.texture = texture;
		}
	}
}
