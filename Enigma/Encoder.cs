using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Threading;

namespace Enigma
{
    class Encoder : BindableObject
    {
        
        private PlugBoard plugBoard;

        public Encoder(PlugBoard p)
        {
            plugBoard = p;
        }

        private string ringValue = "AAA";
        public string RingValue
        {
            get { return ringValue; }
            set
            {
                string newValue = "";
                string oldValue = value.ToUpper();
                int count = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i < oldValue.Length && oldValue[i] <= 'Z' && oldValue[i] >= 'A')
                        newValue += oldValue[i];
                    else
                    {
                        newValue += 'A';
                        count++;
                    }
                }
                SetProperty(ref ringValue, newValue);
            }
        }

        private bool refresh = true;
        private string keyValue = "AAA";
        public string KeyValue
        {
            get { return keyValue; }
            set
            {
                string newValue = "";
                string oldValue = value.ToUpper();
                int count = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i < oldValue.Length && oldValue[i] <= 'Z' && oldValue[i] >= 'A')
                        newValue += oldValue[i];
                    else
                    {
                        newValue += 'A';
                        count++;
                    }
                }
                if (refresh)
                    SetProperty(ref keyValue, newValue);
                else
                    keyValue = newValue;
            }
        }

        private string rotorValue = "321";
        public string RotorValue
        {
            get { return rotorValue; }
            set
            {
                string newValue = "";
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] <= '5' && value[i] >= '1' && newValue.Length < 3)
                    {
                        bool flag = false;
                        for (int j = 0; j < newValue.Length; j++)
                        {
                            if (newValue[j] == value[i])
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            newValue += value[i];
                        }
                    }
                }
                var choices = new List<char>(new char[] { '1', '2', '3', '4', '5' });
                foreach (var i in newValue)
                {
                    choices.Remove(i);
                }
                int n = newValue.Length;
                for (int i = 0; i < 3 - n; i++)
                    newValue += choices[i];

                SetProperty(ref rotorValue, newValue);
            }
        }

        private string plainText = "";
        public string PlainText
        {
            get { return plainText; }
            set
            {
                string newValue = "";
                string oldValue = value.ToUpper();
                for (int i = 0; i < oldValue.Length; i++)
                {
                    if (oldValue[i] <= 'Z' && oldValue[i] >= 'A')
                        newValue += oldValue[i];
                }
                SetProperty(ref plainText, newValue);
            }
        }

        private string cipherText = "";
        public string CipherText
        {
            get { return cipherText; }
            set
            {
                string newValue = "";
                string oldValue = value.ToUpper();
                for (int i = 0; i < oldValue.Length; i++)
                {
                    if (oldValue[i] <= 'Z' && oldValue[i] >= 'A')
                        newValue += oldValue[i];
                }
                SetProperty(ref cipherText, newValue);
            }
        }

        private string[] rotorTable = { "EKMFLGDQVZNTOWYHXUSPAIBRCJ","AJDKSIRUXBLHWTMCQGZNPYFVOE",
            "BDFHJLCPRTXVZNYEIWGAKMUSQO","ESOVPZJAYQUIRHXLNFTGKDCMWB","VZBRGITYUPSDNHLXAWMJQOFECK" };

        private string step = "QEVJZ";

        private string reflector = "YRUHQSLDPXNGOKMIEBFZCWVJAT";

        private void IncreaseChar(ref char c)
        {
            int n = c;
            n++ ;
            if (n > 'Z')
                n -= 26;
            c = (char)n;
        }

        private void IncreaseKey()
        {
            char[] cs = keyValue.ToCharArray();
            if(cs[2] == step[rotorValue[2]-'1'] || cs[1] == step[rotorValue[1]-'1'])
            {
                if (cs[1] == step[rotorValue[1] - '1'])
                    IncreaseChar(ref cs[0]);
                IncreaseChar(ref cs[1]);
            }
            IncreaseChar(ref cs[2]);
            KeyValue = new string(cs);
            keyValue = new string(cs);
        }

        private char SearchTable(char c, int index)
        {
            string s = rotorTable[index];
            for (int i=0;i<s.Length;i++)
            {
                if (s[i] == c)
                    return (char)(i + 'A');
            }
            return 'A';
        }

        /// <summary>
        /// Define three indexes of the geer as 0,1,2 from left to right.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="index"></param>
        /// <param name="IsForward"></param>
        /// <returns></returns>
        private char GeerMap(char c, int index, bool IsForward)
        {
            c = (char)(c + keyValue[index] - ringValue[index]);
            if (c > 'Z')
                c -= (char)26;
            else if (c < 'A')
                c += (char)26;
            if (IsForward)
                c = rotorTable[rotorValue[index] - '1'][c-'A'];
            else
                c = SearchTable(c, rotorValue[index] - '1');
            c = (char)(c - keyValue[index] + ringValue[index]);
            if (c < 'A')
                c += (char)26;
            else if (c > 'Z')
                c -= (char)26;
            return c;
        }

        private char Encode(char c)
        {
            c = plugBoard.Get(c);
            IncreaseKey();
            for(int i=0;i<3;i++)
                c = GeerMap(c, 2 - i, true);
            c = reflector[c - 'A'];
            for (int i = 0; i < 3; i++)
                c = GeerMap(c, i, false);
            c = plugBoard.Get(c);
            return c;
        }

        private string Encode(string s)
        {
            StringBuilder sb = new StringBuilder(32767);
            refresh = false;
            foreach (char c in s)
                sb.Append(Encode(c));
            refresh = true;
            KeyValue = keyValue;
            return sb.ToString();
        }

        
        public void Encode(bool encode)
        {
            if (encode)
                CipherText = Encode(plainText);
            else
                PlainText = Encode(cipherText);
            KeyValue = keyValue;
        }

        public void BeginEncode(MainWindow m, bool encode)
        {
            string text;
            if (encode)
                text = plainText;
            else
                text = cipherText;
            if (string.IsNullOrEmpty(text))
                return;
            else if(text.Length>99)
            {
                Encode(encode);
                return;
            }
            m.groupBox.IsEnabled = false;
            CipherText = "";
            PlainText = "";
            int sleeptime = text.Length;
            sleeptime = (int)(1000*Math.Sqrt(Math.Log(sleeptime, 2))/sleeptime);
            var thread = new Thread(() =>
            {
                SoundPlayer player = null;
                try
                {
                    player = new SoundPlayer("Resources/Knock.wav");
                    player.Load();
                }
                catch { player = null; }
                int tick = 60;
                if(encode)
                    foreach (char c in text)
                    {
                        tick -= sleeptime;
                        m.Dispatcher.Invoke(()=>
                        {
                            PlainText += c;
                            CipherText += Encode(c);
                        });
                        if (tick < 0 && player!=null)
                        {
                            player.Play();
                            tick = 60;
                        }
                        Thread.Sleep(sleeptime);
                    }
                else
                    foreach (char c in text)
                    {
                        tick -= sleeptime;
                        m.Dispatcher.Invoke(() =>
                        {
                            CipherText += c;
                            PlainText += Encode(c);
                        });
                        if (tick < 0 && player != null)
                        {
                            player.Play();
                            tick = 60;
                        }
                        Thread.Sleep(sleeptime);
                    }
                m.Dispatcher.Invoke(() => { m.groupBox.IsEnabled = true; });
            });
            thread.Start();
        }
    }
}
