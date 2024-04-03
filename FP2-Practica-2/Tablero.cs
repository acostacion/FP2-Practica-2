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

        public Tablero(string file)
        {
            // Abrimos flujo de archivo y se leen los niveles de levels/level0X.dat. ¡¡¡Ojo, meter luego ($"levels/{file}")!!!
            StreamReader archivo = new StreamReader($"levels/level00.dat");

            /*if(file != "levelblablba...")
            {
                // Código de lectura del archivo.
            }
            else
            {
                throw new Exception("El nivel seleccionado no existe.");
            }*/
            
            string leeCaracteres = "X"; // Condición de parada al encontrar al primer "no-número" (casilla en blanco).
            int[,] matrizNumeros;

            // Habrá dos lecturas:
            // 1. Para determinar el tamaño de la matriz.
            // 2. Para ir rellenando la matriz.

            int tamaño = 0; // Inicialmente no sabemos el tamaño.

            while (leeCaracteres != "")
            {
                tamaño++;
                Console.WriteLine(tamaño);
            }

            // Cerramos flujo.
            archivo.Close();
        }
    }
}
