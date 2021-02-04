#!/usr/bin/env bash

pushd .. > /dev/null

archive="Bakhtawar.tgz"

scp $archive root@chavli.com:/var/www/bakhtawar
rm $archive

unset archive

popd > /dev/null
