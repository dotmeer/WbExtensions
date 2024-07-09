# Персональные расширения для WirenBoard

## Публикация и развертывание

публикация докер-образа:   
`docker build -t cr.yandex/{registry}/wbextensions:{tag} . -f WbExtensions.Service/Dockerfile`
`docker push cr.yandex/{registry}/wbextensions:{tag}`

не забывать логиниться в яндекс через консоль
перекачиваем образ в NAS, там docker-compose с прямым адресом образа

## Roadmap проекта

не по порядку и важности, а по желанию

* метрики через OpenTelemetry вместо prometheus-net, но отдавать в prometheus, подумать о трейсах. после окончательного релиза
* бот для оповещений WB
* однострочные json-логи
* виртуальные устройства храним в памяти (Application.DevicesManager), через DI в менеджере хэндлеры событий (в БД, в метрики, в яндекс), AliceDevicesManager - становится плослойкой-фасадом для конвертации данных