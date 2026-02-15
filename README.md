# Персональные расширения для WirenBoard

Интеграция с WB MQTT.
Интеграция с Алисой (pull и push).
Метрики через прометеус в графану.
Телеграм бот для оповещений с контролем доступа.

- [Архитектура и workflow](docs/architecture.aigend.md)

## Публикация и развертывание

публикация докер-образа:   
`docker build -t cr.yandex/{registry}/wbextensions:{tag} . -f WbExtensions.Service/Dockerfile`
`docker push cr.yandex/{registry}/wbextensions:{tag}`

не забывать логиниться в яндекс через консоль
перекачиваем образ в NAS, там docker-compose с прямым адресом образа
