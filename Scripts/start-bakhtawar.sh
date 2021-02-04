#!/usr/bin/env bash

systemctl start bakhtawar-gateway.service
systemctl start bakhtawar-web-frontend.service
systemctl start bakhtawar-backend.service
systemctl start bakhtawar-queue.service
systemctl start nginx.service
