taskkill /f /im valuator.exe
taskkill /f /im RankCalculator.exe
taskkill /f /im EventLogger.exe

docker stop my-nginx
docker stop my-redis
docker stop my-rabbitmq