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

		SetCoor cs;


        #region 1.Lectura de nivel y renderizado
        public Tablero(string file)
		{
			// Para la comprobacion de la existencia del archivo
			if (File.Exists(file))
			{
				LeeNivel(file, out int[,] tableroNumeros);
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

			string s = archivo.ReadLine().Replace(" ", ""); // Lee la linea y elimina los espacios en blanco.
			numCols = s.Length; // Saca la cantidad de columnas.  
			numFils = 1; // Empezamos a contar desde 1.

			while (!archivo.EndOfStream)
			{
				if(archivo.ReadLine().Replace(" ", "") != "")
					numFils++;
            }
			archivo.Close();
		}

		private void LeeNivel(string file, out int[,] tableroNumeros)
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

				for (int j = 0; j < tableroNumeros.GetLength(1); j++)
				{
					// Parsea a int y rellena.
					tableroNumeros[i, j] = int.Parse(v[j]);
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
							pers[1].ini = new Coor(i, j);
							pers[1].pos = pers[1].ini;
							pers[1].dir = new Coor(1, 0);
							break;
						case 6:
							cas[i, j] = Casilla.Libre;
							pers[2].ini = new Coor(i, j);
							pers[2].pos = pers[2].ini;
							pers[2].dir = new Coor(1, 0);
							break;
						case 7:
							cas[i, j] = Casilla.Libre;
							pers[3].ini = new Coor(i, j);
							pers[3].pos = pers[3].ini;
							pers[3].dir = new Coor(1, 0);
							break;
						case 8:
							cas[i, j] = Casilla.Libre;
							pers[4].ini = new Coor(i, j);
							pers[4].pos = pers[4].ini;
							pers[4].dir = new Coor(1, 0);
							break;
						case 9:
							cas[i, j] = Casilla.Libre;
							pers[0].ini = new Coor(i, j);
							pers[0].pos = pers[0].ini;
							pers[0].dir = new Coor(0, 1);
							break;
					}
				}
			}
		}
		#endregion

		public void Render() // Hay que conseguir que no parpadee la consola.
		{
			Console.Clear();
			Console.ResetColor();

            RenderTablero();

			RenderPersonajes();

			if (DEBUG) RenderDebug();
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
            Console.SetCursorPosition(pers[0].pos.Y * 2, pers[0].pos.X);
            Console.BackgroundColor = colors[0];
            Console.ForegroundColor = ConsoleColor.White;
            if (pers[0].dir.X == 1 && pers[0].dir.Y == 0) { Console.Write("VV"); }
            else if (pers[0].dir.X == -1 && pers[0].dir.Y == 0) { Console.Write("^^"); }
            else if (pers[0].dir.X == 0 && pers[0].dir.Y == 1) { Console.Write(">>"); }
            else if (pers[0].dir.X == 0 && pers[0].dir.Y == -1) { Console.Write("<<"); }

            // Fantasma rojo.
            Console.SetCursorPosition(pers[1].pos.Y * 2, pers[1].pos.X);
            Console.BackgroundColor = colors[1];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");

            // Fantasma magenta.
            Console.SetCursorPosition(pers[2].pos.Y * 2, pers[2].pos.X);
            Console.BackgroundColor = colors[2];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");

            // Fantasma cyan.
            Console.SetCursorPosition(pers[3].pos.Y * 2, pers[3].pos.X);
            Console.BackgroundColor = colors[3];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");

            // Fantasma azul.
            Console.SetCursorPosition(pers[4].pos.Y * 2, pers[4].pos.X);
            Console.BackgroundColor = colors[4];
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ºº");
        }

		private void RenderDebug()
		{
            Console.SetCursorPosition(0, cas.GetLength(0) + 2);
            Console.ResetColor();

            // Quizá en el debug luego haya que incluir cosas tipo Fantasma rojo, fantasma azul en lugar de f1.
            Console.Write($"Pacman: pos{pers[0].pos.ToString()} dir{pers[0].dir.ToString()}" +
                $"\nFantasmas:" +
                $"\n-F1: pos{pers[1].pos.ToString()} dir{pers[1].dir.ToString()}" +
                $"\n-F2: pos{pers[2].pos.ToString()} dir{pers[2].dir.ToString()}" +
                $"\n-F3: pos{pers[3].pos.ToString()} dir{pers[3].dir.ToString()}" +
                $"\n-F4: pos{pers[4].pos.ToString()} dir{pers[4].dir.ToString()}");
            Console.WriteLine();            
        }
        #endregion
        #endregion

        #region 2.Movimiento de Pacman
        // Calcula la siguiente posición en la dirección de movimiento. Devuelve true si puede moverse, false si hay un muro
        private bool Siguiente(Coor pos, Coor dir, out Coor newPos)
		{

			newPos = new Coor(pos.X + dir.X, pos.Y + dir.Y);
            // newPos = pos + dir

            // Si en la newPos no hay muro...
            bool avanza = false;
            if (cas[newPos.X, newPos.Y] != Casilla.Muro && cas[newPos.X, newPos.Y] != Casilla.MuroCelda) avanza = true;

            // Si el personaje escapa por un borde sale por otro. NO FUNCIONA ESTA PARTE. ¿CÓMO SE HARÍA PARA QUE SALGA POR EL OTRO LADO SI HAY PARTES NO CONECTADAS DIRECTAMENTE?
            if (newPos.X < 0) newPos.X = cas.GetLength(1) - 1;
            else if (newPos.X > cas.GetLength(1)) newPos.X = 0;
            else if (newPos.Y < 0) newPos.Y = cas.GetLength(0) - 1;
            else if (newPos.Y > cas.GetLength(1)) newPos.Y = 0;

            

            return avanza;
		}

		public void MuevePacman() // El método del movimiento va bien (supongo).
		{
			if(Siguiente(pers[0].pos, pers[0].dir, out Coor newPos)) 
			{
				// Se mueve Pacman
				pers[0].pos = newPos;

				if (cas[newPos.X, newPos.Y] == Casilla.Vitamina || cas[newPos.X, newPos.Y] == Casilla.Comida)
				{
					numComida--;
					cas[newPos.X, newPos.Y] = Casilla.Libre;

                }
			}
        }

		public bool CambiaDir(char c) // Andrea cariño mio amor mio luego te explico la representacion de la consola como va que yo la entiendo mejor cambiada.
		{
			bool dirCambiada = false;
			Coor newPos;
			switch (c)
			{
				case 'l':
					Coor l = new Coor(0, -1);
                    if (Siguiente(pers[0].pos, l, out newPos))
					{
                        pers[0].dir = l;
						dirCambiada = true;
					}
					break;

				case 'r':
					Coor r = new Coor(0, 1);
                    if (Siguiente(pers[0].pos, r, out newPos))
                    {
                        pers[0].dir = r;
                        dirCambiada = true;
                    }
                    break;

				case 'u':
					Coor u = new Coor(-1, 0);
                    if (Siguiente(pers[0].pos, u, out newPos))
                    {
						pers[0].dir = u;
                        dirCambiada = true;
                    }
                    break;

                case 'd':
					Coor d = new Coor(1, 0);
                    if (Siguiente(pers[0].pos, d, out newPos))
                    {
						pers[0].dir = d;
                        dirCambiada = true;
                    }
                    break;
            }
			return dirCambiada;
		}


		#endregion

		#region 3.Movimiento de los fantasmas
		private bool HayFantasma(Coor c)
		{
			bool hayFantasma = false;

			for (int i = 1; i < pers.Length; i++)
			{
				if (pers[i].pos == c) { hayFantasma = true; }
			}

			return hayFantasma;
		}

		private int PosiblesDirs(int fant, out SetCoor cs) 
		{
			cs = new SetCoor();
			
			// Nueva posición y direcciones.
			Coor newPos;

			// Van en orden.
            Coor abajo = new Coor(1, 0);
            Coor derecha = new Coor(0, 1);
            Coor arriba = new Coor(-1, 0);
			Coor izquierda = new Coor (0, -1);

            SetCoor direcciones = new SetCoor();
            direcciones.Add(abajo);
            direcciones.Add(derecha);
            direcciones.Add(arriba);
            direcciones.Add(izquierda);

            // Creo que la lógica está mal por el "&& !HayFantasma(newPos)", no sé si habrá que meterlo en un if dentro, pero sería repetir código.
            //Si la posición siguiente(método Siguiente descrito arriba) en esa dirección está libre(no hay muro) y no contiene fantasma, insertamos dicha posición en cs.
            // Hay mucha repetición de código.
            if (Siguiente(pers[fant].pos, abajo , out newPos) && !HayFantasma(newPos))
			{
				cs.Add(newPos);
			}
			else if (Siguiente(pers[fant].pos, derecha, out newPos) && !HayFantasma(newPos))
			{
                cs.Add(newPos);
            }
			else if (Siguiente(pers[fant].pos, arriba, out newPos) && !HayFantasma(newPos))
			{
                cs.Add(newPos);
            }
			else if (Siguiente(pers[fant].pos, izquierda, out newPos) && !HayFantasma(newPos))
			{
                cs.Add(newPos);
            }

			/*for(int i = 0;  i <= direcciones.Size(); i++)
			{
				if (Siguiente(pers[fant].pos, direcciones, out newPos) && !HayFantasma(newPos)) { cs.Add(newPos); }
			}*/
		}
        #endregion
    }
}
