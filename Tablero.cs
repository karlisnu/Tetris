using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using Tetris2D.Graficos;
using Tetris2D.UI;

namespace Tetris2D
{
    public class Tablero  // Administra la cuadrícula lógica y el control de colisiones[cite: 1]
    {
        public const int Columnas = 10;
        public const int Filas = 20;
        public readonly int[,] _celdas = new int[Columnas, Filas];
        public int filaInicio;

        public int ObtenerCeldas(int x, int y) => _celdas[x, y]; // Retorna el valor numérico almacenado en una celda

        public void Limpiar() //limpiar la columnos y filas del tablero
        {
            for (int x = 0; x < Columnas; x++)
            {
                for (int y = 0; y < Filas; y++)
                {
                    _celdas[x, y] = 0;
                }
            }
        }
        public bool Colisiona(IReadOnlyList<(int x, int y)> bloques, int posX, int posY)// Valida si una pieza choca con paredes o bloques fijos
        {
            foreach ((int bx, int by) in bloques)
            {
                int x = posX + bx;
                int y = posY + by;
                if(x < 0 || x >= Columnas || y < 0 || y >= Filas) // Comprueba si se sale de los límites laterales o fondo
                    return true;
                if (_celdas[x, y] != 0)                           // Comprueba si la celda del tablero ya está ocupada
                    return true;
            }
            return false;
        }
        public void FijarPieza(Pieza pieza, int posX, int posY) // Fija la pieza en el tablero, marcando las celdas ocupadas
        {
            int ValorColor = (int)pieza.Tipo + 1;
            foreach ((int bx, int by) in pieza.Bloques)
            {
                int x = posX + bx;
                int y = posY + by;
                if (x >= 0 && y >= 0 && y < Filas)
                _celdas[x, y] = ValorColor;
            }
        }

        public int LimpiarLineasCompletas() // Revisa y elimina filas llenas, desplazando las de arriba hacia abajo
        {
            int lineasBorradas = 0;
            for (int y = Filas -1; y >= 0; y--)
            {
                if (FilaCompleta(y))
                {
                    lineasBorradas++;
                    BajarFilasSuperiores(y);
                    y++; // Vuelve a evaluar la misma fila tras el desplazamiento
                }

            }
            return lineasBorradas;
        }

        private void BajarFilasSuperiores(int fila)
        {
            for (int y = fila; y > 0; y--) 
            {
                for (int x = 0; x < Columnas; x++)
                {
                    _celdas[x, y] = _celdas[x, y - 1];
                }
            }
            for (int x = 0; x < Columnas; x++)
            {
                _celdas[x, 0] = 0;
            }
        }

        private bool FilaCompleta(int y)
        {
            for (int x = 0; x < Columnas; x++)
            {
                if (_celdas[x, y] == 0) return false;
            }
            return true;
        }
        public void Renderizar(DibujadorCuadros cuadros, float x0, float y0, float tamCelda) // Dibuja el fondo, cuadrícula y bloques fijos[cite: 1]
        {
            float ancho = Columnas * tamCelda;
            float alto = Filas * tamCelda;

            cuadros.DibujarRectangulo(TemaArcade.PanelOscuro, x0, y0, x0 + ancho, y0 + alto); // Dibuja el fondo del área de juego[cite: 1]
            float grosorBorde = MathF.Max(2f, tamCelda * 0.10f);
            cuadros.DibujarBordeRectangulo(TemaArcade.Cian, x0, y0, x0 + ancho, y0 + alto, grosorBorde); // Dibuja el borde neón[cite: 1]

            Vector4 colorCuadricula = new(TemaArcade.TextoSuave.X, TemaArcade.TextoSuave.Y, TemaArcade.TextoSuave.Z, 0.10f);
            for (int x = 1; x < Columnas; x++) // Dibuja las líneas verticales de la cuadrícula[cite: 1]
            {
                float lx = x0 + x * tamCelda;
                cuadros.DibujarLinea(colorCuadricula, lx, y0, lx, y0 + alto, 1f);
            }
            for (int y = 1; y < Filas; y++) // Dibuja las líneas horizontales de la cuadrícula[cite: 1]
            {
                float ly = y0 + y * tamCelda;
                cuadros.DibujarLinea(colorCuadricula, x0, ly, x0 + ancho, ly, 1f);
            }

            Vector4 bordeOscuro = new(0f, 0f, 0f, 0.35f);
            float grosorBisel = MathF.Max(1f, tamCelda * 0.06f);

            for (int x = 0; x < Columnas; x++) // Renderiza cada bloque fijo almacenado en la matriz de celdas[cite: 1]
            {
                for (int y = 0; y < Filas; y++)
                {
                    int valor = _celdas[x, y];
                    if (valor == 0) continue;

                    Vector4 color = PiezaColor(valor);
                    float px = x0 + x * tamCelda;
                    float py = y0 + y * tamCelda;
                    cuadros.DibujarRectangulo(color, px, py, px + tamCelda, py + tamCelda);
                    cuadros.DibujarBordeRectangulo(bordeOscuro, px, py, px + tamCelda, py + tamCelda, grosorBisel);
                }
            }
        }

        private static Vector4 PiezaColor(int valorCelda) // Traduce el valor numérico de la celda al color del tetrominó[cite: 1]
        {
            TipoPieza tipo = (TipoPieza)(valorCelda - 1);
            return (Vector4)new Pieza(tipo).Color;
        }
    }

}


