// Andrea Aparicio López.
// José Tomás Gómez Becerra.

namespace FP2P2
{
  internal class Program
  {
    

    static void Main(string[] args)
    {
      string file = "levels/level01.dat";
			Tablero tab = new Tablero(file);
			tab.Render();
		}

  }
}
