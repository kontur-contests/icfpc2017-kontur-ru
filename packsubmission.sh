#!/usr/bin/env bash

if [ ! -d "punter/bin/Release/net461" ]; then
  echo "build punter first"
  exit 1
fi

cp -r punter/bin/Release/net461 _submission
pushd _submission

# ./install
echo "#!/usr/bin/env bash" > install
chmod a+x install

# ./punter
mv punter.exe punter
chmod a+x punter

# ./PACKAGES
touch PACKAGES

# ./src
# TODO

# ./README
cp ../README .

tar -cvzf ../icfp-b6ef35be-3e2b-458e-a17f-21cd407a8522.tar.gz .
popd
rm -rf _submission