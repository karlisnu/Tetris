using Tetris2D.Graficos;
using Tetris2D.UI;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Tetris2D.Pantallas
{
    public abstract class Pantalla
    {
        // Herramientas de dibujo compartidas por todas las pantallas.
        protected readonly GestorShader Shaders;
        protected readonly DibujadorCuadros Cuadros;
        protected readonly RenderizadorTexto Texto;

        public event Action<string>? JugarSolicitado;

        protected Pantalla(GestorShader shaders, DibujadorCuadros cuadros, RenderizadorTexto texto)
        {
            Shaders = shaders;
            Cuadros = cuadros;
            Texto = texto;
        }

        public virtual void Cargar() { }
        public virtual void Actualizar(float dt, Vector2 raton) { }
        public abstract void Renderizar(float ancho, float alto);
        public virtual void AlClick(Vector2 posicion, MouseButton boton) { }
        public virtual void AlTecla(Keys tecla) { }
        public virtual void AlTexto(string caracter) { }

        protected void SolicitarJuego(string nombreJugador)
        {
            JugarSolicitado?.Invoke(nombreJugador);
        }
    }
}