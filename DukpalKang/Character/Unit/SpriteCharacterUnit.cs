using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;

namespace RasingDeokPal.Character.Unit
{
    /// <summary>
    /// 애니메이션 캐릭터 유닛
    /// </summary>
    internal class SpriteCharacterUnit : SpriteUnit
    {
        public SpriteCharacterUnit(Canvas canvas, string imgUri, int width, int height, int frameWidth, int frameHeight, int frameColumn, int frameRow, int marginLeft, int marginTop, bool ScaleRight) 
            : base(canvas, imgUri, frameWidth, frameHeight, frameColumn, frameRow, 12)
        {
            // 이미지 사이즈 조절
            SetUIElementSize(width, height);
            animation.SetSize(width, height);
            SetImageScaleRight(ScaleRight);
            
            // 이미지 위치 조정
            Point pos = WindowControlMethod.GetWindowSize();
            SetUIPosition(((int)pos.X / 2 + marginLeft), (int)pos.Y / 2 + marginTop);
            SetAnimationSpeed(100);

            // 애니메이션
            SetAnimationLoop(true);
            AnimationStart();
        }
    }
}
