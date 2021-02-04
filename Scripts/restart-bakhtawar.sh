#!/usr/bin/env bash

systemctl stop nginx.service || true
systemctl stop bakhtawar-gateway.service || true
systemctl stop bakhtawar-web-frontend.service || true
systemctl stop bakhtawar-backend.service || true
systemctl stop bakhtawar-queue.service || true

systemctl start bakhtawar-web-frontend.service
systemctl start bakhtawar-backend.service
systemctl start bakhtawar-gateway.service
systemctl start nginx.service
