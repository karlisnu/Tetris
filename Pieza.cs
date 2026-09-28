using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Tetris2D.UI;

namespace Tetris2D
{
    public enum TipoPieza
    {
        I, J, L, O, S,T, Z
    }

    public class Pieza
    {
        public IReadOnlyList<(int x, int y)> Bloques { get; private set; }
        public TipoPieza Tipo { get; }
        public System.Numerics.Vector4 Color { get; }

        // Define las formas base de cada tipo de pieza en coordenadas relativas
        private static readonly IReadOnlyDictionary<TipoPieza, (int x, int y)[]> FormasBase = new Dictionary<TipoPieza, (int x, int y)[]>
        {
            { TipoPieza.I, new[] { (0, 1), (1, 1), (2, 1), (3, 1) } },
            { TipoPieza.J, new[] { (0, 0), (0, 1), (1, 1), (2, 1) } },
            { TipoPieza.L, new[] { (0, 1), (1, 1), (2, 1), (2, 0) } },
            { TipoPieza.O, new[] { (0, 0), (0, 1), (1, 0), (1, 1) } },
            { TipoPieza.S, new[] { (0, 1), (1, 1), (1, 0), (2, 0) } },
            { TipoPieza.T, new[] { (0, 1), (1, 1), (2, 1), (1, 0) } },
            { TipoPieza.Z, new[] { (0, 0), (1, 0), (1, 1), (2, 1) } }
        };

        // Define los colores asociados a cada tipo de pieza usando la paleta de colores del tema arcade
        public static readonly IReadOnlyDictionary<TipoPieza, OpenTK.Mathematics.Vector4> Colores = new Dictionary<TipoPieza, OpenTK.Mathematics.Vector4>
        {
            [TipoPieza.I] = TemaArcade.Cian,
            [TipoPieza.J] = TemaArcade.Amarillo,
            [TipoPieza.L] = TemaArcade.Magenta,
            [TipoPieza.O] = TemaArcade.Verde,
            [TipoPieza.S] = TemaArcade.Rojo,
            [TipoPieza.Z] = TemaArcade.Azul,
            [TipoPieza.T] = TemaArcade.Naranja,
        };

        // Constructor que inicializa una pieza con su tipo, forma y color correspondiente
        public Pieza(TipoPieza tipo)
        {
            Tipo = tipo;
            Bloques = FormasBase[tipo];
            Color = (System.Numerics.Vector4)Colores[tipo];
        }

        // Método que devuelve la rotación de los bloques de la pieza en sentido horario o antihorario
        public IReadOnlyList<(int x, int y)> ObtenerRotacion(bool horario)
        {
            var resultado = new List<(int x, int y)>(Bloques.Count);
            foreach ((int x, int y) in Bloques)
            {
                resultado.Add(horario ? (y, -x) : (-y, x));
            }
            return resultado;
        }

        // Método que aplica la rotación a los bloques de la pieza
        public void AplicarRotacion(bool horario)
        {
            Bloques = ObtenerRotacion(horario);
        }

        // Método que genera una pieza aleatoria utilizando un generador de números aleatorios
        public static Pieza Aleatoria(Random rnd)
        {
            TipoPieza tipo = (TipoPieza)rnd.Next(7);
            return new Pieza(tipo);
        }
    }
}
