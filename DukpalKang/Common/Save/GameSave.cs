using System.Diagnostics;
using System.IO;
using System.Text.Json;
using static RasingDeokPal.Common.Save.SaveDataFormat;

namespace RasingDeokPal.Common.Save
{
    internal class GameSave
    {
        private static string dirName = "RagingDeokPal";
        // 2. 새 버전 데이터 저장
        private static string saveFileName = "save.ini";

        /// <summary>
        /// 데이터 저장
        /// </summary>
        /// <param name="data"></param>
        public static void Save(GameData data)
        {
            try
            {
                string iniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dirName, saveFileName);
                // 디렉토리가 존재하지 않으면 생성
                string directoryPath = Path.GetDirectoryName(iniFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // 게임 파일 작성
                string jsonStr = JsonSerializer.Serialize<GameData>(data);
                File.WriteAllText(iniFilePath, jsonStr);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"""세이브 데이터 저장 실패""");
            }
        }
        
        /// <summary>
        /// 데이터 불러오기
        /// </summary>
        /// <returns></returns>
        public static GameData Load()
        {
            try
            {
                // 파일 로드
                string iniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dirName, saveFileName);
                // json string 변환
                IniJsonParser parser = new IniJsonParser(iniFilePath);
                GameData saveData = parser.ParseFile<GameData>();

                return saveData;
            }
            catch(Exception e)
            {
                // 기본 데이터 반환
                Debug.WriteLine($"""세이브 데이터 로드 실패""");
                return new GameData();
            }
        }

    }
}
