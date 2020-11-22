#!/usr/bin/env bash

mkdir -p ../Keys
pushd ../Keys

rm -f localhost.cnf
touch localhost.cnf
echo "[req]" >> localhost.cnf
echo "default_bits = 4096" >> localhost.cnf
echo "prompt = no" >> localhost.cnf
echo "default_md = sha256" >> localhost.cnf
echo "distinguished_name = dn" >> localhost.cnf
echo "" >> localhost.cnf
echo "[dn]" >> localhost.cnf
echo "C=UK" >> localhost.cnf
echo "ST=West Midlands" >> localhost.cnf
echo "L=Solihull" >> localhost.cnf
echo "O=Bakhtawar" >> localhost.cnf
echo "OU=Research and Development" >> localhost.cnf
echo "emailAddress=info@bakhtawar.co.uk" >> localhost.cnf
echo "CN = localhost" >> localhost.cnf

rm -f localhost.ext
touch -f localhost.ext
echo "authorityKeyIdentifier=keyid,issuer" >> localhost.ext
echo "basicConstraints=CA:FALSE" >> localhost.ext
echo "keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment" >> localhost.ext
echo "subjectAltName = @alt_names" >> localhost.ext
echo "" >> localhost.ext
echo "[alt_names]" >> localhost.ext
echo "DNS.1 = localhost" >> localhost.ext

openssl req -new -sha256 -nodes -out localhost.csr -newkey rsa:4096 -keyout localhost.key -config <( cat localhost.cnf )

if [ -e ~/Keys/huntingdonresearch.srl ]
then
	openssl x509 -req -in localhost.csr -CA ~/Keys/huntingdonresearch.pem -CAkey ~/Keys/huntingdonresearch.key -CAserial ~/Keys/huntingdonresearch.srl -out localhost.crt -days 365 -sha256 -extfile localhost.ext
else
	openssl x509 -req -in localhost.csr -CA ~/Keys/huntingdonresearch.pem -CAkey ~/Keys/huntingdonresearch.key -CAcreateserial -out localhost.crt -days 365 -sha256 -extfile localhost.ext
fi

rm -f localhost.pem
touch localhost.pem
cat localhost.crt >> localhost.pem
cat localhost.key >> localhost.pem

rm -f localhost.csr localhost.cnf localhost.ext

rm -rf localhost.pfx
openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt

popd
