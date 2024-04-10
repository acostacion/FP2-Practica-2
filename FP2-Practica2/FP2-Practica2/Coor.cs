using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FP2P2
{
    class Coor
    {
        int x, y; // componentes x y de la coordenada

        public Coor(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }
        public int X
        {
            get => x;
            set => x = value;
        }

        public int Y
        {
            get => y;
            set => y = value;
        }

        public override string ToString() { return $"({x},{y})"; }

        public static Coor Parse(string s)
        {
            int ini, fin; // buscamos índices de "(" y de ")"
            (ini, fin) = (s.IndexOf("("), s.IndexOf(")"));
            // nos quedamos con la subcadena entre medias de ambos
            s = s.Substring(ini + 1, fin - ini - 1);
            // dividimos con la ","
            string[] nums = s.Split(",", StringSplitOptions.RemoveEmptyEntries);
            // parseamos los dos enteros y construimos coordenada
            return new Coor(int.Parse(nums[0]), int.Parse(nums[1]));
        }

        //Método para sumar un vector v a la coordenada: en este caso se modifica la coordenada actual
        public void Translate(Coor v)
        {
            x += v.X;
            y += v.Y;
        }

        // suma de coordenadas
        public static Coor operator +(Coor c1, Coor c2)
        {
            return new Coor(c1.X + c2.X, c1.Y + c2.Y);
        }

        // igualdad y desigualdad de coordenadas.
        public static bool operator ==(Coor c1, Coor c2) { return c1.x == c2.x && c1.y == c2.y; }

        public static bool operator !=(Coor c1, Coor c2) { return c1.x != c2.x || c1.y != c2.y; }
    }
}


