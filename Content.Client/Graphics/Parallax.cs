using SS14.Client.Graphics.Overlays;
using SS14.Client.Interfaces.Graphics.Overlays;
using SS14.Client.Interfaces.ResourceManagement;
using SS14.Client.ResourceManagement;
using SS14.Client.ResourceManagement.ResourceTypes;
using SS14.Client.Scenes;

namespace Content.Client.Graphics
{
    public class Parallax : SceneOverlay
    {
        IResourceCache resourceCache;

        public override OverlaySpace Space => OverlaySpace.ScreenSpace;

        public Parallax() : base("parallax")
        {
            var sprite = new SpriteNode("star");
            var tex = resourceCache.GetResource<TextureResource>("/Textures/Effects/SpaceTexture.png");
            sprite.Texture = tex;
            Scene.AddChild(sprite);
        }
    }
}
