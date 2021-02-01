#!/usr/bin/env bash

systemctl start bakhtawar-web-frontend.service
systemctl start bakhtawar-backend.service
systemctl start bakhtawar-gateway.service
systemctl start nginx.service
