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
    /// Interaction logic for WindowRegister.xaml
    /// </summary>
    public partial class WindowRegister : Window
    {
        /// <summary>
        ///  Initialize window register
        /// </summary>
        public WindowRegister()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Drag window event
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
        /// Open login window button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Button_Click(object sender, RoutedEventArgs @event)
        {
            WindowLogin loginLindow = new WindowLogin();
            loginLindow.Show();
            this.Hide();
        }

        /// <summary>
        /// Register button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnRegister_Click(object sender, RoutedEventArgs @event)
        {
            // Check if email, username, and password is not empty
            if (txtEmail.Text.Length > 0 && txtUser.Text.Length > 0 && txtPass.Password.Length > 0)
            {
                // Write to CSV file to register user
                using (var streamWriter = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + @"/user.csv"))
                {
                    var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);
                    csvWriter.WriteField("Email");
                    csvWriter.WriteField("Username");
                    csvWriter.WriteField("Password");
                    csvWriter.NextRecord();
                    csvWriter.WriteField(txtEmail.Text);
                    csvWriter.WriteField(txtUser.Text);
                    csvWriter.WriteField(ComputeSha256Hash(txtPass.Password));
                    csvWriter.NextRecord();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Hide();
                }
            }
            else
            {
                MessageBox.Show("Please provide all user information!");
            }
        }

        /// <summary>
        /// Function to hash password using Sha256 encryption
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