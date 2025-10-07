### Запуск контейнера БД
```
docker run --name currencyTrackerDb -e POSTGRES_DB=currency_tracker -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin -p 17003:5432 -d postgres:15
```

### Примените миграции
```
cd src/Migration
dotnet run
```
### Запустите сервисы в следующем порядке:
```
Background
Users.API (порт 5001)
Finance.API (порт 5002)
Gateway (порт 5000)
Finance.Grpc (порт 7003)
Документация API:
http://localhost:5000/swagger — агрегированная документация
```