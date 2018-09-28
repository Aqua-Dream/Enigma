using System;
using System.Windows;
using System.Windows.Controls;
using System.Text;


namespace Enigma
{
    class PlugBoard
    {
        private int[] map = new int[26];

        private ListBox plugListBox;
        
        public PlugBoard(ListBox l)
        {
            plugListBox = l;
            Clear();
        }

        private bool Set(char c)
        {
            int n = c - 'A';
            int m = map[n];
            if (m != n)
            {
                var result = MessageBox.Show(string.Format
                    ("The map between {0} and {1} exists.\nRemove it?", c, (char)('A' + map[n])),
                     "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return false;
                map[n] = n;
                map[m] = m;
                for(int i=plugListBox.Items.Count-1;i>=0;i--)
                {
                    if (plugListBox.Items[i].ToString().IndexOf(c) != -1)
                        plugListBox.Items.RemoveAt(i);
                }
            }
            return true;
        }

        public char Get(char c)
        {
            int n = c - 'A';
            return (char)(map[n]+'A');
        }

        public void Add(string s1, string s2)
        {
            char c1='A', c2='A';
            int n1, n2;
            string msg=null;
            try
            {
                c1 = Convert.ToChar(s1.ToUpper());
                c2 = Convert.ToChar(s2.ToUpper());
                if (c1 > 'Z'||c2>'Z' || c1 < 'A'||c2<'A')
                    throw new FormatException();
                if (c1 == c2)
                    throw new Exception();
            }
            catch(FormatException)
            {
                msg = "The text in textbox should be a single letter!";
            }
            catch
            {
                msg = "The letters from different textbox can not be the same!";
            }
            if (msg != null)
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                n1 = c1 - 'A';
                n2 = c2 - 'A';
                if(Set(c1)&&Set(c2))
                {
                    map[n1] = n2;
                    map[n2] = n1;
                    if (c1 > c2)
                    {
                        char temp = c1;
                        c1 = c2;
                        c2 = temp;
                    }
                    plugListBox.Items.Add(string.Format("{0} <=> {1}",c1,c2));
                }
            }
        }

        public void Remove()
        {
            while (plugListBox.SelectedItems.Count > 0)
            {
                var item = plugListBox.SelectedItems[0].ToString();
                char c = item[0];
                int n = c - 'A';
                map[map[n]] = map[n];
                map[n] = n;
                plugListBox.Items.Remove(item);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < 26; i++)
                map[i] = i;
            plugListBox.Items.Clear();
        }
    }
}
