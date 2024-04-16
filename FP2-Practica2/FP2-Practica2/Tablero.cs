using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
		const int lapCarcelFantasmas = 30; // retardo para quitar el muro a los fantasmas
		public int lapFantasmas; // tiempo restante para quitar el muro

		int numComida; // numero de casillas restantes con comida o vitamina
		Random rnd; // generador de aleatorios
								// flag para mensajes de depuracion en consola
		private bool DEBUG = true;

		SetCoor cs;


		#region 1.Lectura de nivel y renderizado
		public Tablero(string file) // [DONE] Constructora: método especial para inicializar un objeto (tablero) y asignarle valores iniciales a sus instancia.
        {
			// Si el archivo existe...
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
		private void SacaSize(string file, out int numFils, out int numCols) // [DONE] Método auxiliar que saca filas y columnas.
		{
			StreamReader archivo = new StreamReader(file);

			// ---COLUMNAS---.
			string s = archivo.ReadLine().Replace(" ", ""); // Lee la linea y elimina los espacios en blanco.
			numCols = s.Length; // Saca la cantidad de columnas.  

			// ---FILAS---.
			numFils = 1; // Empezamos a contar desde 1.
            // Mientras no acabe el archivo.
            while (!archivo.EndOfStream)
            {
                // Ignora las filas vacías y las cuenta.
                if (archivo.ReadLine().Replace(" ", "") != "")
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
							cas[i, j] = Casilla.Comida;
							numComida++;
							break;
						case 3:
							cas[i, j] = Casilla.Vitamina;
							numComida++;
							break;
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
			else if (pers[0].dir.X == 0 && pers[0].dir.Y == 0) { Console.Write("00"); }

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
			newPos = pos + dir;

			// Si el personaje escapa por un borde sale por otro
			if (newPos.X < 0) newPos.X = cas.GetLength(0) - 1;
			else if (newPos.X > cas.GetLength(0) - 1) newPos.X = 0;
			else if (newPos.Y < 0) newPos.Y = cas.GetLength(1) - 1;
			else if (newPos.Y > cas.GetLength(1) - 1) newPos.Y = 0;

			bool avanza = false;
			// Si en la newPos no hay muro...
			if (cas[newPos.X, newPos.Y] != Casilla.Muro && cas[newPos.X, newPos.Y] != Casilla.MuroCelda) avanza = true;

			return avanza;
		}

		public void MuevePacman() // El método del movimiento va bien (supongo).
		{
			if (Siguiente(pers[0].pos, pers[0].dir, out Coor newPos))
			{
				// Se mueve Pacman
				pers[0].pos = newPos;

				if (cas[newPos.X, newPos.Y] == Casilla.Comida)
				{
					numComida--;
					cas[newPos.X, newPos.Y] = Casilla.Libre;
				}
				else if (cas[newPos.X, newPos.Y] == Casilla.Vitamina)
				{
                    numComida--;
                    cas[newPos.X, newPos.Y] = Casilla.Libre;
					ReseteaFantasmas();
                }

			}
		}

		#region Submétodos MuevePacman
		private void ReseteaFantasmas()
		{
			for(int i = 1; i< pers.Length; i++)
			{
				pers[i].pos = pers[i].ini;
			}
			lapFantasmas = lapCarcelFantasmas;
		}
		#endregion

		public bool CambiaDir(char c) 
		{
			bool dirCambiada = false;
			Coor newPos;
			switch (c)
			{
				case 'l': // <<
					Coor l = new Coor(0, -1);
					if (Siguiente(pers[0].pos, l, out newPos))
					{
						pers[0].dir = l;
						dirCambiada = true;
					}
					break;

				case 'r': // >>
					Coor r = new Coor(0, 1);
					if (Siguiente(pers[0].pos, r, out newPos))
					{
						pers[0].dir = r;
						dirCambiada = true;
					}
					break;

				case 'u': // ^^
					Coor u = new Coor(-1, 0);
					if (Siguiente(pers[0].pos, u, out newPos))
					{
						pers[0].dir = u;
						dirCambiada = true;
					}
					break;

				case 'd': // VV
                    Coor d = new Coor(1, 0);
					if (Siguiente(pers[0].pos, d, out newPos))
					{
						pers[0].dir = d;
						dirCambiada = true;
					}
					break;

				case 'p': // pausa
					Coor p = new Coor(0, 0);
					if (Siguiente(pers[0].pos, p, out newPos))
					{
						pers[0].dir = p;
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

			int i = 1;
			while (!hayFantasma && i < pers.Length)
			{
				hayFantasma = pers[i].pos == c;
				i++;
			}

			return hayFantasma;
		}

		private int PosiblesDirs(int fant, out SetCoor cs)
		{
			cs = new SetCoor(4);

			// Nueva posición y direcciones.
			Coor newPos;

			// Van en orden.
			Coor abajo = new Coor(1, 0);
			Coor derecha = new Coor(0, 1);
			Coor arriba = new Coor(-1, 0);
			Coor izquierda = new Coor(0, -1);

			SetCoor direcciones = new SetCoor(4);
			direcciones.Add(abajo);
			direcciones.Add(derecha);
			direcciones.Add(arriba);
			direcciones.Add(izquierda);

			//Si la posición siguiente en esa dirección está libre(no hay muro) y no contiene fantasma, insertamos dicha DIRECCIÓN en cs.
			for (int i = 0; i < direcciones.Size(); i++)
			{
				if (Siguiente(pers[fant].pos, direcciones.Coors[i], out newPos) && !HayFantasma(newPos)) cs.Add(direcciones.Coors[i]);
			}

			return cs.Size();
		}

		private void SeleccionaDir(int fant)
		{
			int numDirs = PosiblesDirs(fant, out SetCoor cs);

			if (numDirs > 1)
			{
				Coor dirOp = new Coor(pers[fant].dir.X * (-1), pers[fant].dir.Y * (-1));
				cs.Remove(dirOp);

				int n = rnd.Next(0, cs.Size());

				for (int i = 0; i < n; i++)
				{
					cs.PopElem();
				}
			}
			if(cs.Size() > 0) pers[fant].dir = cs.Coors[cs.Size() - 1];
		}

		private void EliminaMuroFantasmas()
		{
			for (int i = 0; i < cas.GetLength(0); i++)
			{
				for (int j = 0; j < cas.GetLength(1); j++)
				{
					if (cas[i, j] == Casilla.MuroCelda) cas[i, j] = Casilla.Libre;
				}
			}
		}

		public void MueveFantasmas(ref int lap) // Lo que se me ocurre para parar a los fantasmas es meterle la sobrecarga de leeinput y pararlo cuando le des a la p como cuando hago con el pacman.
		{
			if(lap <= 0)
			{
				for (int i = 1; i < pers.Length; i++)
				{
					SeleccionaDir(i);

					if (Siguiente(pers[i].pos, pers[i].dir, out Coor newPos)) pers[i].pos = newPos;
				}
			}
			else
			{
				lap--;
				if(lap == 0) EliminaMuroFantasmas();
			}
		}
        #endregion

        #region 4.Colisiones y final de nivel
        public bool Captura()
		{
			bool capturado = false;
			int i = 1;
			while(i < pers.Length && !capturado)
			{
				capturado = pers[0].pos == pers[i].pos;
				i++;
			}
			return capturado;
		}

		public bool FinNivel()
		{
			return numComida == 0;
		}
		#endregion
	}
}
