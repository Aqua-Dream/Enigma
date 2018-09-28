using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace Enigma
{
    class MyButton : Button
    {
        protected override void OnClick()
        {
            try
            {
                if (!File.Exists("Resources/Click.wav"))
                    throw new Exception();
                var player = new MediaPlayer();
                player.Open(new Uri("Resources/Click.wav", UriKind.Relative));
                player.Play();
            }
            catch { }
            base.OnClick();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            try
            {
                if (!File.Exists("Resources/Hover.wav"))
                    throw new Exception();
                var player = new MediaPlayer();
                player.Open(new Uri("Resources/Hover.wav", UriKind.Relative));
                player.Play();
            }
            catch { }
            base.OnMouseEnter(e);
        }
    }
}
