const { chromium } = require('playwright');  // или 'firefox', 'webkit'

const totalUsers = 25;  // общее количество пользователей, которых мы хотим подключить
const duration = 100000;  // время для подключения 1000 пользователей (100 секунд = 100000 мс)

const users = [];  // Массив для хранения информации о подключенных пользователях
const connectionTimes = [];

(async () => {
  // Запуск браузера
  const browser = await chromium.launch({ headless: true });  // headless: false — для визуального контроля

  // Создаём контекст с ограничением сети
  const context = await browser.newContext();

  const pages = [];

  // Функция для имитации подключения пользователя
  async function connectUser(userId) {
    const startTime = Date.now();

    const page = await context.newPage();
    pages.push(page);
    // Эмуляция плохой сети через перехват запросов
    await page.route('**/*', (route) => {
      route.continue({
        headers: { ...route.request().headers() },
        timing: { 
          download: 500, // Задержка загрузки, в миллисекундах
          upload: 300,   // Задержка отправки
        },
      });
    });

    await page.goto('https://localhost:7249/');

    await page.waitForTimeout(Math.random() * (duration / totalUsers));

    // Нажатие на кнопку для каждого пользователя
    await page.click('#startAutoBtn');

    // Добавляем информацию о пользователе в массив
    users.push(userId);

    const connectionTime = Date.now() - startTime; // Сколько времени заняло подключение
    connectionTimes.push(connectionTime);

    console.log(`Пользователь ${userId} подключен`);

    // Проверяем, подключено ли 1000 пользователей
    if (users.length === totalUsers) {
      console.log("Все 1000 пользователей подключены!");
      // Начинаем выход пользователей через некоторое время
      startUserExit(pages);
    }
  }

  // Функция для имитации выхода пользователей
  async function startUserExit(pages) {
    for (let i = 0; i < pages.length; i++) {
      await pages[i].waitForTimeout(500);
      console.log(`Пользователь ${users[i]} выходит`);

      await pages[i].keyboard.press('F5',delay=500)
      await pages[i].reload();// Закрытие конкретной страницы
    }
    await browser.close();  // Закрытие браузера после всех выходов
  }
  

  // Имитируем подключение пользователей
  for (let i = 1; i <= totalUsers; i++) {
    await connectUser(i);
  }

  // Экспорт данных в файл (например, JSON)
  const fs = require('fs');
  const data = { users, connectionTimes };
  fs.writeFileSync('connection_data.json', JSON.stringify(data, null, 2));
})();
