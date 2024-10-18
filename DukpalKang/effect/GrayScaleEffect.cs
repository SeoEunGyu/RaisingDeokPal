using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace RasingDeokPal.Effect
{
    public class GrayscaleEffect : ShaderEffect
    {
        private static readonly PixelShader pixelShader = new PixelShader()
        {
            UriSource = new Uri("pack://application:,,,/effect/GrayScaleEffect.ps", UriKind.Absolute)
        };

        public GrayscaleEffect()
        {
            PixelShader = pixelShader;
            UpdateShaderValue(InputProperty);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty(
                "Input", typeof(GrayscaleEffect), 0);
    }
}
