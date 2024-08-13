[![Codacy Badge](https://app.codacy.com/project/badge/Grade/05e6f15388244258b9435b6c64f86691)](https://app.codacy.com/gh/vanUbor/Wiremock/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)

docker build -t wiremock-debug .
docker run -p 8080:8080 -p 8083:8083 -p 5001:5001 wiremock-debug
