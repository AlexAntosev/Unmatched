@echo off
set docker_compose_file=docker-compose.yml

rem Stop specific services
docker-compose -f %docker_compose_file% stop ms catalogservice

rem Remove stopped containers
docker-compose -f %docker_compose_file% rm -f ms catalogservice

rem Rebuild
docker-compose -f %docker_compose_file% build ms catalogservice

rem Start
docker-compose -f %docker_compose_file% up -d ms catalogservice

rem Show logs
docker-compose -f %docker_compose_file% logs -f ms catalogservice