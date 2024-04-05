# Персональные расширения для WirenBoard

## Публикация и развертывание

публикация докер-образа:   
`docker build -t dotmeer/wbextensions:{tag} . -f dotmeer.WbExtensions.Service/Dockerfile`   
`docker login`   
`docker push dotmeer/wbextensions:{tag}`

перекачиваем образ в NAS, там docker-compose

## Roadmap проекта

не по порядку и важности, а по желанию

* метрики через OpenTelemetry вместо prometheus-net, но отдавать в prometheus, подумать о трейсах. после окончательного релиза
* переименовать проект и прочее на NASе, переименовать домен для подключения
* сохранение данных в sqlite через очередь в репозитории, для контейнера сделать прокидывание файла в файловую систему
* интеграция с Алисой (перенести из AliceAPI)
* бот для оповещений WB