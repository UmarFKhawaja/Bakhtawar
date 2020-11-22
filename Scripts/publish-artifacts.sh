#!/usr/bin/env bash

pushd .. > /dev/null

rm -rf Artifacts/
mkdir -p Artifacts/

dotnet clean
dotnet publish

for folder in `find . -type d | grep '/netcoreapp3.1/publish$' | grep 'Apps'`;
do
  name="${folder/\.\/Projects\/}"
  name="${name/\/bin\/Debug\/netcoreapp3\.1\/publish/}"

  source="${folder/\.\//}"
  source="${source/\/bin\/Debug\/netcoreapp3\.1\/publish/}"
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

popd > /dev/null
