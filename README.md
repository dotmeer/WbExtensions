# Персональные расширения для WirenBoard

## Публикация и развертывание

публикация докер-образа:   
`docker build -t cr.yandex/{registry}/wbextensions:{tag} . -f WbExtensions.Service/Dockerfile`
`docker push cr.yandex/{registry}/wbextensions:{tag}`

не забывать логиниться в яндекс через консоль
перекачиваем образ в NAS, там docker-compose с прямым адресом образа

## Roadmap проекта

не по порядку и важности, а по желанию

* свой аналог MediatR
* собственная внутренняя очередь для событий (event-based) с несколькими подписчиками (асинхронно делать метрики прометеуса, пуши Алисы) (аналог notification в MediatR)
* новая версия .net
