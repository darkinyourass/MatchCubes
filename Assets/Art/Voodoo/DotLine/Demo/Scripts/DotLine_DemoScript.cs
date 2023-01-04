using UnityEngine;

namespace Voodoo.Render.Shaders
{
	public class DotLine_DemoScript : MonoBehaviour
	{
		public Renderer lineRenderer;
		public float speed = 1f;
		private Vector2 mainTextureOffset;

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			if (lineRenderer != null)
				mainTextureOffset = lineRenderer.material.mainTextureOffset;
		}

		private void Update()
		{
			UpdateMainTextureOffset();
		}

		private void UpdateMainTextureOffset()
		{
			if (lineRenderer != null)
			{
				mainTextureOffset = new Vector2(mainTextureOffset.x + Time.deltaTime * speed, mainTextureOffset.y);
				lineRenderer.material.mainTextureOffset = mainTextureOffset;
			}
		}
	}
}
