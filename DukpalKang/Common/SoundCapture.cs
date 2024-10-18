using NAudio.Dsp;

namespace RasingDeokPal.Common
{
    internal class SoundCapture
    {


        /// <summary>
        /// 사운드 레벨 반환
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static float CalculateRMSLevel(byte[] buffer)
        {
            int bytesPerSample = 2; // 16비트 오디오
            int sampleCount = buffer.Length / bytesPerSample;
            float sumOfSquares = 0;
            for (int i = 0; i < sampleCount; i++)
            {
                short sample = BitConverter.ToInt16(buffer, i * bytesPerSample);
                sumOfSquares += sample * sample;
            }
            float rms = (float)Math.Sqrt(sumOfSquares / sampleCount);
            return rms;
        }

        /// <summary>
        /// 베이스 사운드 변환
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferSize"></param>
        public  static double ProcessAudioData(byte[] buffer, int bytesRecorded, int startHz, int endHz)
        {
            int fftSize = 2048; // FFT 크기 설정
            float[] floatBuffer = new float[fftSize];
            int samplesCount = Math.Min(fftSize, bytesRecorded / 2);

            // 바이트 데이터를 float 배열로 변환
            for (int i = 0; i < samplesCount; i++)
            {
                short sample = BitConverter.ToInt16(buffer, i * 2);
                floatBuffer[i] = sample / 32768f; // -1.0f에서 1.0f 범위로 변환
            }

            // FFT 분석
            var fft = new Complex[fftSize];
            for (int i = 0; i < fftSize; i++)
            {
                fft[i] = new Complex { X = floatBuffer[i] };
            }
            FastFourierTransform.FFT(true, (int)Math.Log(fftSize, 2), fft);

            // 특정 주파수 범위의 에너지 계산
            float bassEnergy = 0;
            int sampleRate = 44100;
            int startBin = (int)(startHz * fftSize / sampleRate); // 60Hz 시작 주파수
            int endBin = (int)(endHz * fftSize / sampleRate); // 250Hz 끝 주파수

            for (int i = startBin; i <= endBin; i++)
            {
                bassEnergy += fft[i].X * fft[i].X + fft[i].Y * fft[i].Y;
            }

            //Debug.WriteLine($"Bass Energy: {bassEnergy}");
            return bassEnergy;
        }
    }
}
