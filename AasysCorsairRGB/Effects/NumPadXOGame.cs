using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AasysCorsairRGB.KeyboardHook;
using CUE.NET.Devices.Generic;

namespace AasysCorsairRGB
{
    public class NumPadXOGame : IGlobalKeyboardListener, IRunnableFx
    {
        private static readonly String[,] KEYS = new string[3, 3]
        {
            {"Keypad7", "Keypad8", "Keypad9"},
            {"Keypad4", "Keypad5", "Keypad6"},
            {"Keypad1", "Keypad2", "Keypad3"}
        };

        private static readonly String[] OUT_KEYS =
        {
            "NumLock", "KeypadSlash", "KeypadAsterisk", "KeypadMinus",
            "KeypadPlus", "KeypadEnter", "Keypad0", "KeypadPeriodAndDelete"
        };

        private static readonly String[,] KEYS_LISTNER_IDS = new string[3, 3]
        {
            {"NumPad7", "NumPad8", "NumPad9"},
            {"NumPad4", "NumPad5", "NumPad6"},
            {"NumPad1", "NumPad2", "NumPad3"}
        };

        private static readonly String START_ID = "NumPad0";
        private static readonly String CURRENT_PLAYER_KEY = "Keypad0";
        private static readonly String STOP_ID = "Decimal";

        private static readonly Color X_COLOR = Color.Red;
        private static readonly Color O_COLOR = Color.Cyan;

        private readonly CorsairRgb _corsairRgb;
        private readonly CorsairLed[,] _ledGrid = new CorsairLed[3, 3];
        private readonly CorsairLed[] _outLeds;
        private readonly CorsairLed _playerLed;

        private Boolean?[,] _gridMap = new Boolean?[3, 3];
        private Boolean?[] _solutionMap = new bool?[8];

        private bool _gameActive = false;
        private bool _xTurn;
        private bool _singlePlayer = false;
        private bool _humanIsX;
        private int _filledCount = 0;

        public NumPadXOGame()
        {
            _corsairRgb = CorsairRgb.INSTANCE;
            for (int i = 0; i < KEYS.GetLength(0); i++)
            {
                for (int j = 0; j < KEYS.GetLength(1); j++)
                {
                    _ledGrid[i, j] = _corsairRgb.GetKeyboardLed(KEYS[i, j]);
                }
            }

            _outLeds = _corsairRgb.GetKeyboardLeds(OUT_KEYS);
            _playerLed = _corsairRgb.GetKeyboardLed(CURRENT_PLAYER_KEY);
        }

        private void NewGame()
        {
            ClearGrid();
            _xTurn = CommonUtil.Random.Next(2) == 0;
            _filledCount = 0;

            _gameActive = true;

            ColorGrid();
            if (_xTurn)
            {
                RandomPlayX(CommonUtil.Random.Next(9));
            }
        }

        private void ClearGrid()
        {
            for (int i = 0; i < KEYS.GetLength(0); i++)
            {
                for (int j = 0; j < KEYS.GetLength(1); j++)
                {
                    _gridMap[i, j] = null;
                }
            }

            _corsairRgb.ColorAll(_outLeds, Color.Black);
        }

        private void ColorGrid()
        {
            for (int i = 0; i < KEYS.GetLength(0); i++)
            {
                for (int j = 0; j < KEYS.GetLength(1); j++)
                {
                    _ledGrid[i, j].Color = _gridMap[i, j].HasValue
                        ? (_gridMap[i, j].Value ? X_COLOR : O_COLOR)
                        : Color.Black;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                _solutionMap[i] = null;
            }
            _playerLed.Color = _xTurn ? X_COLOR : O_COLOR;
            _corsairRgb.Update();
        }


        public void GameComplete()
        {
            for (int i = 0; i < KEYS.GetLength(0); i++)
            {
                for (int j = 0; j < KEYS.GetLength(1); j++)
                {
                    bool? row = _gridMap[i, j];
                    if (i == 0)
                    {
                        _solutionMap[j] = row;
                    }
                    else
                    {
                        if (!row.HasValue || (_solutionMap[j].HasValue && _solutionMap[j].Value != row.Value))
                        {
                            _solutionMap[j] = null;
                        }
                    }

                    if (j == 0)
                    {
                        _solutionMap[3 + i] = row;
                    }
                    else
                    {
                        if (!row.HasValue || (_solutionMap[3 + i].HasValue && _solutionMap[3 + i].Value != row.Value))
                        {
                            _solutionMap[3 + i] = null;
                        }
                    }
                }

                bool? diag1 = _gridMap[i, i];
                if (i == 0)
                {
                    _solutionMap[6] = diag1;
                }
                else
                {
                    if (!diag1.HasValue || (_solutionMap[6].HasValue && _solutionMap[6].Value != diag1.Value))
                    {
                        _solutionMap[6] = null;
                    }
                }

                bool? diag2 = _gridMap[2 - i, i];
                if (i == 0)
                {
                    _solutionMap[7] = diag2;
                }
                else
                {
                    if (!diag2.HasValue || (_solutionMap[7].HasValue && _solutionMap[7].Value != diag2.Value))
                    {
                        _solutionMap[7] = null;
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (_solutionMap[i].HasValue)
                {
                    _corsairRgb.ColorAll(_outLeds, _solutionMap[i].Value ? X_COLOR : O_COLOR);
                    _gameActive = false;
                    _corsairRgb.Update();
                    return;
                }
            }

            if (_filledCount == 9)
            {
                _gameActive = false;
                _corsairRgb.ColorAll(_outLeds, Color.White);
                _corsairRgb.Update();
                return;
            }
        }

        public void OnKeyPressed(Keys pressedKey)
        {
            Console.WriteLine(pressedKey);

            if (pressedKey.ToString().Equals(START_ID))
            {
                NewGame();
                return;
            }


            if (pressedKey.ToString().Equals(STOP_ID))
            {
                //StaticKeyboardHook.RemoveListner(this);
                _gameActive = false;
                _corsairRgb.ReinitializeSdk();
                return;
            }

            if (_gameActive)
            {

                for (int i = 0; i < KEYS.GetLength(0); i++)
                {
                    for (int j = 0; j < KEYS.GetLength(1); j++)
                    {
                        if (pressedKey.ToString().Equals(KEYS_LISTNER_IDS[i, j]))
                        {
                            if (!_gridMap[i, j].HasValue)
                            {
                                _filledCount++;
                                _gridMap[i, j] = _xTurn;
                                _ledGrid[i, j].Color = _xTurn ? X_COLOR : O_COLOR;
                                _xTurn = !_xTurn;
                                ColorGrid();
                                GameComplete();
                                if (_gameActive && _xTurn)
                                {
                                    RandomPlayX(CommonUtil.Random.Next(9));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnKeyPressed(KeyCombo keyCombo)
        {
            //throw new NotImplementedException();
        }

        private void RandomPlayX(int move)
        {
            for (int i = 0; i < KEYS.GetLength(0); i++)
            {
                for (int j = 0; j < KEYS.GetLength(1); j++)
                {
                    if (!_gridMap[i, j].HasValue)
                    {
                        if (move == 0)
                        {
                            _filledCount++;
                            _gridMap[i, j] = _xTurn;
                            _xTurn = !_xTurn;
                            ColorGrid();
                            GameComplete();
                            move = -1;
                            break;
                        }
                        move--;
                    }
                }
            }

            if (move >= 0)
            {
                RandomPlayX(move);
            }
        }

        public bool Running { get; }
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
