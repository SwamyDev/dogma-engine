using System;
using System.Windows.Forms;

namespace PontAnimator
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();

                MainForm form = new MainForm();
                form.Show();

                using (Animator game = new Animator(form.getHandle()))
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
#endif
}