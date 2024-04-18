﻿// Andrea Aparicio López.
// José Tomás Gómez Becerra.

using System;

namespace FP2P2
{
  internal class Program
  {
    static void Main(string[] args)
    {
      

      // Nivel inicial.
      int num = 0;
      bool capturado = false;

      while (num < 10)
      {
        string file = $"levels/level0{num}.dat";
        Tablero t = new Tablero(file);
        t.Render();

        int lap = 200; // retardo para bucle ppal
        char c = ' ';
        bool pausa = false;

        while (!capturado && !t.FinNivel())
        {
					// input de usuario
					LeeInput(ref c);
          if (!pausa && c == 'p')
          {
            pausa = true;
            c = ' ';
          }
          else if (pausa && c == 'p')
          {
            pausa = false;
            c = ' ';
          }
          if (!pausa)
          {
						// procesamiento del input
						if (c != ' ' && t.CambiaDir(c)) c = ' ';

						t.MuevePacman();
						capturado = t.Captura();

						if (!capturado)
						{
							// IA de los fantasmas
							t.MueveFantasmas(ref t.lapFantasmas);
							capturado = t.Captura();
						}

						// rederizado
						t.Render();

						// retardo
						System.Threading.Thread.Sleep(lap);
					}
        }

        Console.Clear();
        if (capturado)
        {
          Console.Write("¡Te han comido!");
        }
        else
        {
          Console.Write("¡Victoria!");
          num++;
        }
        capturado = false;
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
          case "P": dir = 'p'; break; // Pausa.
          case "X": dir = 'x'; break; // Salir.
        }
      }
      while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
    }

  }
}