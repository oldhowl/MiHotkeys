using MiHotkeys.Forms;

namespace MiHotkeys;

public class Program
{
     [STAThread]
     static void Main()
     { 
          
          ApplicationConfiguration.Initialize();
          Application.Run(new MainForm());
     }    
}