#!/usr/bin/env bash

pushd .. > /dev/null

export GATEWAY_PREFIX=https://id.bakhtawar.co.uk
export BACKEND_PREFIX=/api

envsubst < Templates/Projects/Bakhtawar.Apps.WebFrontendApp/App/src/services/prefix-holder.js.tmpl > Projects/Bakhtawar.Apps.WebFrontendApp/App/src/services/prefix-holder.js 

unset GATEWAY_PREFIX
unset BACKEND_PREFIX

rm -rf Artifacts/
mkdir -p Artifacts/

dotnet clean
dotnet publish

for folder in `find . -type d | grep '/net5.0/publish$' | grep 'Apps'`;
do
  name="${folder/\.\/Projects\/}"
  name="${name/\/bin\/Debug\/net5\.0\/publish/}"

  source="${folder/\.\//}"
  source="${source/\/bin\/Debug\/net5\.0\/publish/}"
  target="Artifacts/${name}"
  
  cp -R $folder $target
  rm "$target/appsettings.Development.json"
  Scripts/generate-sysconfigs.js $name $source $target
  cp Keys/localhost.pfx $target

  unset name
  unset source
  unset target
done;

archive="Bakhtawar.tgz"

pushd Artifacts/ > /dev/null

tar cvfz $archive *
mv $archive ..

popd > /dev/null

unset archive

git checkout HEAD -- Projects/Bakhtawar.Apps.WebFrontendApp/App/src/services/prefix-holder.js

popd > /dev/null
