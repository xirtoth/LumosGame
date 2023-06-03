using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TorchShader
{
    private readonly Effect effect;

    public TorchShader(Effect effect)
    {
        this.effect = effect;
    }

    public void Apply()
    {
        effect.CurrentTechnique.Passes[0].Apply();
    }

    public void SetTorchPosition(Vector2 position)
    {
        effect.Parameters["TorchPosition"].SetValue(position);
    }

    public void SetTorchRadius(Vector2 radius)
    {
        effect.Parameters["TorchRadius"].SetValue(radius);
    }

    public void SetTorchIntensity(float intensity)
    {
        effect.Parameters["TorchIntensity"].SetValue(intensity);
    }
}