@echo off
set docker_compose_file=docker-compose.yml

rem Down and remove existing Docker containers (if any)
docker-compose down

rem Build Docker image
docker-compose build

rem Build and run Docker containers using Docker Compose
docker-compose up -d

rem Display container logs (optional)
docker-compose logs -f