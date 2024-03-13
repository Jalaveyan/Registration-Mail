using System.Net.Http;
using System.Windows;
using Newtonsoft.Json;
using System.Text;

namespace Clients
{
    public partial class MainWindow : Window
    {
        // Создаем один экземпляр HttpClient, который будет использоваться повторно для всех запросов

        private static HttpClient _httpClient = new()
        {
            //Строка подключения к сервису
            BaseAddress = new Uri("https://localhost:7081/swagger/v1/swagger.json")
        };

        // Конструктор главного окна
        public MainWindow()
        {

            InitializeComponent();
        }

        // Обработчик события клика по кнопке "SendEmail"
        private async void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            await PostAsync(_httpClient); // Вызываем метод для отправки данных
        }

        // Асинхронный метод для отправки данных post запросом
        public async Task PostAsync(HttpClient httpClient)
        {
            //обработчик ошибок
            try
            {
                // Создаем объект с данными для отправки
                var emailObject = new
                {
                    Title = txtTitle.Text,
                    DateSent = DateTime.Now,
                    Recipient = txtRecipient.Text,
                    Sender = txtSender.Text,
                    Content = txtContent.Text
                };

                // Сериализуем объект в формат JSON
                string jsonContent = JsonConvert.SerializeObject(emailObject);

                // Создаем контент запроса с использованием JSON
                using StringContent stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Отправляем POST-запрос на сервис
                using HttpResponseMessage response = await httpClient.PostAsync("/api/PersonEmail", stringContent);

                // Проверяем успешность запроса (статус код 2xx)
                response.EnsureSuccessStatusCode();

                // Получаем и выводим ответ от сервера
                var jsonResponce = await response.Content.ReadAsStringAsync();
                await Console.Out.WriteLineAsync($"{jsonResponce}");

            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Ошибка HTTP запроса: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Ошибка при сериализации/десериализации JSON: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла неизвестная ошибка: {ex.Message}");
            }
        }
    }
}
