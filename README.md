### Запуск контейнера БД
```
docker run --name currencyTrackerDb -e POSTGRES_DB=currency_tracker -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin -p 17003:5432 -d postgres:15
```