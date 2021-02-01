#!/usr/bin/env bash

systemctl stop nginx.service
systemctl stop bakhtawar-gateway.service
systemctl stop bakhtawar-backend.service
systemctl stop bakhtawar-web-frontend.service

systemctl start bakhtawar-web-frontend.service
systemctl start bakhtawar-backend.service
systemctl start bakhtawar-gateway.service
systemctl start nginx.service
