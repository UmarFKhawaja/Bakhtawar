#!/usr/bin/env bash

if [ -e /var/www/bakhtawar/Bakhtawar.tgz ];
then
	systemctl stop nginx.service
	systemctl stop bakhtawar-gateway.service
	systemctl stop bakhtawar-backend.service
	systemctl stop bakhtawar-web-frontend.service

    mkdir -p /var/www/bakhtawar
    chown -R bakhtawar:bakhtawar /var/www/bakhtawar
    chmod 777 /var/www/bakhtawar

	pushd /var/www/bakhtawar > /dev/null

	rm -rf Bakhtawar.Apps.BackendApp/
	rm -rf Bakhtawar.Apps.DatabaseApp/
	rm -rf Bakhtawar.Apps.GatewayApp/
	rm -rf Bakhtawar.Apps.WebFrontendApp/
	tar xvfz Bakhtawar.tgz
	rm Bakhtawar.tgz
	chown -R bakhtawar:bakhtawar *

	popd > /dev/null

	systemctl start bakhtawar-web-frontend.service
	systemctl start bakhtawar-backend.service
	systemctl start bakhtawar-gateway.service
	systemctl start nginx.service
else
	echo "cannot find Bakhtawar.tgz; ensure it has been uploaded"
fi
