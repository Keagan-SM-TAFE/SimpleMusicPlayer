using System.IO;
using System.Windows;
using CsvHelper;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SimpleMusicPlayer
{
    /**
     * Author:             Keagan Young
     * Date:               16 - 06 - 2021
     * Project Name:       Music Player Project Assesment.WPF application.
     * Version:            1.6
     * Description:        This project was created as part of a Diploma in Software Development.
     *                     The requirements of this task are:
     *                          - Must contain dynamic data structures
     *                          - Must contain hashing techniques
     *                          - Must contain sorting algorithm
     *                          - Must contain searching technique
     *                          - Must contain 3rd party library
     *                          - Must have a GUI
     *                          - Must adhere to coding standards
     */

    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin : Window
    {
        /// <summary>
        /// Initialize window login
        /// </summary>
        public WindowLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Drag Window event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs @event)
        {
            this.DragMove();
        }

        /// <summary>
        /// Window exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Btn_exit_Click(object sender, RoutedEventArgs @event)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Open register window button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Button_Click(object sender, RoutedEventArgs @event)
        {
            WindowRegister registerWindow = new WindowRegister();
            registerWindow.Show();
            this.Hide();
        }

        /// <summary>
        /// Login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnLogin_Click(object sender, RoutedEventArgs @event)
        {
            // Check if username and password is not empty
            if (txtUser.Text.Length > 0 && txtPass.Password.Length > 0)
            {
                // Read CSV file to validate the user account
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"/user.csv"))
                {
                    using (var streamReader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"/user.csv"))
                    {
                        using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                        {
                            csvReader.Read();
                            csvReader.ReadHeader();
                            csvReader.Read();
                            if (csvReader.GetField<string>(0).Length > 0)
                            {
                                if (txtUser.Text == csvReader.GetField<string>(1) && csvReader.GetField<string>(2) == ComputeSha256Hash(txtPass.Password))
                                {
                                    MainWindow mainWindow = new MainWindow();
                                    mainWindow.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("The username or password is incorrect.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("This username does not exist!\nPlease register your user account.");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("This username does not exist!\nPlease register your user account.");
                }
            }
            else
            {
                MessageBox.Show("The username or password is missing.");
            }
        }

        /// <summary>
        /// Function to hash the password using Sha256 encryption
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 hash 
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash -- returns a byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array into a string   
                StringBuilder stringBuilder = new StringBuilder();
                for (int count = 0; count < bytes.Length; count++)
                {
                    stringBuilder.Append(bytes[count].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}