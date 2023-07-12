# vertr
Algorithmic Trading Engine

# 12-07-23
- перераспределить сущности по папкам
- доделать генерацию всех евентов из OrderCommand
- разбить один евент консьюмер на три
- для всех объектов перенести общие свойства в базовые классы

## 10-07-23
- доделать MatcherEvent и обвязку вокруг него
- доделать TradeEvents и обвязку вокруг них
- генерировать маркет дату
- зарефакторить хендлеры дисраптора. Оставить только действительно нужные
- переключиться на матчинг енжин


## 09-07-23
- Disruptor Exception Hhandler
- Async events handler
- Не возвращать OrderCommand в евентах - нужен другой объект
- implement API commands

## 02-07-23

### US-1 Basic Disruptor Engine via REST API
- ASP.NET хост Vertr.Exchange.Server
- простой REST API для поткока входящих ордеров
- простой набор хендлеров для дисраптора
-- order command processor - закидывает в Disruptor
-- order matching - матчит ордера
-- event processor - собирает и создает события
-- event publisher - публикует события в консоль
Дополнительно для тестов
- простая inMemory DB для хранения событий вместе/вместо консоли/лога
- простой REST API для извлечения и просмотра событий из DB

### US-1-A Доработка (разработка) алгоритмов Mathing Engine
- доменная модель и сущности, структуры данных
- простой наивный алгоритм
- типовой алгоритм

### US-2 Замена/дополнение REST Аероном
- ASP.NET хост Vertr.Exchange.Proxy
- Каналы аерона для связи с сервером
- REST/Grpc API для потока ордеров
- DB - хранение информации о клиенте, ордерах и событиях
- REST/GRPC клиенты для запроса информации (ордера, портфель, маркет даата, сделки и тп.)

### Бэклог
- Раутинг и мультиплексирование запросов по книгам (символам)
- Администрирование символов
- Администрирование аккаунта. Депозит. Несколько аккаунтов
- Клиентский портфель и его показатели
 


## 16-06-23
- Неявный пайплайн через ченнел. Неудобно и не видно, что происходит. Нужна более явная логика
- Сложная схема с книгами - нужно упростить и переделать

## 25-05-23
- MessageQueue, MessageQueueProvider
- Notifications(Events) in Domain - IMediator
- Remaining Qty for Orders?

## 24-05-23
- Add MarketData - Best Bid-Ask, LastTrade Ptrice/Qty, Time

## 22-05-23 
- Command Processor (Mediatr)
- Cancel Order Command - (Hashset + Stack)

## 19-05-23 Order matching engine

- не регистрируем сделки Market - Market (тк нужно брать откуда-то цену)
- для маркет ордеров - всегда ходим в лимитную книгу, если там ничего нет - кладем его в маркет книгу
- для лимит ордеров - всегда вначале проверяем маркетную книгу

### Тесты
- тесты для ордербука
- тесты для маркет ордеров
- наполнение ордербуков
- снапшонты ордербука до и после матчинга ордера

### Функционал
- (!) отмена (cancel) ордеров в книге?
- визуальное отображение книг (стакан заявок) - в статике и динамике
- доменная обвязка - хранение ордеров и трейдов
- событийная модель - что использовать? хранение событий?

### Вопросы
- Переопределен ли хэшкод и операции сравнения в структурах? Как проверить?
- Массив и коллекция стуктур размещаются в стеке?
- Можно сделать спан из трейдов и передать его наружу
- Матчи правильно отрабатывают при всех комбинациях ордеров и книжек
- Цена сделки соответствует полиси
- Сервис работает в Zero allcation моде
- Сервис держит нагрузку
- Чем заменить Debug.Assert?



