docker build -t wiremock-debug .
docker run -p 8080:8080 -p 8083:8083 -p 5001:5001 wiremock-debug
