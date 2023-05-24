# vertr
Algorithmic Trading Engine

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



