
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/79cbafffb2994a7390d15a0e5c615216)](https://app.codacy.com/gh/vanUbor/Wiremock?utm_source=github.com&utm_medium=referral&utm_content=vanUbor/Wiremock&utm_campaign=Badge_Grade)

docker build -t wiremock-debug .
docker run -p 8080:8080 -p 8083:8083 -p 5001:5001 wiremock-debug
