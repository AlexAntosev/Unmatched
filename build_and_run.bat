@echo off
set service_name=ms
set docker_compose_file=docker-compose.yml

rem Stop and remove specific container
docker-compose -f %docker_compose_file% rm -sf %service_name%

rem Rebuild only that service
docker-compose -f %docker_compose_file% build %service_name%

rem Start it again
docker-compose -f %docker_compose_file% up -d %service_name%

rem Show logs
docker-compose -f %docker_compose_file% logs -f %service_name%