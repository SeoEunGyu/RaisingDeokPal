using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RasingDeokPal.Components
{
    internal class MouseCursor
    {
        public Cursor cursor;
        
        public MouseCursor(string cursorPath)
        {
            //cursor = new Cursor(CreateCursorStream(cursorPath));
            cursor = new Cursor(GetCursorStream(cursorPath));
        }

        /// <summary>
        /// 커서 생성
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private Stream GetCursorStream(string cursorPath)
        {
            // Pack URI에서 스트림을 가져옴
            var uri = new Uri(cursorPath, UriKind.Absolute);
    
            var streamResourceInfo = Application.GetResourceStream(uri);

            if (streamResourceInfo != null)
            {
                return streamResourceInfo.Stream;
            }
            else
            {
                throw new FileNotFoundException("Cursor file not found", cursorPath);
            }
        }
    }
}
