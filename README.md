
# TL;DR

_SSWMUI_ (Super Simple WireMock User Interface) is an ASP.NET Razor Page
making use of [WireMock.NET](https://github.com/WireMock-Net/WireMock.Net),
providing the possiblity to create, start,
stop and configure web api proxy instances.
In its current state it is super limited in its functionality but will grow continuously.

# How does it work
coming soon

# Code quality

## SonarCloud
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vanUbor_Wiremock&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=vanUbor_Wiremock)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=vanUbor_Wiremock&metric=bugs)](https://sonarcloud.io/summary/new_code?id=vanUbor_Wiremock)

[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=vanUbor_Wiremock&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=vanUbor_Wiremock)

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vanUbor_Wiremock&metric=coverage)](https://sonarcloud.io/summary/new_code?id=vanUbor_Wiremock)

[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=vanUbor_Wiremock&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=vanUbor_Wiremock)

## Codacy
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/05e6f15388244258b9435b6c64f86691)](https://app.codacy.com/gh/vanUbor/Wiremock/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)

## Codecov
[![codecov](https://codecov.io/gh/vanUbor/Wiremock/graph/badge.svg?token=ESJLR9JY72)](https://codecov.io/gh/vanUbor/Wiremock)

## GitGuardian
![GitGuardian](https://img.shields.io/badge/GitGuardian-passed-brightgreen)

## Qodana
N/A

## Coderabbit
N/A

## Codefactor
[![CodeFactor](https://www.codefactor.io/repository/github/vanubor/wiremock/badge)](https://www.codefactor.io/repository/github/vanubor/wiremock)

# How to use containers
docker build -t wiremock-debug .
docker run -p 8080:8080 -p 8083:8083 -p 5001:5001 wiremock-debug

# Releases
- none