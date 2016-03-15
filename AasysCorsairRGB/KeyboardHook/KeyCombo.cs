using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace AasysCorsairRGB.KeyboardHook
{
    public class KeyCombo
    {
        private List<Keys> _comboKeys = new List<Keys>();

        public KeyCombo()
        {
        }

        public KeyCombo(Keys key)
        {
            AddKey(key);
        }

        public KeyCombo(Keys[] keys)
        {
            foreach (var key in keys)
            {
                AddKey(key);
            }
        }

        public void AddKey(Keys key)
        {
            if (!_comboKeys.Contains(key))
            {
                _comboKeys.Add(key);
            }
        }

        public void RemoveKey(Keys key)
        {
            if (_comboKeys.Contains(key))
            {
                _comboKeys.Remove(key);
            }
        }

        public bool Equals(KeyCombo keyCombo)
        {
            if (_comboKeys.Count > 0 && _comboKeys.Count == keyCombo._comboKeys.Count)
            {
                foreach (var key in _comboKeys)
                {
                    if (!keyCombo._comboKeys.Contains(key)) return false;
                }
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            if (_comboKeys.Count == 0)
                return "";

            var retStr = _comboKeys[0].ToString();
            for (int i = 1; i < _comboKeys.Count; i++)
            {
                retStr += " + " + _comboKeys[i];
            }
            return retStr;
        }

        public KeyCombo Clone()
        {
            return new KeyCombo(_comboKeys.ToArray());
        }

    }
}
