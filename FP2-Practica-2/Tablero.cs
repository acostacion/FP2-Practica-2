using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP2P2
{
    class Tablero
    {
        // contenido de las casillas
        enum Casilla { Libre, Muro, Comida, Vitamina, MuroCelda };
        // matriz de casillas (tablero)
        Casilla[,] cas;
        // representacion de los personajes (pacman y fantasmas)
        struct Personaje
        {
            public Coor pos, dir, // posicion y direccion actual
            ini; // posicion inicial (para fantasmas)
        }
        // vector de personajes, 0 es pacman y el resto fantasmas
        Personaje[] pers;
        // colores para los personajes
        ConsoleColor[] colors = {ConsoleColor.DarkYellow, 
                                 ConsoleColor.Red,
                                 ConsoleColor.Magenta, 
                                 ConsoleColor.Cyan, 
                                 ConsoleColor.DarkBlue };
        const int lapCarcelFantasmas = 3000; // retardo para quitar el muro a los fantasmas
        int lapFantasmas; // tiempo restante para quitar el muro
        int numComida; // numero de casillas restantes con comida o vitamina
        Random rnd; // generador de aleatorios
                    // flag para mensajes de depuracion en consola
        private bool DEBUG = true;
    }
}
