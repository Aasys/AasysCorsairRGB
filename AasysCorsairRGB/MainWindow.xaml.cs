

using System;
using System.Windows;
using System.Windows.Forms;
using AasysCorsairRGB.Effects;
using AasysCorsairRGB.KeyboardHook;
using Application = System.Windows.Forms.Application;

namespace AasysCorsairRGB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IGlobalKeyboardListener
    {

        private static readonly KeyCombo KEY_COMBO_FN_CLOCK = new KeyCombo(new Keys[] { Keys.Insert, Keys.Home });
        private static readonly KeyCombo KEY_COMBO_BINARY_CLOCK = new KeyCombo(new Keys[] { Keys.Insert, Keys.PageUp });
        private static readonly KeyCombo KEY_COMBO_PERF_FX = new KeyCombo(new Keys[] { Keys.Insert, Keys.PageDown });
        private static readonly KeyCombo KEY_COMBO_RANDOM_FX = new KeyCombo(new Keys[] { Keys.Insert, Keys.Delete });
        private static readonly KeyCombo KEY_COMBO_DOTA2_FX = new KeyCombo(new Keys[] { Keys.Insert, Keys.End });

        private NotifyIcon _tray;
        private MenuItem _menuItemExit;

        private MenuItem _menuItemFnClock;
        private MenuItem _menuItemBinaryClock;
        private MenuItem _menuItemPerfFx;
        private MenuItem _menuItemRandomFx;
        private MenuItem _menuItemDota2Fx;
        private MenuItem _menuItemScreenFx;

        private CorsairRgb _corsairRgb;
        private FunctionClockRgb _functionClockRgb;
        private BinaryGkeyColock _binaryGkeyColock;
        private PerformanceFx _performanceFx;
        private NumPadXOGame _numPadXoGame;
        private RandomFlashes _randomFlashes;
        private DotaFX _dotaFx;
        private ScreenMappingFx _screenMappingFx;
        private ScreenThemeFx _screenThemeFx;

        public MainWindow()
        {
            InitializeComponent();

            WindowState = WindowState.Minimized;
            Visibility = System.Windows.Visibility.Hidden;

            InitializeTray();
            _corsairRgb = CorsairRgb.INSTANCE;


            _functionClockRgb = new FunctionClockRgb();
            _binaryGkeyColock = new BinaryGkeyColock();
            _performanceFx = new PerformanceFx();
            _numPadXoGame = new NumPadXOGame();
            _randomFlashes = new RandomFlashes();
            _dotaFx = new DotaFX();
            _screenMappingFx = new ScreenMappingFx();
            _screenThemeFx = new ScreenThemeFx();

            StaticKeyboardHook.AddListner(this);
            StaticKeyboardHook.AddListner(_numPadXoGame);
            
            StaticKeyboardHook.Start();
            
            //Initial run
            //_screenThemeFx.Start();
            trayItemScreenFx_Click(this, null);
        }

    private void InitializeTray()
        {
            var contextMenu = new ContextMenu();

            _menuItemExit = new MenuItem {Text = "E&xit"};
            _menuItemFnClock = new MenuItem {Text = "&Fn Keys Clock"};
            _menuItemBinaryClock = new MenuItem() { Text = "&Binary Gkey Clock" };
            _menuItemPerfFx = new MenuItem() { Text = "&Performance Fx" };
            _menuItemRandomFx = new MenuItem() { Text = "&Random Fx" };
            _menuItemDota2Fx = new MenuItem() { Text = "&Dota2 Fx" };
            _menuItemScreenFx = new MenuItem() { Text = "&Screen Fx" };

            _menuItemExit.Click += new EventHandler(trayItemExit_Click);
            _menuItemFnClock.Click += new EventHandler(trayItemFnClock_Click);
            _menuItemBinaryClock.Click += new EventHandler(trayItemBinaryClock_Click);
            _menuItemPerfFx.Click += new EventHandler(trayItemPerfFx_Click);
            _menuItemRandomFx.Click += new EventHandler(trayItemRandomFx_Click);
            _menuItemDota2Fx.Click += new EventHandler(trayItemDota2Fx_Click);
            _menuItemScreenFx.Click += new EventHandler(trayItemScreenFx_Click);
            

            contextMenu.MenuItems.AddRange(new MenuItem[]{
                _menuItemFnClock,
                _menuItemBinaryClock,
                _menuItemPerfFx,
                _menuItemRandomFx,
                _menuItemDota2Fx,
                _menuItemScreenFx,
                _menuItemExit
            });
            _tray = new NotifyIcon()
            {
                Visible = true,
                Icon = Properties.Resources.AppIcon,
                ContextMenu = contextMenu
            };
            _tray.Click += new EventHandler(tray_Click);
        }

        private void trayItemScreenFx_Click(object sender, EventArgs e)
        {
            FxStartStop(_screenMappingFx);
            _menuItemScreenFx.Checked = _screenMappingFx.Running;
        }

        private void trayItemDota2Fx_Click(object sender, EventArgs e)
        {
            FxStartStop(_dotaFx);
            _menuItemDota2Fx.Checked = _dotaFx.Running;
        }

        private void trayItemRandomFx_Click(object sender, EventArgs e)
        {
            FxStartStop(_randomFlashes);
            _menuItemRandomFx.Checked = _randomFlashes.Running;
        }

        private void trayItemPerfFx_Click(object sender, EventArgs e)
        {
            FxStartStop(_performanceFx);
            _menuItemPerfFx.Checked = _performanceFx.Running;
        }

        private void trayItemBinaryClock_Click(object sender, EventArgs e)
        {
            FxStartStop(_binaryGkeyColock);
            _menuItemBinaryClock.Checked = _binaryGkeyColock.Running;
        }

        private void trayItemFnClock_Click(object sender, EventArgs e)
        {
            FxStartStop(_functionClockRgb);
            _menuItemFnClock.Checked = _functionClockRgb.Running;
        }

        private void tray_Click(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void trayItemExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        
        public void OnKeyPressed(KeyCombo keyCombo)
        {
            if (keyCombo.Equals(KEY_COMBO_FN_CLOCK))
            {
                FxStartStop(_functionClockRgb);
                _menuItemFnClock.Checked = _functionClockRgb.Running;
            } else if (keyCombo.Equals(KEY_COMBO_PERF_FX))
            {
                FxStartStop(_performanceFx);
                _menuItemPerfFx.Checked = _performanceFx.Running;
            } else if (keyCombo.Equals(KEY_COMBO_RANDOM_FX))
            {
                FxStartStop(_randomFlashes);
                _menuItemRandomFx.Checked = _randomFlashes.Running;
            } else if (keyCombo.Equals(KEY_COMBO_DOTA2_FX))
            {
                FxStartStop(_dotaFx);
                _menuItemDota2Fx.Checked = _dotaFx.Running;
            } else if (keyCombo.Equals(KEY_COMBO_BINARY_CLOCK))
            {
                FxStartStop(_binaryGkeyColock);
                _menuItemBinaryClock.Checked = _binaryGkeyColock.Running;
            }
        }

        public void FxStartStop(IRunnableFx runnableFx)
        {
            if (!runnableFx.Running)
            {
                runnableFx.Start();
            }
            else
            {
                runnableFx.Stop();
                _corsairRgb.ReinitializeSdk();
            }
        }
    }
}
