using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace GUIFrontend
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string DROP_NAVI_TEXT = "ここに動画ファイルをドロップしてください。";

        public static readonly ICommand ExecCommand = new RoutedCommand(nameof(ExecCommand), typeof(MainWindow));

        public ObservableCollection<string> DropListItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.InitItemBindings();
            this.InitCommandBindings();
        }

        private void InitItemBindings()
        {
            this.DropListItems = new ObservableCollection<string>();
            Enumerable.Range(0, 5).ToList().ForEach(x => this.DropListItems.Add(DROP_NAVI_TEXT));
            dropList.ItemsSource = this.DropListItems;
        }

        private void InitCommandBindings()
        {
            this.CommandBindings.Add(new CommandBinding(ExecCommand, Exec_Execute, Exec_CanExecute));
        }

        /// <summary>
        /// プログラム実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exec_Execute(object sender, RoutedEventArgs e)
        {
            string option = optionText.Text.Trim();

            if (execOnceCheck.IsChecked == true)
            {
                string args = option + " ";
                // ドロップファイルをまとめて引数に渡し、1回だけプログラム実行
                foreach (var item in this.DropListItems)
                {
                    args += $"\"{item}\" ";
                }

                System.Diagnostics.Process.Start(commandText.Text, args);
            }
            else
            {
                // ドロップファイル数分、プログラム実行
                foreach (var item in this.DropListItems)
                {
                    System.Diagnostics.Process.Start(commandText.Text, $"{option} \"{item}\"");
                }
            }
        }

        private void Exec_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // プログラムパスが入力されていて、
            // ファイルがドロップされていれば実行ボタン有効
            if (commandText.Text != "" &&
                this.DropListItems.Any() &&
                this.DropListItems[0] != DROP_NAVI_TEXT)
            {
                e.CanExecute = true;
            }
        }
        

        ///実行ボタンをクリック後コマンドラインを呼び出してbatファイルを実行
        private void ExecButton_Click(object sender, EventArgs e)
        {
            //Processオブジェクトを作成
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            //入力できるようにする
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.ErrorDialog = false;

            //非同期で出力を読み取れるようにする
            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += p_OutputDataReceived;

            p.StartInfo.FileName =
                System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.CreateNoWindow = true;

            //起動
            p.Start();

            //非同期で出力の読み取りを開始
            p.BeginOutputReadLine();

            //入力のストリームを取得
            System.IO.StreamWriter sw = p.StandardInput;
            if (sw.BaseStream.CanWrite)
            {

                //「dir c:\ /w」を実行する
                sw.WriteLine(@"activate tensorflow1.5");
                //「dir d:\ /w」を実行する
                sw.WriteLine(@"C:\MMD\openpose\motion_trace_bulk\MotionTraceBulk.bat");
                //ドラッグしてきた動画ファイルを実行

                Console.ReadLine();

            }
            sw.Close();

            p.WaitForExit();
            p.Close();

            Console.ReadLine();
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }





        /// <summary>
        /// ファイルドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dropList_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files != null)
            {
                this.DropListItems.Clear();
                foreach (var s in files)
                {
                    this.DropListItems.Add(s);
                }

                execOnceCheck.Focus();
            }
        }

        private void dropList_PreviewDragOver(object sender, DragEventArgs e)
        {
            // ドラッグ中のデータの形式チェック
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
    





        private void execButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CommandText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}