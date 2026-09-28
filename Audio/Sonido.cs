using System;
using System.IO;
using System.Media;
using System.Text;

namespace Tetris2D.Audio
{
    // Enumeración que define los diferentes eventos de sonido disponibles en el juego
    public enum TonoJuego
    {
        Mover,
        Rotar,
        Fijar,
        Linea,
        Nivel,
        GameOver
    }

    public static class Sonido
    {
        // Frecuencia de muestreo estándar de audio digital (22.05 kHz)[cite: 9]
        private const int Tasa = 22050;

        // Método principal para reproducir un sonido de manera asíncrona[cite: 9]
        public static void Reproducir(TonoJuego tono)
        {
            // Construye el arreglo de bytes del archivo WAV en memoria
            byte[] wav = ConstruirWav(tono);
            if (wav.Length == 0)
                return;

            // Utiliza MemoryStream y SoundPlayer de .NET para reproducir el audio generado
            using var ms = new MemoryStream(wav);
            using var jugador = new SoundPlayer(ms);
            jugador.Play(); // Reproducción asíncrona del tono[cite: 9]
        }

        // Configura los parámetros (frecuencia inicial, frecuencia final, duración y volumen) para cada evento[cite: 9]
        private static byte[] ConstruirWav(TonoJuego tono)
        {
            return tono switch
            {
                TonoJuego.Mover => CrearBarrido(980, 980, 0.05, 0.30),
                TonoJuego.Rotar => CrearBarrido(740, 740, 0.05, 0.30),
                TonoJuego.Fijar => CrearBarrido(330, 300, 0.10, 0.50),
                TonoJuego.Linea => CrearBarrido(660, 990, 0.18, 0.60),
                TonoJuego.Nivel => CrearBarrido(440, 880, 0.28, 0.60),
                TonoJuego.GameOver => CrearBarrido(420, 90, 0.70, 0.65),
                _ => Array.Empty<byte>()
            };
        }

        // Genera las muestras PCM de una onda senoidal aplicando fórmulas matemáticas de audio[cite: 9]
        private static byte[] CrearBarrido(double fInicio, double fFin, double segundos, double volumen)
        {
            int n = (int)(Tasa * segundos); // Cantidad total de muestras
            var muestras = new byte[n * 2]; // 16 bits por muestra (2 bytes por canal)

            for (int i = 0; i < n; i++)
            {
                double t = i / (double)Tasa; // Tiempo actual en segundos[cite: 9]
                double progreso = i / (double)Math.Max(1, n - 1);
                double frecuencia = fInicio + (fFin - fInicio) * progreso; // Interpolación de frecuencia[cite: 9]

                // Función matemática de onda senoidal[cite: 9]
                double seno = Math.Sin(2 * Math.PI * frecuencia * t);

                // Conversión a formato PCM de 16 bits con control de volumen y saturación
                short pcm = (short)Math.Clamp((double)(short)(seno * short.MaxValue * volumen), short.MinValue, short.MaxValue);

                // Almacenamiento de bytes (Little-Endian)
                muestras[i * 2] = (byte)(pcm & 0xFF);
                muestras[i * 2 + 1] = (byte)((pcm >> 8) & 0xFF);
            }
            return EnvolverEnWav(muestras);
        }

        // Construye la cabecera estándar RIFF / WAV de 44 bytes para encapsular los datos PCM[cite: 9]
        private static byte[] EnvolverEnWav(byte[] pcm)
        {
            var wav = new byte[44 + pcm.Length];

            Array.Copy(Encoding.ASCII.GetBytes("RIFF"), 0, wav, 0, 4);
            BitConverter.GetBytes(36 + pcm.Length).CopyTo(wav, 4);
            Array.Copy(Encoding.ASCII.GetBytes("WAVE"), 0, wav, 8, 4);
            Array.Copy(Encoding.ASCII.GetBytes("fmt "), 0, wav, 12, 4);
            BitConverter.GetBytes(16).CopyTo(wav, 16); // Tamaño del subchunk de formato[cite: 9]
            BitConverter.GetBytes((short)1).CopyTo(wav, 20); // Formato PCM[cite: 9]
            BitConverter.GetBytes((short)1).CopyTo(wav, 22); // Canal Mono[cite: 9]
            BitConverter.GetBytes(Tasa).CopyTo(wav, 24); // Frecuencia de muestreo[cite: 9]
            BitConverter.GetBytes(Tasa * 2).CopyTo(wav, 28); // Bytes por segundo[cite: 9]
            BitConverter.GetBytes((short)2).CopyTo(wav, 32); // Alineación de bloque[cite: 9]
            BitConverter.GetBytes((short)16).CopyTo(wav, 34); // Bits por muestra[cite: 9]
            Array.Copy(Encoding.ASCII.GetBytes("data"), 0, wav, 36, 4);
            BitConverter.GetBytes(pcm.Length).CopyTo(wav, 40); // Tamaño total de datos PCM[cite: 9]
            Array.Copy(pcm, 0, wav, 44, pcm.Length);

            return wav;
        }
    }
}