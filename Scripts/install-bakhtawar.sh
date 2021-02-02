#!/usr/bin/env bash

pushd /var/systemd/system/ > /dev/null

systemctl disable bakhtawar-backend.service
systemctl disable bakhtawar-gateway.service
systemctl disable bakhtawar-web-frontend.service

cat <<EOF > bakhtawar-backend.service
[Unit]
Description="Bakhtawar Backend"
After=network.target

[Service]
User=bakhtawar
Group=bakhtawar
WorkingDirectory=/var/www/bakhtawar/Bakhtawar.Apps.BackendApp
ExecStart=/usr/bin/dotnet Bakhtawar.Apps.BackendApp.dll
KillMode=process
StandardOutput=syslog
StandardError=syslog
SyslogIdentifier=bakhtawar-backend
Restart=on-failure
RestartSec=10
EnvironmentFile=/var/www/bakhtawar/Bakhtawar.Apps.BackendApp/Bakhtawar.Apps.BackendApp.sysconfig

[Install]
WantedBy=multi-user.target
EOF

cat <<EOF > bakhtawar-gateway.service
[Unit]
Description="Bakhtawar Gateway"
After=network.target

[Service]
User=bakhtawar
Group=bakhtawar
WorkingDirectory=/var/www/bakhtawar/Bakhtawar.Apps.GatewayApp
ExecStart=/usr/bin/dotnet Bakhtawar.Apps.GatewayApp.dll
KillMode=process
StandardOutput=syslog
StandardError=syslog
SyslogIdentifier=bakhtawar-gateway
Restart=on-failure
RestartSec=10
EnvironmentFile=/var/www/bakhtawar/Bakhtawar.Apps.GatewayApp/Bakhtawar.Apps.GatewayApp.sysconfig

[Install]
WantedBy=multi-user.target
EOF

cat <<EOF > bakhtawar-web-frontend.service
[Unit]
Description="Bakhtawar Web Frontend"
After=network.target

[Service]
User=bakhtawar
Group=bakhtawar
WorkingDirectory=/var/www/bakhtawar/Bakhtawar.Apps.WebFrontendApp
ExecStart=/usr/bin/dotnet Bakhtawar.Apps.WebFrontendApp.dll
KillMode=process
StandardOutput=syslog
StandardError=syslog
SyslogIdentifier=bakhtawar-web-frontend
Restart=on-failure
RestartSec=10
EnvironmentFile=/var/www/bakhtawar/Bakhtawar.Apps.WebFrontendApp/Bakhtawar.Apps.WebFrontendApp.sysconfig

[Install]
WantedBy=multi-user.target
EOF

systemctl enable bakhtawar-backend.service
systemctl enable bakhtawar-gateway.service
systemctl enable bakhtawar-web-frontend.service

popd > /dev/null
