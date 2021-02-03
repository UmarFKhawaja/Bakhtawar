#!/usr/bin/env bash

pushd .. > /dev/null

archive="Bakhtawar.tgz"

scp $archive root@chavli.com:/var/www/bakhtawar

unset archive

popd > /dev/null
