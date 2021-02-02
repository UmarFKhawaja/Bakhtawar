#!/usr/bin/env bash

pushd .. > /dev/null

archive="Bakhtawar.tgz"

scp $archive root@chavli.com:/var/www/bakhtawar
scp *-bakhtawar.sh root@chavli.com:/root

unset archive

popd > /dev/null
