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
			// Para la comprobacion de la existencia del archivo File.Exist creo...
			if (File.Exists(file))
			{
				int[,] tableroNumeros;
				LeeNivel(file, out tableroNumeros);
				InicializaCasyPers(out cas, out pers, tableroNumeros);
				lapFantasmas = lapCarcelFantasmas;

				if (DEBUG) rnd = new Random(100);
				else rnd = new Random();

			}
			else
			{
				throw new Exception("No existe el nivel seleccionado.");
			}
		}

		#region Submétodos Constructora
		private void SacaSize(string file, out int numFils, out int numCols)
		{
			StreamReader archivo = new StreamReader(file);

			numFils = 1; // Empezamos a contar desde 1.
			numCols = 0;// Inicialmente el tamaño es 0.

			string leeCols = archivo.ReadLine().Replace(" ", ""); // Lee la linea y elimina los espacios en blanco.
			numCols = leeCols.Length; // Saca la cantidad de columnas.  

			while (!archivo.EndOfStream)
			{
				numFils++;
				archivo.ReadLine();
			}

			archivo.Close();
		}

		private void LeeNivel(string file, out int[,] tableroNumeros) // Falla en tableroNumeros[i, j] = myInts[j] para los niveles [2 - 9]. Denisa dijo que era por el final del archivo.
        {
			// Habrá dos lecturas:
			// 1. Para determinar el tamaño de la matriz.
			int numFils;
			int numCols;
			SacaSize(file, out numFils, out numCols);

			tableroNumeros = new int[numFils, numCols];

			// Abrimos flujo de archivo y se leen los niveles de levels/level0X.dat. ¡¡¡Ojo, meter luego ($"levels/{file}")!!!
			StreamReader archivo = new StreamReader(file);

			// 2. Para ir rellenando la matriz. 
			for (int i = 0; i < tableroNumeros.GetLength(0); i++)
			{
				// Lee la fila actual.
				string s = archivo.ReadLine();

				// En el array v va almacenando el contenido de la fila actual pero sin los espacios. Se guardan los números.
				string[] v = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);

				// String -> Int.
				int[] myInts = Array.ConvertAll(v, int.Parse);

				for (int j = 0; j < tableroNumeros.GetLength(1); j++)
				{
					// Rellena.
					tableroNumeros[i, j] = myInts[j];
				}
			}

			// Cerramos flujo.
			archivo.Close();
		}

		private void InicializaCasyPers(out Casilla[,] cas, out Personaje[] pers, int[,] tableroNumeros)
		{
			cas = new Casilla[tableroNumeros.GetLength(0), tableroNumeros.GetLength(1)];
			pers = new Personaje[5];

			for (int i = 0; i < tableroNumeros.GetLength(0); i++)
			{
				for (int j = 0; j < tableroNumeros.GetLength(1); j++)
				{
					switch (tableroNumeros[i, j])
					{
						case 0:
							cas[i, j] = Casilla.Libre; break;
						case 1:
							cas[i, j] = Casilla.Muro; break;
						case 2:
							cas[i, j] = Casilla.Comida; break;
						case 3:
							cas[i, j] = Casilla.Vitamina; break;
						case 4:
							cas[i, j] = Casilla.MuroCelda; break;
						case 5:
							cas[i, j] = Casilla.Libre;
							pers[1].ini = new Coor(i, 2*j);
							pers[1].pos = pers[1].ini;
							pers[1].dir = new Coor(1, 0);
							break;
						case 6:
							cas[i, j] = Casilla.Libre;
							pers[2].ini = new Coor(i, 2 * j);
							pers[2].pos = pers[2].ini;
							pers[2].dir = new Coor(1, 0);
							break;
						case 7:
							cas[i, j] = Casilla.Libre;
							pers[3].ini = new Coor(i, 2 * j);
							pers[3].pos = pers[3].ini;
							pers[3].dir = new Coor(1, 0);
							break;
						case 8:
							cas[i, j] = Casilla.Libre;
							pers[4].ini = new Coor(i, 2 * j);
							pers[4].pos = pers[4].ini;
							pers[4].dir = new Coor(1, 0);
							break;
						case 9:
							cas[i, j] = Casilla.Libre;
							pers[0].ini = new Coor(i, 2	* j);
							pers[0].pos = pers[0].ini;
							pers[0].dir = new Coor(0, 1);
							break;
					}


				}

			}
		}
		#endregion

		public void Render()
		{
			RenderTablero();

			RenderPersonajes();

            RenderDebug();
		}

		#region Submétodos Render
		private void RenderTablero()
		{
            for (int i = 0; i < cas.GetLength(0); i++)
            {
                for (int j = 0; j < cas.GetLength(1); j++)
                {
                    switch (cas[i, j])
                    {
                        case Casilla.Libre:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("  ");
                            break; // Luego mirar fantasmas y movidas.
                        case Casilla.Muro:
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.Write("  ");
                            break;
                        case Casilla.Comida:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("··");
                            break;
                        case Casilla.Vitamina:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("**");
                            break;
                        case Casilla.MuroCelda:
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.Write("  ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

		private void RenderPersonajes()
		{
            // ¡¡¡ OJO, CONSOLESETCURSORPOSITION ESTÁ INVERTIDO!!!

            // Pacman.
            Console.SetCursorPosition(pers[0].pos.Y, pers[0].pos.X);
            Console.BackgroundColor = colors[0];
            Console.ForegroundColor = ConsoleColor.White;
            if (pers[0].dir.X == 1 && pers[0].dir.Y == 0) { Console.Write("VV"); }
            else if (pers[0].dir.X == -1 && pers[0].dir.Y == 0) { Console.Write("^^"); }
            else if (pers[0].dir.X == 0 && pers[0].dir.Y == 1) { Console.Write(">>"); }
            else if (pers[0].dir.X == 0 && pers[0].dir.Y == -1) { Console.Write("<<"); }

            // Fantasma rojo.
            Console.SetCursorPosition(pers[1].pos.Y, pers[1].pos.X);
            Console.BackgroundColor = colors[1];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");

            // Fantasma magenta.
            Console.SetCursorPosition(pers[2].pos.Y, pers[2].pos.X);
            Console.BackgroundColor = colors[2];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");

            // Fantasma cyan.
            Console.SetCursorPosition(pers[3].pos.Y, pers[3].pos.X);
            Console.BackgroundColor = colors[3];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");

            // Fantasma azul.
            Console.SetCursorPosition(pers[4].pos.Y, pers[4].pos.X);
            Console.BackgroundColor = colors[4];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");
        }

		private void RenderDebug()
		{
            Console.SetCursorPosition(0, cas.GetLength(0));
            Console.ResetColor();

            if (DEBUG)
            {
                // Quizá en el debug luego haya que incluir cosas tipo Fantasma rojo, fantasma azul en lugar de f1.
                Console.WriteLine();
                Console.Write($"Pacman: pos{pers[0].pos.ToString()} dir{pers[0].dir.ToString()}" +
                    $"\nFantasmas:" +
                    $"\n-F1: pos{pers[1].pos.ToString()} dir{pers[1].dir.ToString()}" +
                    $"\n-F2: pos{pers[2].pos.ToString()} dir{pers[2].dir.ToString()}" +
                    $"\n-F3: pos{pers[3].pos.ToString()} dir{pers[3].dir.ToString()}" +
                    $"\n-F4: pos{pers[4].pos.ToString()} dir{pers[4].dir.ToString()}");
                Console.WriteLine();
            }
        }
        #endregion

        private bool Siguiente(ref Coor pos, ref Coor dir, Coor newPos)
		{
			bool avanza = false;
			newPos = new Coor(pos.X + dir.X, pos.Y + dir.Y);

			// Hacer lo de que el personaje si escapa por un borde salga por otro.

			// Si en la newPos no hay muro...
			if(cas[newPos.X, newPos.Y] != Casilla.Muro)
			{
				// Se mueve el pacman (mirar lo de MuevePacman).
				pos.X = newPos.X;
				pos.Y = newPos.Y;
				avanza = true;

				if (cas[newPos.X, newPos.Y] == Casilla.Vitamina || cas[newPos.X, newPos.Y] == Casilla.Comida) { numComida--; }
				
				// Lo de que aparezca por otro lado.
				if (pos.X == 0 && dir.X == -1)
				{
					newPos.X = cas.GetLength(0);
				}
				else if (pos.Y == 0 && dir.Y == -1)
				{
					newPos.Y = cas.GetLength(1);
				}
				else if (pos.X == cas.GetLength(0) && dir.X == 1)
				{
					newPos.X = 0;
				}
				else if (pos.Y == cas.GetLength(1) && dir.Y == 1)
				{
					newPos.Y = 0;
				}
			}
			else
			{
				// No cambia la posición.
				pos.X = pos.X;
				pos.Y = pos.Y;
				avanza = false;
			}

			// Dice si ha podido avanzar o no.
			return avanza;
		}

		private void MuevePacman()
		{
			// He hecho una "METAMORFOSIS" y he mezclado MuevePacman con Siguiente porque no entiendo muy bien el enunciado.
		}

		private bool CambiaDir(char c)
		{
			Coor newPos = new Coor();

			if(Siguiente(ref pers[0].pos, ref pers[0].dir, newPos))
			{
                switch (c)
                {
                    case 'l':
                        pers[0].dir.X = 0;
                        pers[0].dir.Y = -1;
                        break;
                    case 'r':
                        pers[0].dir.X = 0;
                        pers[0].dir.Y = 1;
                        break;
                    case 'u':
                        pers[0].dir.X = -1;
                        pers[0].dir.Y = 0;
                        break;
                    case 'd':
                        pers[0].dir.X = 1;
                        pers[0].dir.Y = 0;
                        break;
                }
            }
		}

        static void LeeInput(ref char dir)
        {
            if (Console.KeyAvailable)
            {
                string tecla = Console.ReadKey(true).Key.ToString();
                switch (tecla)
                {
                    case "LeftArrow": dir = 'l'; break;
                    case "UpArrow": dir = 'u'; break;
                    case "RightArrow": dir = 'r'; break;
                    case "DownArrow": dir = 'd'; break;
                }
            }
            while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
        }


    }
}
